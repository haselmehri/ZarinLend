using System.ComponentModel.DataAnnotations;

namespace Common
{
    public  class Enums
    {
        #region BaseDocumnet entity Enums
        public enum DocumentStatus
        {
            [Display(Name = "فعال")]
            Active = 1,

            [Display(Name = "غیرفعال")]
            DeActive = 2
        }

        public enum FileType
        {
            Video = 1,
            Image = 2,
            Docs = 3,
            Voice = 4,
            Pdf = 5,
            Xml = 6,
            Json = 7,
            Excel = 8,
            Unknown = 1000
        }

        public enum DocumentType
        {
            /// <summary>
            /// تصویر صفحه اول شناسنامه
            /// </summary>
            [Display(Name = "تصویر صفحه اول شناسنامه")]
            BirthCertificatePage1 = 1,

            /// <summary>
            /// تصویر صفحه دوم شناسنامه
            /// </summary>
            [Display(Name = "تصویر صفحه دوم شناسنامه")]
            BirthCertificatePage2 = 2,

            /// <summary>
            /// تصویر روی کارت ملی
            /// </summary>
            [Display(Name = "تصویر روی کارت ملی")]
            NationalCardFront = 3,
            /// <summary>
            /// تصویر پشت کارت ملی
            /// </summary>
            [Display(Name = "تصویر پشت کارت ملی")]
            NationalCardBack = 4,

            /// <summary>
            /// کارت پایان خدمت سربازی
            /// </summary>
            [Display(Name = "کارت پایان خدمت سربازی")]
            MilitaryServiceCard = 5,

            /// <summary>
            /// تصویر روزنامه رسمی
            /// </summary>
            [Display(Name = "تصویر روزنامه رسمی")]
            OfficialNewspaper = 6,

            /// <summary>
            /// چک صیادی
            /// </summary>
            [Display(Name = "چک صیادی")]
            Cheque = 7,

            /// <summary>
            /// فایل PDF اعتبارسنجی ایرانیان
            /// </summary>
            [Display(Name = "فایل PDF اعتبارسنجی ایرانیان")]
            IranCreditScoringPDF = 8,

            /// <summary>
            /// فایل JSON اعتبارسنجی ایرانیان
            /// </summary>
            [Display(Name = "فایل JSON اعتبارسنجی ایرانیان")]
            IranCreditScoringJSON = 9,

            /// <summary>
            /// فایل XML اعتبارسنجی ایرانیان
            /// </summary>
            [Display(Name = "فایل XML اعتبارسنجی ایرانیان")]
            IranCreditScoringXML = 10,

            /// <summary>
            /// سند صدور بیمه
            /// </summary>
            [Display(Name = "سند صدور بیمه")]
            InsuranceIssuance = 11,

            /// <summary>
            /// مدرک شغلی
            /// </summary>
            [Display(Name = "مدرک شغلی")]
            JobDocument = 12,

            /// <summary>
            /// تصویر توضیحات شناسنامه
            /// </summary>
            [Display(Name = "تصویر توضیحات شناسنامه")]
            BirthCertificateDescription = 13,

            /// <summary>
            /// تصویر سند مالکیت یا اجاره نامه
            /// </summary>
            [Display(Name = "تصویر سند مالکیت یا اجاره نامه")]
            AddressDocument = 14,

            /// <summary>
            /// فایل PDF سفته
            /// </summary>
            [Display(Name = "فایل PDF سفته الکترونیکی")]
            PromissoryNoteDocument = 15,

            /// <summary>
            /// فایل صورتحساب/گردش حساب
            /// </summary>
            [Display(Name = "فایل صورتحساب/گردش حساب")]
            AccountStatement = 16,

            /// <summary>
            /// سایر
            /// </summary>
            [Display(Name = "سایر")]
            Other = 100
        }

        #endregion
    }
}
