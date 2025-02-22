using Asp.Versioning;
using Common;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Services;
using Services.Dto;
using Services.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

public class DataTableParameter
{
    public int draw { get; set; }
    public int length { get; set; }
    public int start { get; set; }
}

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class RequestFacilityInstallmentController : BaseApiController
    {

        private readonly ILogger<RequestFacilityInstallmentController> logger;
        private readonly IRequestFacilityInstallmentService requestFacilityInstallmentService;

        public RequestFacilityInstallmentController(ILogger<RequestFacilityInstallmentController> logger, IRequestFacilityInstallmentService requestFacilityInstallmentService)
        {
            this.logger = logger;
            this.requestFacilityInstallmentService = requestFacilityInstallmentService;
        }

        [HttpPost("[action]/{requestFacilityId:int}")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> SelectInstallment(int requestFacilityId, DataTableParameter data, CancellationToken cancellationToken)
        {
            var installmentList = await requestFacilityInstallmentService.SelectInstallment(new Guid(User.Identity!.GetUserId()), requestFacilityId, cancellationToken);
            return new ApiResult<List<RequestFacilityInstallmentModel>>(true, ApiResultStatusCode.Success, installmentList);
        }

        [HttpPost("[action]/{id:int}")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> CheckExistUnpaidInstallmentBeforeThis(int id, CancellationToken cancellationToken)
        {
            return new ApiResult<RequestFacilityInstallmentModel>(true, ApiResultStatusCode.Success,
                 await requestFacilityInstallmentService.CheckExistUnpaidInstallmentBeforeThis(new Guid(User.Identity!.GetUserId()), id, cancellationToken));
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<PagingDto<RequestFacilityInstallmentModel>> Search(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            return await requestFacilityInstallmentService.Search(filter, false, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<string> ExportExcel(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            return await GenerateExcel((await requestFacilityInstallmentService.Search(filter, true, cancellationToken)).Data, cancellationToken);
        }

        private async Task<string> GenerateExcel(IEnumerable<RequestFacilityInstallmentModel> requestList, CancellationToken cancellationToken = default)
        {
            if (requestList == null && requestList.Count() == 0)
                return null;

            var result = requestList.Select(p => new
            {
                p.Amount,
                p.ShamsiDueDate,
                p.PenaltyDays,
                p.PenaltyAmount,
                p.RealPayAmount,
                p.ShamsiRealPayDate,
                Paid = p.Paid ? "پرداخت شده" : "پرداخت نشده",
                p.FacilityAmount,
                p.MonthCountTitle,
                p.Requester,
                p.NationalCode
            });

            var memoryStream = new MemoryStream();
            using (var excelPackage = new ExcelPackage(memoryStream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("گزارش اقساط");
                worksheet.View.RightToLeft = true;
                worksheet.Cells["A1"].LoadFromCollection(result, true, OfficeOpenXml.Table.TableStyles.Light1);
                //worksheet.Cells["A1"].LoadFromDataTable(profileService.GetAllReservation(), true, OfficeOpenXml.Table.TableStyles.Light1);

                var idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "Amount").Start.Column;
                worksheet.Column(idx).Style.Numberformat.Format = "#,##0";
                worksheet.Column(idx).Width = 22;

                idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "FacilityAmount").Start.Column;
                worksheet.Column(idx).Style.Numberformat.Format = "#,##0";
                worksheet.Column(idx).Width = 22;

                idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "PenaltyDays").Start.Column;
                worksheet.Column(idx).Style.Numberformat.Format = "#,##0";
                worksheet.Column(idx).Width = 22;

                idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "PenaltyAmount").Start.Column;
                worksheet.Column(idx).Style.Numberformat.Format = "#,##0";
                worksheet.Column(idx).Width = 22;

                idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "RealPayAmount").Start.Column;
                worksheet.Column(idx).Style.Numberformat.Format = "#,##0";
                worksheet.Column(idx).Width = 35;

                var paidColumnIndex= worksheet.Cells["1:1"].First(c => c.Value.ToString() == "Paid").Start.Column;

                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.MistyRose));

                worksheet.DefaultRowHeight = 20;
                worksheet.DefaultColWidth = 22;
                worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.SetValue(1, 1, "مبلغ قسط");
                worksheet.SetValue(1, 2, "تاریخ سررسید");
                worksheet.SetValue(1, 3, "تعداد روزهای دیرکرد");
                worksheet.SetValue(1, 4, "مبلغ دیرکرد/جریمه");
                worksheet.SetValue(1, 5, "مبلغ کل(قسط + دیرکرد/جریمه)");
                worksheet.SetValue(1, 6, "تاریخ پرداخت/تسویه");
                worksheet.SetValue(1, 7, "وضعیت");
                worksheet.SetValue(1, 8, "مبلغ تسهیلات/وام");
                worksheet.SetValue(1, 9, "بازه پرداخت");
                worksheet.SetValue(1, 10, "نام کامل وام گیرنده");
                worksheet.SetValue(1, 11, "کد ملی");

                var dataRowIndex = 2;
                while (worksheet.Dimension.End.Row >= dataRowIndex)
                {
                    worksheet.Cells[dataRowIndex, paidColumnIndex].Style.Font.Bold = true;
                    if (worksheet.Cells[dataRowIndex, paidColumnIndex].Value.ToString() == "پرداخت شده")
                    {
                        worksheet.Cells[dataRowIndex, paidColumnIndex].Style.Font.Color.SetColor(System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.DarkOliveGreen));
                    }
                    else
                    {
                        worksheet.Cells[dataRowIndex, paidColumnIndex].Style.Font.Color.SetColor(System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Red));
                    }
                    dataRowIndex++;
                }
                    //Convert File to Base64 string and send to Client.
                    var bytes = await excelPackage.GetAsByteArrayAsync(cancellationToken);
                string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);
                return base64;
                //return File(excelPackage.GetAsByteArray(), "application/octet-stream", "GetAllReservation.xlsx");
            }
        }
    }
}
