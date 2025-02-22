using Common.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Services.Model.GlobalSetting
{
    public class GlobalSettingViewModel
    {
        public int Id { get; set; }

        [Display(Name = "هزینه اعتبار سنجی(ريال)")]
        public long ValidationFee { get; set; }

        [Display(Name = "مدت زمان اعتبار نتیجه اعتبارسنجی(روز)")]
        public int ValidityPeriodOfValidation { get; set; }

        [Display(Name = "مبلغ ضمانت نامه(درصد)")]
        public double WarantyPercentage { get; set; }

        [Display(Name = "سود تسهیلات(درصد)")]
        public double FacilityInterest { get; set; }

        [Display(Name = "کارمزد تسهیلات-زرین لند(درصد)")]
        public double LendTechFacilityFee { get; set; }

        [Display(Name = "کارمزد تسهیلات برای مشتریان زرین پال-زرین لند(درصد)")]
        public double LendTechFacilityForZarinpalClientFee { get; set; }

        [Display(Name = "کارمزد تسهیلات-نهاد مالی(درصد)")]
        public double FinancialInstitutionFacilityFee { get; set; }

        [Display(Name = "وضعیت")]
        public bool IsActive { get; set; }

        [Display(Name = "ایجاد کننده")]
        public string Creator { get; set; }

        [Display(Name = "تاریخ ایجاد")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "تاریخ ایجاد")]
        public string ShamsiCreateDate
        {
            get
            {
                return CreateDate.GregorianToShamsi();
            }
        }

        [Display(Name = "تاریخ ویرایش")]
        public DateTime? UpdateDate { get; set; }

        [Display(Name = "تاریخ ویرایش")]
        public string ShamsiUpdateDate
        {
            get
            {
                if (UpdateDate.HasValue && UpdateDate != default)
                    return UpdateDate.Value.GregorianToShamsi();

                return string.Empty;
            }
        }

        [Display(Name = "ویرایش کننده")]
        public string Updater { get; set; }
    }
}
