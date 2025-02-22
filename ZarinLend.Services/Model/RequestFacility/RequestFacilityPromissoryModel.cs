using System;
using System.ComponentModel.DataAnnotations;

namespace Services.Model
{
    public class RequestFacilityPromissoryModel
    {
        public int Id { get; set; }
        public int RequestFacilityId { get; set; }

        [Display(Name = "نام صادر کننده سفته")]
        public string IssuerName { get; set; }

        [Display(Name = "کد ملی صادر کننده سفته")]
        public string IssuerIdCode { get; set; }

        [Display(Name = "تاریخ تولد صادر کننده سفته")]       
        public string BirthDate { get; set; }

        [Display(Name = "شماره شبا صادر کننده سفته")]
        public string IssuerIban { get; set; }

        [Display(Name = "کد پستی صادر کننده سفته")]
        public string IssuerPostalCode { get; set; }

        [Display(Name = "آدرس صادر کننده سفته")]
        public string IssuerAddress { get; set; }

        [Display(Name = "موبایل صادر کننده سفته")]
        public string IssuerMobile { get; set; }

        [Display(Name = "نام دریافت کننده سفته")]
        public string ReceiverName { get; set; } = "بانک آینده سهامی عام";

        [Display(Name = "کد ملی دریافت کننده سفته")]
        public string ReceiverIdCode { get; set; } = "10320894878";

        [Display(Name = "موبایل دریافت کننده سفته")]
        public string ReceiverMobile { get; set; } = "09171851169";

        [Display(Name = "آدرس دریافت کننده سفته")]
        public string PaymentPlace { get; set; } = "تهران،بزرگراه مدرس جنوب به شمال،خروجی الهیه،نبش خیابان بیدار،برج آینده";

        [Display(Name = "مبلغ سفته")]
        public long Amount { get; set; }

        //[Display(Name = "تاریخ سررسید")]
        //public string DuDate { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Display(Name = "تاریخ صدور سفته")]
        public DateTime CreateDate { get; set; }
        public string TrackId { get; set; }

        #region Response-> PromissoryPublishRequest api
        public string? RequestId { get; set; }
        public string? PromissoryId { get; set; }
        public string? UnSignedPdf { get; set; }
        #endregion Response-> PromissoryPublishRequest api

        //#region request body -> request sign api
        //public string? Application { get; set; }
        //public string? CallbackUrl { get; set; }
        //public bool? LetSignerDownload { get; set; }
        //public string? State { get; set; }
        //#endregion request body -> request sign api

        #region Response -> request sign api
        public string? SigningTrackId { get; set; }
        #endregion Response body -> request sign api

        #region Response -> promissoryFinalize api
        public string? MultiSignedPdf { get; set; }
        #endregion Response -> promissoryFinalize api

        #region Response -> statusInquiry api
        public int? SigningStatus { get; set; }
        #endregion Response -> statusInquiry api
    }
}
