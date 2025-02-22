using System;
using System.ComponentModel.DataAnnotations;

namespace ZarinLend.Services.Model.Promissory
{
    public enum PromissoryStatusInquiryEnum
    {
        /// <summary>
        /// نامشخص
        /// </summary>
        [Display(Name = "نامشخص")]
        Unknown = 0,

        /// <summary>
        /// وجود ندارد
        /// </summary>
        [Display(Name = "وجود ندارد")]
        NotExist = 1,

        /// <summary>
        /// هنوز امضا نشده
        /// </summary>
        [Display(Name = "هنوز امضا نشده")]
        NotSignedYet = 2,

        /// <summary>
        /// منقضی شده
        /// </summary>
        [Display(Name = "منقضی شده")]
        Expired = 3,

        /// <summary>
        /// امضا شده
        /// </summary>
        [Display(Name = "امضا شده")]
        Signed = 4,

        /// <summary>
        /// تحویل داده شده
        /// </summary>
        [Display(Name = "تحویل داده شده")]
        Delivered = 5,

        /// <summary>
        /// رد شده
        /// </summary>
        [Display(Name = "رد شده")]
        Rejected = 6
    }
    [Serializable]
    public class PromissoryStatusInquiryResponseModel
    {
        public int SigningStatus { get; set; }
        public PromissoryStatusInquiryEnum MultiSignedPdf
        {
            get
            {
                return (PromissoryStatusInquiryEnum)SigningStatus;
            }
        }
    }
}
