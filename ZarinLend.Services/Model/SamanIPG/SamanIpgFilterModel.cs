using Common.Utilities;
using Core.Entities.Business.Payment;
using System;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class SamanIpgFilterModel
    {
        [Display(Name = "نام")]
        [RegularExpression(RegularExpression.PersianCharacter,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string FName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [RegularExpression(RegularExpression.PersianCharacter,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string LName { get; set; }

        [StringLength(10)]
        [Display(Name = "NationalCode", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.NationalCode,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string NationalCode { get; set; }

        [Display(Name = "تاریخ درخواست از")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "تاریخ درخواست تا")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "وضعیت")]
        public bool? IsSuccess { get; set; }

        [Display(Name = "شناسه خرید")]
        public string ResNum { get; set; }

        [Display(Name = "نوع پرداخت")]
        public PaymentType? PaymentType { get; set; }
    }
}
