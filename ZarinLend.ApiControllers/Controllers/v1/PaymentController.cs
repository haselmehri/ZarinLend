using Asp.Versioning;
using Common;
using Common.Utilities;
using Core.Entities;
using Core.Entities.Business.Payment;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Services;
using Services.Dto;
using Services.Model;
using Services.Model.Payment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class PaymentController : BaseApiController
    {
        private readonly ISamanInternetPaymentService samanInternetPaymentService;
        private readonly IPaymentService paymentService;
        private readonly IPaymentInfoService paymentInfoService;

        public PaymentController(ISamanInternetPaymentService samanInternetPaymentService, IPaymentService paymentService)
        {
            this.samanInternetPaymentService = samanInternetPaymentService;
            this.paymentService = paymentService;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<PagingDto<IntenetBankPaymentModel>> GetPaymentList(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            return await paymentService.GetPaymentList(new Guid(User.Identity.GetUserId()), filter, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<PagingDto<SamanIntenetBankPaymentModel>> Search(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var payments = await samanInternetPaymentService.Search(filter, cancellationToken);

            return payments;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<string> Export(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            return await GenerateExcel((await samanInternetPaymentService.SearchForExport(filter, cancellationToken)).Data, cancellationToken);
        }

        private async Task<string> GenerateExcel(IEnumerable<SamanIntenetBankPaymentExportModel> items, CancellationToken cancellationToken = default)
        {
            if (items == null && items.Count() == 0)
                return null;

            var result = items.Select(p => new
            {
                p.Amount,
                FinancialInstitutionFacilityFee = p.PaymentType == PaymentType.PaySalesCommission ? p.FinancialInstitutionFacilityFee : 0,
                p.FinancialInstitutionFacilityAmount,
                LendTechFacilityFee = p.PaymentType == PaymentType.PaySalesCommission ? p.LendTechFacilityFee : 0,
                p.LendTechFacilityFeeAmount,
                p.PaymentTypeDesc,
                p.PayerNationalCode,
                p.Payer,
                IsSuccess = p.IsSuccess.HasValue ? (p.IsSuccess.Value ? "موفق" : "ناموفق") : "نامشخص",
                p.ResNum,
                p.Status,
                p.State,
                p.MaskedPan,
                p.RefNum,
                p.RRN,
                p.TraceNo,
                Date = p.ShamsiStraceDate.HasValue() ? p.ShamsiStraceDate
                                   : p.ShamsiUpdateDate.HasValue()
                                        ? p.ShamsiUpdateDate
                                        : p.ShamsiCreateDate,
                p.Id,
            });

            var memoryStream = new MemoryStream();
            using (var excelPackage = new ExcelPackage(memoryStream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("گزارش اقساط");
                worksheet.View.RightToLeft = true;
                worksheet.Cells["A1"].LoadFromCollection(result, true, OfficeOpenXml.Table.TableStyles.Light1);

                var idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "Amount").Start.Column;
                worksheet.Column(idx).Style.Numberformat.Format = "#,##0";
                worksheet.Column(idx).Width = 22;
                worksheet.SetValue(1, idx, "مبلغ تراکنش");

                idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "FinancialInstitutionFacilityAmount").Start.Column;
                worksheet.Column(idx).Style.Numberformat.Format = "#,##0";
                worksheet.Column(idx).Width = 22;
                worksheet.SetValue(1, idx, "سهم نهاد مالی");

                idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "LendTechFacilityFeeAmount").Start.Column;
                worksheet.Column(idx).Style.Numberformat.Format = "#,##0";
                worksheet.Column(idx).Width = 22;
                worksheet.SetValue(1, idx, "سهم زرین لند");

                idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "FinancialInstitutionFacilityFee").Start.Column;
                worksheet.SetValue(1, idx, "درصد نهاد مالی");

                idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "LendTechFacilityFee").Start.Column;
                worksheet.SetValue(1, idx, "درصد زرین لند");

                idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "PaymentTypeDesc").Start.Column;
                worksheet.SetValue(1, idx, "نوع پرداخت");

                idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "Payer").Start.Column;
                worksheet.SetValue(1, idx, "پرداخت کننده");

                idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "PayerNationalCode").Start.Column;
                worksheet.SetValue(1, idx, "کد ملی پرداخت کننده");

                idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "Date").Start.Column;
                worksheet.SetValue(1, idx, "تاریخ");

                var successColumnIndex = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "IsSuccess").Start.Column;
                worksheet.SetValue(1, successColumnIndex, "وضعیت پرداخت");

                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.MistyRose));

                worksheet.DefaultRowHeight = 20;
                worksheet.DefaultColWidth = 25;
                worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                var dataRowIndex = 2;
                while (worksheet.Dimension.End.Row >= dataRowIndex)
                {
                    worksheet.Cells[dataRowIndex, successColumnIndex].Style.Font.Bold = true;
                    if (worksheet.Cells[dataRowIndex, successColumnIndex].Value.ToString() == "موفق")
                    {
                        worksheet.Cells[dataRowIndex, successColumnIndex].Style.Font.Color.SetColor(System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.DarkOliveGreen));
                    }
                    else if (worksheet.Cells[dataRowIndex, successColumnIndex].Value.ToString() == "نامشخص")
                    {
                        worksheet.Cells[dataRowIndex, successColumnIndex].Style.Font.Color.SetColor(System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Gray));
                    }
                    else
                    {
                        worksheet.Cells[dataRowIndex, successColumnIndex].Style.Font.Color.SetColor(System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Red));
                    }
                    dataRowIndex++;
                }
                //Convert File to Base64 string and send to Client.
                var bytes = await excelPackage.GetAsByteArrayAsync(cancellationToken);
                return Convert.ToBase64String(bytes, 0, bytes.Length);
                //return File(excelPackage.GetAsByteArray(), "application/octet-stream", "GetAllReservation.xlsx");
            }
        }
    }
}
