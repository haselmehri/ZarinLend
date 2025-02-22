using Common.Exceptions;
using Common;
using Common.Utilities;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Services;
using Services.Model;
using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;
using static System.Net.WebRequestMethods;
using System.Linq;
using Common.CustomFileAttribute;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class ApplicantValidationResultController : BaseApiController
    {
        private readonly ILogger<ApplicantValidationResultController> logger;
        private readonly IApplicantValidationResultService applicantValidationResultService;
        private readonly IRequestFacilityService requestFacilityService;

        public ApplicantValidationResultController(ILogger<ApplicantValidationResultController> logger,
            IApplicantValidationResultService applicantValidationResultService,
            IRequestFacilityService requestFacilityService)
        {
            this.logger = logger;
            this.applicantValidationResultService = applicantValidationResultService;
            this.requestFacilityService = requestFacilityService;
        }

        [HttpPost("[action]/{requestFacilityId:int}/{organizationId:int}")]
        [CustomAuthorize(RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [RequestSizeLimit(bytes: 1 * 1024 * 100/*Max File Size : 100KB*/)]
        public virtual async Task<ApplicantValidationResultModel> UploadVerifyOctectResult([MaxFileSize(1 * 1024 * 100/*Max File Size : 100KB*/),
            AllowedExtensions(new string[] {".xls",".xlsx"},errorMessage:"فقط فایل هایی از نوع اکسل با پسوند (xls,xlsx) می توانید بارگذاری کنید!" )] IFormFile verifyOctectResultFile, int requestFacilityId, int organizationId, CancellationToken cancellationToken)
        {
            if (User.Identity.GetUserLeasingId() != organizationId)
                throw new AppException(ApiResultStatusCode.UnAuthorized, "خطای دسترسی!");
            if (verifyOctectResultFile == null)
                throw new AppException(ApiResultStatusCode.LogicError, "لطفا فایل نتیجه اعتبارسنجی را جهت بارگذاری،انتخاب کنید!");

            if (verifyOctectResultFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    verifyOctectResultFile.CopyTo(ms);
                    var personInfo = await requestFacilityService.GetPersonInfo(requestFacilityId, cancellationToken);
                    var extractVerifyResult = ExtractResultFromExcel(ms);
                    if (personInfo != null && extractVerifyResult != null)
                    {
                        if (personInfo.NationalCode != extractVerifyResult.NationalCode)
                            throw new AppException(ApiResultStatusCode.LogicError,
                                $"کد ملی موجود در فایل اکسل با کد ملی درخواست دهنده تسهیلات مطابقت ندارد!<br/>کد ملی درخواست دهنده " +
                                $": {personInfo.NationalCode}<br/>کد ملی در فایل اکسل : {extractVerifyResult.NationalCode}");

                        return extractVerifyResult;
                    }
                }
            }

            return null;
        }

        private ApplicantValidationResultModel ExtractResultFromExcel(Stream stream)
        {
            using (var excelPackage = new ExcelPackage(stream))
            {
                ApplicantValidationResultModel result = null;
                if (excelPackage.Workbook.Worksheets.Count > 0)
                {
                    result = new ApplicantValidationResultModel();
                    var workSheet = excelPackage.Workbook.Worksheets[0];
                    var nationalCodeColumn = workSheet.Cells["1:1"].FirstOrDefault(c => c.Value != null && c.Value.ToString().CleanString().Replace(" ", string.Empty) == "کدملی");
                    if (nationalCodeColumn == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, "ستون 'کد ملی' در فایل اکسل یافت نشد");

                    result.NationalCode = workSheet.Cells[2, nationalCodeColumn.Start.Column].Value.ToString();

                    var civilRegistryInquiryColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استعلام ثبت احوال".CleanString().Replace(" ", string.Empty));
                    if (civilRegistryInquiryColumn != null)
                    {
                        var value = Convert.ToString(workSheet.Cells[2, civilRegistryInquiryColumn.Start.Column].Value).Replace(" ", string.Empty).CleanString();
                        result.CivilRegistryInquiry = value == "بله" ? true : (value == "خیر" ? false : (bool?)null);
                    }

                    var returnedCheckInquiryColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استعلام چک برگشتی".CleanString().Replace(" ", string.Empty));
                    if (returnedCheckInquiryColumn != null)
                    {
                        var value = Convert.ToString(workSheet.Cells[2, returnedCheckInquiryColumn.Start.Column].Value).Replace(" ", string.Empty).CleanString();
                        result.ReturnedCheckInquiry = value == "بله" ? true : (value == "خیر" ? false : (bool?)null);
                    }

                    var facilityInquiryColumn = workSheet.Cells["1:1"]
                    .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استعلام تسهیلات و تعهدات".CleanString().Replace(" ", string.Empty));
                    if (facilityInquiryColumn != null)
                    {
                        var value = Convert.ToString(workSheet.Cells[2, facilityInquiryColumn.Start.Column].Value).Replace(" ", string.Empty).CleanString();
                        result.FacilityInquiry = value == "بله" ? true : (value == "خیر" ? false : (bool?)null);
                    }

                    var postalCodeInquiryColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استعلام کدپستی به آدرس".CleanString().Replace(" ", string.Empty));
                    if (postalCodeInquiryColumn != null)
                    {
                        var value = Convert.ToString(workSheet.Cells[2, postalCodeInquiryColumn.Start.Column].Value).Replace(" ", string.Empty).CleanString();
                        result.PostalCodeInquiry = value == "بله" ? true : (value == "خیر" ? false : (bool?)null);
                    }

                    var securityCouncilSanctionsInquiryColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استعلام تحریم شورا امنیت".CleanString().Replace(" ", string.Empty));
                    if (securityCouncilSanctionsInquiryColumn != null)
                    {
                        var value = Convert.ToString(workSheet.Cells[2, securityCouncilSanctionsInquiryColumn.Start.Column].Value).Replace(" ", string.Empty).CleanString();
                        result.SecurityCouncilSanctionsInquiry = value == "بله" ? true : (value == "خیر" ? false : (bool?)null);
                    }
                    var shahkarInquiryColumn = workSheet.Cells["1:1"]
                        .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استعلام شاهکار".CleanString().Replace(" ", string.Empty));
                    if (shahkarInquiryColumn != null)
                    {
                        var value = Convert.ToString(workSheet.Cells[2, shahkarInquiryColumn.Start.Column].Value).Replace(" ", string.Empty).CleanString();
                        result.ShahkarInquiry = value == "بله" ? true : (value == "خیر" ? false : (bool?)null);
                    }

                    var militaryInquiryColumn = workSheet.Cells["1:1"]
                       .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استعلام نظام وظیفه".CleanString().Replace(" ", string.Empty));
                    if (militaryInquiryColumn != null)
                    {
                        var value = Convert.ToString(workSheet.Cells[2, militaryInquiryColumn.Start.Column].Value).Replace(" ", string.Empty).CleanString();
                        result.MilitaryInquiry = value == "بله" ? true : (value == "خیر" ? false : (bool?)null);
                    }

                    var blackListInquiryColumn = workSheet.Cells["1:1"]
                      .FirstOrDefault(c => c.Value != null && c.Value.ToString().Replace(" ", string.Empty).CleanString() == "استعلام لیست سیاه".CleanString().Replace(" ", string.Empty));
                    if (blackListInquiryColumn != null)
                    {
                        var value = Convert.ToString(workSheet.Cells[2, blackListInquiryColumn.Start.Column].Value).Replace(" ", string.Empty).CleanString();
                        result.BlackListInquiry = value == "بله" ? true : (value == "خیر" ? false : (bool?)null);
                    }
                }
                return result;
            }
        }
    }
}
