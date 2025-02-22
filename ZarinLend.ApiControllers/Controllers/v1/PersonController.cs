using Asp.Versioning;
using Common;
using Common.Utilities;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class PersonController : BaseApiController
    {
        private readonly ILogger<PersonController> logger;
        private readonly IPersonService personService;

        public PersonController(ILogger<PersonController> logger, IPersonService personService)
        {
            this.logger = logger;
            this.personService = personService;
        }

        [HttpPost("[action]/{id:int}")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<string> DownloadVerifyOctectExcel(int id, CancellationToken cancellationToken)
        {
            var personInfo = await personService.GetPersonInfo(id, cancellationToken);
            return await GenerateVerifyOctectExcel(personInfo, cancellationToken);
        }


        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> UpdateJobInfo([FromForm] PersonJobEditModel model, CancellationToken cancellationToken)
        {
            model.UserId = new Guid(User.Identity!.GetUserId());
            return await personService.UpdateJobInfo(model, cancellationToken);
        }

        [HttpPost("[action]/{userId:guid}")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> UpdateJobInfo(Guid userId, [FromForm] PersonJobEditModel model, CancellationToken cancellationToken)
        {
            model.UserId = userId;
            return await personService.UpdateJobInfo(model, cancellationToken);
        }

        private async Task<string> GenerateVerifyOctectExcel(PersonCompleteInfoModel personInfo, CancellationToken cancellationToken = default)
        {
            if (personInfo == null)
                return null;

            DataTable dt = new DataTable();
            dt.Columns.Add("ردیف");
            dt.Columns.Add("کد ملی");
            dt.Columns.Add("شماره شناسنامه");
            dt.Columns.Add("تاریخ تولد");
            dt.Columns.Add("نوع شخص");
            dt.Columns.Add("کد پستی");
            dt.Columns.Add("نام");
            dt.Columns.Add("نام خانوادگی");
            dt.Columns.Add("جنسیت");
            dt.Columns.Add("نام پدر");
            dt.Columns.Add("نام شرکت");
            dt.Columns.Add("تاریخ ثبت");
            dt.Columns.Add("شماره ثبت");
            dt.Columns.Add("ملیت");
            dt.Columns.Add("آدرس");
            dt.Columns.Add("شماره موبایل");
            //dt.Columns.Add("محل تولد");
            dt.Columns.Add("استعلام ثبت احوال");
            dt.Columns.Add("استعلام چک برگشتی");
            dt.Columns.Add("استعلام تسهیلات و تعهدات");
            dt.Columns.Add("استعلام کدپستی به آدرس");
            dt.Columns.Add("استعلام تحریم شورا امنیت");
            dt.Columns.Add("استعلام شاهکار");
            dt.Columns.Add("استعلام نظام وظیفه");
            dt.Columns.Add("استعلام لیست سیاه");
            var row = dt.NewRow();
            row["ردیف"] = 1;
            row["کد ملی"] = personInfo.NationalCode;
            row["شماره شناسنامه"] = personInfo.SSID;
            row["تاریخ تولد"] = DateTimeHelper.GregorianToShamsi(personInfo.BirthDate, string.Empty);
            row["نوع شخص"] = "حقیقی";
            row["کد پستی"] = personInfo.PostalCode;
            row["نام"] = personInfo.FName;
            row["نام خانوادگی"] = personInfo.LName;
            row["جنسیت"] = personInfo.Gender == GenderEnum.Male ? "مرد" : "زن";
            row["نام پدر"] = personInfo.FatherName;
            row["نام شرکت"] = string.Empty;
            row["تاریخ ثبت"] =  string.Empty;
            row["شماره ثبت"] = string.Empty;
            row["ملیت"] = personInfo.Nationality;
            row["آدرس"] = personInfo.Address;
            row["شماره موبایل"] = personInfo.Mobile;
            //row["محل تولد"] = $"{personInfo.ProvinceOfBirth}-{personInfo.CityOfBirth}";
            row["استعلام ثبت احوال"] = string.Empty;
            row["استعلام چک برگشتی"] = string.Empty;
            row["استعلام تسهیلات و تعهدات"] = string.Empty;
            row["استعلام کدپستی به آدرس"] = string.Empty;
            row["استعلام تحریم شورا امنیت"] = string.Empty;
            row["استعلام شاهکار"] = string.Empty;
            row["استعلام نظام وظیفه"] = string.Empty;
            row["استعلام لیست سیاه"] = string.Empty;
            dt.Rows.Add(row);

            var memoryStream = new MemoryStream();
            using (var excelPackage = new ExcelPackage(memoryStream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("اطلاعات هویتی درخواست دهنده");
                worksheet.View.RightToLeft = true;
                worksheet.Cells["A1"].LoadFromDataTable(dt, true, OfficeOpenXml.Table.TableStyles.Light1);
                //worksheet.Cells["A1"].LoadFromDataTable(profileService.GetAllReservation(), true, OfficeOpenXml.Table.TableStyles.Light1);

                //var idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "UpdateDate").Start.Column;
                worksheet.Cells[2,1].Style.Numberformat.Format = "Text";
                worksheet.Cells[2, 1].Style.Numberformat.Format = "@";
                //worksheet.Column(idx).Width = 15;

                //idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "CreateDate").Start.Column;
                //worksheet.Column(idx).Style.Numberformat.Format = "yyyy-mm-dd";
                //worksheet.Column(idx).Width = 15;

                //idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "Amount").Start.Column;
                //worksheet.Column(idx).Style.Numberformat.Format = "#,##0";

                //idx = worksheet.Cells["1:1"].First(c => c.Value.ToString() == "Wage").Start.Column;
                //worksheet.Column(idx).Style.Numberformat.Format = "#,##0";

                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Row(1).Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.MistyRose));

                worksheet.DefaultRowHeight = 20;
                worksheet.DefaultColWidth = 22;
                worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                worksheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                //worksheet.Columns[14].Style.Numberformat.Format = "#,##0";
                //worksheet.Columns[15].Style.Numberformat.Format = "#,##0";

                //worksheet.SetValue(1, 1, "کد عضویت");
                //worksheet.SetValue(1, 2, "شماره کارت(Pan)");
                //worksheet.SetValue(1, 3, "نام خانوادگی");
                //worksheet.SetValue(1, 4, "کد ملی");
                //worksheet.SetValue(1, 5, "مبلغ وام");
                //worksheet.SetValue(1, 6, "نرخ وام");
                //worksheet.SetValue(1, 7, "تاریخ تایید");
                //worksheet.SetValue(1, 8, "نوع وام");
                //worksheet.SetValue(1, 9, "معرف");
                //worksheet.SetValue(1, 10, "پرداخت از محل");
                //worksheet.SetValue(1, 11, "تاریخ تخصیص");
                //worksheet.SetValue(1, 12, "تاریخ درخواست");
                //worksheet.SetValue(1, 13, "تاریخ معرفی به بانک");
                //worksheet.SetValue(1, 14, "مبلغ تراکنش");
                //worksheet.SetValue(1, 15, "موجودی");
                //worksheet.SetValue(1, 16, "نوع تراکنش");
                //worksheet.SetValue(1, 17, "Transaction Type");

                //Convert File to Base64 string and send to Client.
                var bytes = await excelPackage.GetAsByteArrayAsync();
                string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);
                return base64;
                //return File(excelPackage.GetAsByteArray(), "application/octet-stream", "GetAllReservation.xlsx");
            }
        }
    }
}
