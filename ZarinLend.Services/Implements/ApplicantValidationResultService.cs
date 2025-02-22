using Common;
using Common.Exceptions;
using Common.Utilities;
using Core.Data.Repositories;
using Core.Entities;
using Core.Entities.Business.RequestFacility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Services.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class ApplicantValidationResultService : IApplicantValidationResultService, IScopedDependency
    {
        private readonly ILogger<ApplicantValidationResultService> logger;
        private readonly IBaseRepository<ApplicantValidationResult> applicantValidationResultRepository;
        private readonly IBaseRepository<VerifyResultExcelDetail> verifyResultExcelRepository;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ApplicantValidationResultService(ILogger<ApplicantValidationResultService> logger, IBaseRepository<ApplicantValidationResult> applicantValidationResultRepository,
           IBaseRepository<VerifyResultExcelDetail> verifyResultExcelRepository, IWebHostEnvironment webHostEnvironment)
        {
            this.logger = logger;
            this.applicantValidationResultRepository = applicantValidationResultRepository;
            this.verifyResultExcelRepository = verifyResultExcelRepository;
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<ApplicantValidationResultModel>> SelectValidationResult(int requestFacilityId, int organizationId, CancellationToken cancellationToken)
        {
            return await SelectByRequestFacilityAndOrganization(requestFacilityId, organizationId, cancellationToken);
        }

        public async Task<List<ApplicantValidationResultModel>> SelectValidationResult(int requestFacilityId, CancellationToken cancellationToken)
        {
            return await SelectByRequestFacilityAndOrganization(requestFacilityId, null, cancellationToken);
        }

        private async Task<List<ApplicantValidationResultModel>> SelectByRequestFacilityAndOrganization(int requestFacilityId, int? organizationId, CancellationToken cancellationToken)
        {
            return (await applicantValidationResultRepository.SelectByAsync(p => p.RequestFacilityId == requestFacilityId && (!organizationId.HasValue || p.OrganizationId.Equals(organizationId)),
                p => new ApplicantValidationResultModel()
                {
                    RequestFacilityId = p.RequestFacilityId,
                    OrganizationName = p.Organization.Name,
                    BlackListInquiry = p.BlackListInquiry,
                    FacilityInquiry = p.FacilityInquiry,
                    CivilRegistryInquiry = p.CivilRegistryInquiry,
                    MilitaryInquiry = p.MilitaryInquiry,
                    PostalCodeInquiry = p.PostalCodeInquiry,
                    ReturnedCheckInquiry = p.ReturnedCheckInquiry,
                    SecurityCouncilSanctionsInquiry = p.SecurityCouncilSanctionsInquiry,
                    ShahkarInquiry = p.ShahkarInquiry,
                    CreatorId = p.CreatorId,
                    CreatedDate = p.CreatedDate,
                    Creator = $"{p.Creator.Person.FName} {p.Creator.Person.LName}"
                }, cancellationToken))
                .ToList();
        }

        public List<ApplicantValidationResultModel> ExtractVerifyResultFromExcel(Stream stream)
        {
            using (var excelPackage = new ExcelPackage(stream))
            {
                var result = new List<ApplicantValidationResultModel>();
                ApplicantValidationResultModel item = null;
                if (excelPackage.Workbook.Worksheets.Count > 0)
                {
                    var workSheet = excelPackage.Workbook.Worksheets[0];

                    #region find column in excel sheet

                    var nationalCodeColumn = workSheet.Cells["1:1"].FirstOrDefault(c => c.Value != null && c.Value.ToString().CleanString().Replace(" ", string.Empty) == "کدملی");
                    if (nationalCodeColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'کد ملی' در فایل اکسل یافت نشد");

                    var civilRegistryInquiryColumn = workSheet.Cells["1:1"]
                          .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استعلام ثبت احوال".CleanString().Replace(" ", string.Empty));
                    if (civilRegistryInquiryColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'استعلام ثبت احوال' در فایل اکسل یافت نشد");

                    var returnedCheckInquiryColumn = workSheet.Cells["1:1"]
                           .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استعلام چک برگشتی".CleanString().Replace(" ", string.Empty));
                    if (returnedCheckInquiryColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'استعلام چک برگشتی' در فایل اکسل یافت نشد");

                    var facilityInquiryColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استعلام تسهیلات و تعهدات".CleanString().Replace(" ", string.Empty));
                    if (facilityInquiryColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'استعلام تسهیلات و تعهدات' در فایل اکسل یافت نشد");

                    var postalCodeInquiryColumn = workSheet.Cells["1:1"]
                            .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استعلام کدپستی به آدرس".CleanString().Replace(" ", string.Empty));
                    if (postalCodeInquiryColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'استعلام کدپستی به آدرس' در فایل اکسل یافت نشد");

                    var securityCouncilSanctionsInquiryColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استعلام تحریم شورا امنیت".CleanString().Replace(" ", string.Empty));
                    if (securityCouncilSanctionsInquiryColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'استعلام تحریم شورا امنیت' در فایل اکسل یافت نشد");

                    var shahkarInquiryColumn = workSheet.Cells["1:1"]
                          .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استعلام شاهکار".CleanString().Replace(" ", string.Empty));
                    if (shahkarInquiryColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'استعلام شاهکار' در فایل اکسل یافت نشد");

                    var militaryInquiryColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استعلام نظام وظیفه".CleanString().Replace(" ", string.Empty));
                    if (militaryInquiryColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'استعلام نظام وظیفه' در فایل اکسل یافت نشد");

                    var blackListInquiryColumn = workSheet.Cells["1:1"]
                          .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استعلام لیست سیاه".CleanString().Replace(" ", string.Empty));
                    if (blackListInquiryColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'استعلام لیست سیاه' در فایل اکسل یافت نشد");
                    #endregion

                    var dataRowIndex = 2;
                    var regexNationalCode = new Regex(RegularExpression.NationalCode);
                    while (workSheet.Dimension.End.Row >= dataRowIndex)
                    {
                        item = new ApplicantValidationResultModel();
                        List<string> errorList = new List<string>();
                        if (workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value == null ||
                            string.IsNullOrEmpty(workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value.ToString().CleanString().Replace(" ", string.Empty)))
                        {
                            errorList.Add($"کد ملی وارد شده در سطر {dataRowIndex}ام با فرمت کد ملی مطابقت ندارد<br/>");
                            item.HasError = true;
                            continue;
                        }
                        if (!regexNationalCode.IsMatch(workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value.ToString()))
                        {
                            item.NationalCode = Convert.ToString(workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value);
                            item.HasError = true;
                            errorList.Add($"کد ملی وارد شده در سطر {dataRowIndex}ام با فرمت کد ملی مطابقت ندارد<br/>");
                            continue;
                        }
                        item.NationalCode = workSheet.Cells[dataRowIndex, nationalCodeColumn.Start.Column].Value.ToString().CleanString();

                        item.CivilRegistryInquiry = CheckInquiryValue(Convert.ToString(workSheet.Cells[dataRowIndex, civilRegistryInquiryColumn.Start.Column].Value));
                        item.ReturnedCheckInquiry = CheckInquiryValue(Convert.ToString(workSheet.Cells[dataRowIndex, returnedCheckInquiryColumn.Start.Column].Value));
                        item.FacilityInquiry = CheckInquiryValue(Convert.ToString(workSheet.Cells[dataRowIndex, facilityInquiryColumn.Start.Column].Value));
                        item.PostalCodeInquiry = CheckInquiryValue(Convert.ToString(workSheet.Cells[dataRowIndex, postalCodeInquiryColumn.Start.Column].Value));
                        item.SecurityCouncilSanctionsInquiry = CheckInquiryValue(Convert.ToString(workSheet.Cells[dataRowIndex, securityCouncilSanctionsInquiryColumn.Start.Column].Value));
                        item.ShahkarInquiry = CheckInquiryValue(Convert.ToString(workSheet.Cells[dataRowIndex, shahkarInquiryColumn.Start.Column].Value));
                        item.MilitaryInquiry = CheckInquiryValue(Convert.ToString(workSheet.Cells[dataRowIndex, militaryInquiryColumn.Start.Column].Value));
                        item.BlackListInquiry = CheckInquiryValue(Convert.ToString(workSheet.Cells[dataRowIndex, blackListInquiryColumn.Start.Column].Value));

                        if (!item.BlackListInquiry.HasValue)
                            errorList.Add("استعلام لیست سیاه نامشخص است");
                        else if (!item.BlackListInquiry.Value)
                            errorList.Add("استعلام لیست سیاه رد شد");

                        if (!item.CivilRegistryInquiry.HasValue)
                            errorList.Add("استعلام ثبت احوال نامشخص است");
                        else if (!item.CivilRegistryInquiry.Value)
                            errorList.Add("استعلام ثبت احوال رد شد");

                        if (!item.FacilityInquiry.HasValue)
                            errorList.Add("استعلام تسهیلات نامشخص است");
                        else if (!item.FacilityInquiry.Value)
                            errorList.Add("استعلام تسهیلات رد شد");

                        if (!item.MilitaryInquiry.HasValue)
                            errorList.Add("استعلام نظام وظیفه نامشخص است");
                        else if (!item.MilitaryInquiry.Value)
                            errorList.Add("استعلام نظام وظیفه رد شد");

                        if (!item.PostalCodeInquiry.HasValue)
                            errorList.Add("استعلام کد پستی نامشخص است");
                        else if (!item.PostalCodeInquiry.Value)
                            errorList.Add("استعلام کد پستی رد شد");

                        if (!item.ReturnedCheckInquiry.HasValue)
                            errorList.Add("استعلام چک برگشتی نامشخص است");
                        else if (!item.ReturnedCheckInquiry.Value)
                            errorList.Add("استعلام چک برگشتی رد شد");

                        if (!item.SecurityCouncilSanctionsInquiry.HasValue)
                            errorList.Add("استعلام تحریم شورا امنیت نامشخص است");
                        else if (!item.SecurityCouncilSanctionsInquiry.Value)
                            errorList.Add("استعلام تحریم شورا امنیت رد شد");

                        if (!item.ShahkarInquiry.HasValue)
                            errorList.Add("استعلام شاهکار نامشخص است");
                        else if (!item.ShahkarInquiry.Value)
                            errorList.Add("استعلام شاهکار رد شد");

                        if (errorList.Any())
                            item.ErrorMessage = string.Join("<br/>", errorList);

                        item.FinalResult = item.BlackListInquiry.HasValue && item.BlackListInquiry.Value &&
                                 item.CivilRegistryInquiry.HasValue && item.CivilRegistryInquiry.Value &&
                                 item.FacilityInquiry.HasValue && item.FacilityInquiry.Value &&
                                 item.MilitaryInquiry.HasValue && item.MilitaryInquiry.Value &&
                                 item.PostalCodeInquiry.HasValue && item.PostalCodeInquiry.Value &&
                                 item.ReturnedCheckInquiry.HasValue && item.ReturnedCheckInquiry.Value &&
                                 item.SecurityCouncilSanctionsInquiry.HasValue && item.SecurityCouncilSanctionsInquiry.Value &&
                                 item.ShahkarInquiry.HasValue && item.ShahkarInquiry.Value
                                 ? true
                                 : ((item.BlackListInquiry.HasValue && !item.BlackListInquiry.Value) ||
                                    (item.CivilRegistryInquiry.HasValue && !item.CivilRegistryInquiry.Value) ||
                                    (item.FacilityInquiry.HasValue && !item.FacilityInquiry.Value) ||
                                    (item.MilitaryInquiry.HasValue && !item.MilitaryInquiry.Value) ||
                                    (item.PostalCodeInquiry.HasValue && !item.PostalCodeInquiry.Value) ||
                                    (item.ReturnedCheckInquiry.HasValue && !item.ReturnedCheckInquiry.Value) ||
                                    (item.SecurityCouncilSanctionsInquiry.HasValue && !item.SecurityCouncilSanctionsInquiry.Value) ||
                                    (item.ShahkarInquiry.HasValue && !item.ShahkarInquiry.Value))
                                    ? false
                                    : (bool?)null;

                        result.Add(item);
                        dataRowIndex++;
                    }
                }
                return result;
            }
        }

        private List<string> ConfirmValues = new List<string>() { "بله", "تایید", "تائید" };
        private List<string> RejectValues = new List<string>() { "خیر", "عدمتایید", "عدمتائید", "رد" };
        bool? CheckInquiryValue(string value)
        {
            value = value.Replace(" ", string.Empty).CleanString();
            return ConfirmValues.Contains(value) ? true : (RejectValues.Contains(value) ? false : (bool?)null);
        }
    }
}
