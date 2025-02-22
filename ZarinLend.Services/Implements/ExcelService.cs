using Common;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class ExcelService : IExcelService, ISingletonDependency
    {
        public ExcelService()
        {
            // If you are a commercial business and have
            // purchased commercial licenses use the static property
            // LicenseContext of the ExcelPackage class:
            //ExcelPackage.LicenseContext = LicenseContext.Commercial;

            // If you use EPPlus in a noncommercial context
            // according to the Polyform Noncommercial license:
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<MemoryStream> GenerateRequestFacilityDetail(RequestFacilityInfoModel requestFacilityInfo, SamatFacilityHeaderModel samatFacility,
            SamatBackChequeHeaderModel samatBackCheque, CancellationToken cancellationToken)
        {
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                #region جزئیات درخواست تسهیلات
                var workSheet_request = package.Workbook.Worksheets.Add("جزئیات درخواست تسهیلات");
                HeaderRowSetting(workSheet_request.Row(1));
                //workSheet_request.Row(1).Height = 30;
                //workSheet_request.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //workSheet_request.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //workSheet_request.Row(1).Style.Fill.SetBackground(Color.BlueViolet);

                CellSetting(workSheet_request.Cells);

                workSheet_request.Cells[1, 1].Value = "اعتبار درخواستی(ريال)";
                workSheet_request.Column(1).Width = 23;
                workSheet_request.Cells[1, 2].Value = "بازه پرداخت";
                workSheet_request.Column(2).Width = 15;
                workSheet_request.Cells[1, 3].Value = "تاریخ درخواست";
                workSheet_request.Column(3).Width = 20;

                workSheet_request.Cells[2, 1].Value = requestFacilityInfo.RequestFacilityDetail.Amount;
                workSheet_request.Cells[2, 1].Style.Numberformat.Format = "#,##0";

                workSheet_request.Cells[2, 2].Value = requestFacilityInfo.RequestFacilityDetail.MonthCountTitle;

                workSheet_request.Cells[2, 3].Value = requestFacilityInfo.RequestFacilityDetail.ShamsiCreateDate;

                #endregion

                #region اطلاعات هویتی

                var workSheet_userIdentity = package.Workbook.Worksheets.Add("اطلاعات هویتی درخواست کننده");
                HeaderRowSetting(workSheet_userIdentity.Row(1));
                CellSetting(workSheet_userIdentity.Cells);

                workSheet_userIdentity.Cells[1, 1].Value = "نام";
                workSheet_userIdentity.Column(1).Width = 20;
                workSheet_userIdentity.Cells[1, 2].Value = "نام خانوادگی";
                workSheet_userIdentity.Column(2).Width = 20;
                workSheet_userIdentity.Cells[1, 3].Value = "کد ملی";
                workSheet_userIdentity.Column(3).Width = 13;
                workSheet_userIdentity.Cells[1, 4].Value = "شماره موبایل";
                workSheet_userIdentity.Column(4).Width = 13;
                workSheet_userIdentity.Cells[1, 5].Value = "شماره ثابت";
                workSheet_userIdentity.Column(5).Width = 14;
                workSheet_userIdentity.Cells[1, 6].Value = "استان محل اقامت";
                workSheet_userIdentity.Column(6).Width = 22;
                workSheet_userIdentity.Cells[1, 7].Value = "استان محل اقامت";
                workSheet_userIdentity.Column(7).Width = 22;
                workSheet_userIdentity.Cells[1, 8].Value = "کد پستی";
                workSheet_userIdentity.Column(8).Width = 13;
                workSheet_userIdentity.Cells[1, 9].Value = "ایمیل";
                workSheet_userIdentity.Column(9).Width = 30;
                workSheet_userIdentity.Cells[1, 10].Value = "آدرس";
                workSheet_userIdentity.Column(10).Width = 40;

                workSheet_userIdentity.Cells[2, 1].Value = requestFacilityInfo.UserIdentityInfo.FName;
                workSheet_userIdentity.Cells[2, 2].Value = requestFacilityInfo.UserIdentityInfo.LName;
                workSheet_userIdentity.Cells[2, 3].Value = requestFacilityInfo.UserIdentityInfo.NationalCode;
                workSheet_userIdentity.Cells[2, 4].Value = requestFacilityInfo.UserIdentityInfo.Mobile;
                workSheet_userIdentity.Cells[2, 5].Value = requestFacilityInfo.UserIdentityInfo.PhoneNumber;
                workSheet_userIdentity.Cells[2, 6].Value = requestFacilityInfo.UserIdentityInfo.AddressProvinceName;
                workSheet_userIdentity.Cells[2, 7].Value = requestFacilityInfo.UserIdentityInfo.AddressCityName;
                workSheet_userIdentity.Cells[2, 8].Value = requestFacilityInfo.UserIdentityInfo.PostalCode;
                workSheet_userIdentity.Cells[2, 9].Value = requestFacilityInfo.UserIdentityInfo.Email;
                workSheet_userIdentity.Cells[2, 10].Value = requestFacilityInfo.UserIdentityInfo.Address;
                workSheet_userIdentity.Cells[2, 10].Style.WrapText = true;

                #endregion

                #region اطلاعات تسهیلات

                var workSheet_userActiveFacility = package.Workbook.Worksheets.Add("تسهیلات فعال(سمات)");
                CellSetting(workSheet_userActiveFacility.Cells);
                if (samatFacility != null)
                {
                    workSheet_userActiveFacility.Cells[1, 2].Value = "مبلغ کل تسهیلات(ريال)";
                    workSheet_userActiveFacility.Column(2).Width = 25;
                    workSheet_userActiveFacility.Cells[1, 2].Style.Fill.SetBackground(Color.BlueViolet);
                    workSheet_userActiveFacility.Cells[1, 3].Value = samatFacility.FacilityTotalAmount.ToString("N0");
                    workSheet_userActiveFacility.Column(3).Width = 20;

                    workSheet_userActiveFacility.Cells[1, 4].Value = "مبلغ کل بدهی تسهیلات(ريال)";
                    workSheet_userActiveFacility.Column(4).Width = 25;
                    workSheet_userActiveFacility.Cells[1, 4].Style.Fill.SetBackground(Color.BlueViolet);
                    workSheet_userActiveFacility.Cells[1, 5].Value = samatFacility.FacilityDebtTotalAmount.ToString("N0");
                    workSheet_userActiveFacility.Column(5).Width = 28;

                    workSheet_userActiveFacility.Cells[1, 6].Value = "مبلغ کل بدهی معوق(ريال)";
                    workSheet_userActiveFacility.Column(6).Width = 25;
                    workSheet_userActiveFacility.Cells[1, 6].Style.Fill.SetBackground(Color.BlueViolet);
                    workSheet_userActiveFacility.Cells[1, 7].Value = samatFacility.FacilityDeferredTotalAmount.ToString("N0");
                    workSheet_userActiveFacility.Column(7).Width = 20;

                    workSheet_userActiveFacility.Cells[1, 8].Value = "تاریخ بروز رسانی";
                    workSheet_userActiveFacility.Column(8).Width = 20;
                    workSheet_userActiveFacility.Cells[1, 8].Style.Fill.SetBackground(Color.BlueViolet);
                    workSheet_userActiveFacility.Cells[1, 9].Value = samatFacility.ShamsiCreatedDate;
                    workSheet_userActiveFacility.Column(9).Width = 15;

                    #region Grid Header
                    HeaderRowSetting(workSheet_userActiveFacility.Row(3));
                    workSheet_userActiveFacility.Cells[3, 1].Value = "ردیف";
                    workSheet_userActiveFacility.Column(1).Width = 10;

                    workSheet_userActiveFacility.Cells[3, 2].Value = "اصل مبلغ تسهیلات(ريال)";
                    workSheet_userActiveFacility.Cells[3, 3].Value = "سود مبلغ تسهیلات(ريال)";
                    workSheet_userActiveFacility.Cells[3, 4].Value = "مبلغ باقیمانده از تسهیلات(ريال)";
                    workSheet_userActiveFacility.Cells[3, 5].Value = "مجموع بدهی معوق از تسهیلات(ريال)";
                    workSheet_userActiveFacility.Cells[3, 6].Value = "بانک-شعبه تسهیلات";
                    workSheet_userActiveFacility.Cells[3, 7].Value = "تاریخ شروع";
                    workSheet_userActiveFacility.Cells[3, 8].Value = "تاریخ پایان";
                    #endregion

                    if (samatFacility.FacilityList != null && samatFacility.FacilityList.Any())
                    {
                        for (int i = 0; i < samatFacility.FacilityList.Count; i++)
                        {
                            workSheet_userActiveFacility.Cells[i + 4, 1].Value = i + 1;
                            workSheet_userActiveFacility.Cells[i + 4, 2].Value = samatFacility.FacilityList[i].FacilityAmountOrginal.ToString("N0");
                            workSheet_userActiveFacility.Cells[i + 4, 3].Value = samatFacility.FacilityList[i].FacilityBenefitAmount.ToString("N0");
                            workSheet_userActiveFacility.Cells[i + 4, 4].Value = samatFacility.FacilityList[i].FacilityDebtorTotalAmount.ToString("N0");
                            workSheet_userActiveFacility.Cells[i + 4, 5].Value = samatFacility.FacilityList[i].FacilityDeferredAmount.ToString("N0");
                            workSheet_userActiveFacility.Cells[i + 4, 6].Value = samatFacility.FacilityList[i].BranchDescription;
                            workSheet_userActiveFacility.Cells[i + 4, 7].Value =
                                $"{samatFacility.FacilityList[i].FacilitySetDate.Substring(0, 4)}/{samatFacility.FacilityList[i].FacilitySetDate.Substring(4, 2)}/{samatFacility.FacilityList[i].FacilitySetDate.Substring(6)}";
                            workSheet_userActiveFacility.Cells[i + 4, 8].Value =
                                $"{samatFacility.FacilityList[i].FacilityEndDate.Substring(0, 4)}/{samatFacility.FacilityList[i].FacilityEndDate.Substring(4, 2)}/{samatFacility.FacilityList[i].FacilityEndDate.Substring(6)}";
                        }
                    }
                    else
                    {
                        workSheet_userActiveFacility.Cells[4, 1, 4, 8].Merge = true;
                        workSheet_userActiveFacility.Cells[4, 1, 4, 8].Value = "هیچ تسهیلات یا تعهدی یافت نشد";
                    }
                }
                else
                {
                    workSheet_userActiveFacility.Cells[1, 1, 1, 8].Merge = true;
                    workSheet_userActiveFacility.Cells[1, 1, 1, 8].Value = "هیچ تسهیلات یا تعهدی یافت نشد";
                }
                #endregion

                #region اطلاعات چک های برگشتی

                var workSheet_backCheques = package.Workbook.Worksheets.Add("اطلاعات چک های برگشتی");
                CellSetting(workSheet_backCheques.Cells);
                if (samatFacility != null)
                {
                    workSheet_backCheques.Cells[1, 2].Value = "تاریخ بروز رسانی";
                    workSheet_backCheques.Column(2).Width = 20;
                    workSheet_backCheques.Cells[1, 2].Style.Fill.SetBackground(Color.BlueViolet);
                    workSheet_backCheques.Cells[1, 3].Value = samatBackCheque.ShamsiCreatedDate;
                    workSheet_backCheques.Column(3).Width = 20;

                    #region Grid Header
                    HeaderRowSetting(workSheet_backCheques.Row(3));
                    workSheet_backCheques.Cells[3, 1].Value = "ردیف";
                    workSheet_backCheques.Column(1).Width = 10;

                    workSheet_backCheques.Cells[3, 2].Value = "تاریخ برگشت چک";
                    workSheet_backCheques.Cells[3, 3].Value = "مبلغ چک(ريال)";
                    workSheet_backCheques.Cells[3, 4].Value = "شماره چک";
                    workSheet_backCheques.Column(3).Width = 20;
                    workSheet_backCheques.Cells[3, 5].Value = "شماره شبا";
                    workSheet_backCheques.Column(5).Width = 20;
                    workSheet_backCheques.Cells[3, 6].Value = "بانک-شعبه";
                    workSheet_backCheques.Column(6).Width = 25;
                    workSheet_backCheques.Cells[3, 7].Value = "تاریخ چک";
                    workSheet_backCheques.Column(7).Width = 15;
                    #endregion

                    if (samatBackCheque.BackChequeList != null && samatBackCheque.BackChequeList.Any())
                    {
                        for (int i = 0; i < samatBackCheque.BackChequeList.Count; i++)
                        {
                            workSheet_backCheques.Cells[i + 4, 1].Value = i + 1;
                            workSheet_backCheques.Cells[i + 4, 2].Value =
                                $"{samatBackCheque.BackChequeList[i].BackDate.Substring(0, 4)}/{samatBackCheque.BackChequeList[i].BackDate.Substring(4, 2)}/{samatBackCheque.BackChequeList[i].BackDate.Substring(6)}";
                            workSheet_backCheques.Cells[i + 4, 3].Value = samatBackCheque.BackChequeList[i].Amount.ToString("N0");
                            workSheet_backCheques.Cells[i + 4, 4].Value = samatBackCheque.BackChequeList[i].Number;
                            workSheet_backCheques.Cells[i + 4, 5].Value = $"IR{samatBackCheque.BackChequeList[i].AccountNumber}";
                            workSheet_backCheques.Cells[i + 4, 6].Value = samatBackCheque.BackChequeList[i].BranchDescription;
                            workSheet_backCheques.Cells[i + 4, 7].Value =
                                $"{samatBackCheque.BackChequeList[i].Date.Substring(0, 4)}/{samatBackCheque.BackChequeList[i].Date.Substring(4, 2)}/{samatBackCheque.BackChequeList[i].Date.Substring(6)}";
                        }
                    }
                    else
                    {
                        workSheet_backCheques.Cells[4, 1, 4, 7].Merge = true;
                        workSheet_backCheques.Cells[4, 1, 4, 7].Value = "هیچ چک برگشتی یافت نشد";
                    }
                }
                else
                {
                    workSheet_backCheques.Cells[1, 1, 1, 7].Merge = true;
                    workSheet_backCheques.Cells[1, 1, 1, 7].Value = "هیچ چک برگشتی یافت نشد";
                }
                #endregion

                await package.SaveAsync(cancellationToken);
            }
            stream.Position = 0;

            return stream;
        }

        private void HeaderRowSetting(ExcelRow row)
        {
            row.Height = 25;
            row.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            row.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            row.Style.Fill.SetBackground(Color.BlueViolet);
        }

        private void CellSetting(ExcelRange cells)
        {
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.AutoFitColumns();
        }
    }
}
