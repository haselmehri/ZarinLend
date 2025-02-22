using Common.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    public enum PaidStatus
    {
        /// <summary>
        /// پرداخت نشده و بدون جریمه
        /// </summary>
        NotPayAndNotPenalty = 1,
        /// <summary>
        /// پرداخت شده با جریمه
        /// </summary>
        PaidAndPenalty = 2,
        /// <summary>
        /// پرداخت نشده و با جریمه
        /// </summary>
        NotPayAndPenalty = 3,
        /// <summary>
        /// پرداخت شده بدون جریمه
        /// </summary>
        PaidAndNotPenalty = 4,
        /// <summary>
        /// هیچکدام
        /// </summary>
        None
    }

    [Serializable]
    public class RequestFacilityInstallmentFilterModel
    {
        [Display(Name = "شناسه درخواست تسهیلات")]
        public int? RequestFacilityId { get; set; }

        [StringLength(10)]
        [Display(Name = "NationalCode", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.NationalCode, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string NationalCode { get; set; }

        [Display(Name = "تاریخ سررسید از")]
        public DateTime? StartDueDate { get; set; }

        [Display(Name = "تاریخ سررسید تا")]
        public DateTime? EndDueDate { get; set; }

        [Display(Name = "تاریخ پرداخت از")]
        public DateTime? StartRealPaymentDate { get; set; }

        [Display(Name = "تاریخ پرداخت تا")]
        public DateTime? EndRealPaymentDate { get; set; }

        public PaidStatus PaidStatus { get; set; }
    }
}
