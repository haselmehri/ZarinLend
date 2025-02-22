using Common.Utilities;
using Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Services.Model.IranCreditScoring
{
    public class IranCreditScoringResultRuleListModel
    {
        public int Id { get; set; }

        [Display(Name = "جامعه هدف")]
        public IranCreditScoringResultRuleType IranCreditScoringResultRuleType { get; set; }

        [Display(Name = "امتیاز")]
        public string Risk { get; set; }

        [Display(Name = "حداقل مبلغ قابل درخواست")]
        public long? MinimumAmount { get; set; }

        [Display(Name = "حداکثر مبلغ قابل درخواست")]
        public long? MaximumAmount { get; set; }

        [Display(Name = "نیاز به ضامن؟")]
        public bool GuarantorIsRequired { get; set; }

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
                return DateTimeHelper.GregorianToShamsi(CreateDate);
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
                    return DateTimeHelper.GregorianToShamsi(UpdateDate.Value);

                return string.Empty;
            }
        }

        [Display(Name = "ویرایش کننده")]
        public string Updater { get; set; }
    }
}
