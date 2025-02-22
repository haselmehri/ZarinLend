using Common.Utilities;
using Newtonsoft.Json.Linq;
using Services.Model.Payment;
using System;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model.IranCreditScoring
{
    public enum IranCreditScoringRequestType
    {
        ForFacilityRequest = 1,
        ForFacilityRequestGuarantor = 2,
        BeforeFacilityRequest = 3
    }
    [Serializable]
    public class IranCreditScoringInputModel
    {
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "شناسه درخواست")]
        public int RequestId { get; set; }

        public IranCreditScoringRequestType IranCreditScoringRequestType { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(maximumLength: 10, MinimumLength = 10, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.MinMaxLengthValidationMessage))]
        [Display(Name = "کد ملی")]
        [RegularExpression(RegularExpression.NationalCode,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string NationalCode { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(maximumLength: 11, MinimumLength = 11, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.MinMaxLengthValidationMessage))]
        [Display(Name = "موبایل")]
        [RegularExpression(RegularExpression.Mobile,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string Mobile { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "کد مجوز")]
        public string Otp { get; set; }

        public InternetPaymentResponseModel? ZarinPalPaymentResponse { get; set; }
        public SamanInternetPaymentCallBackModel? SamanPaymentResponse { get; set; }
    }

    public class IranCreditScoringModel
    {
        public IranCreditScoringRequestType IranCreditScoringRequestType { get; set; }
        public int RequestId { get; set; }
        public int? Score { get; set; }
        public string? Risk { get; set; }
        public string? Description { get; set; }
        public string? Xml { get; set; }
        public string PdfBase64String { get; set; }
        public string Json { get; set; }
        public string? PdfUrl { get; set; }
        public string? JsonUrl { get; set; }
        public string? XmlUrl { get; set; }
        public DateTime CreateDate { get; set; }
        public string ShamsiCreateDate
        {
            get
            {
                return CreateDate.GregorianToShamsi(showTime: true);
            }
        }
    }
    public class IranCreditScoringResult
    {
        public IranCreditScoringResult()
        {
            Score = new IranCreditScoringModel();
        }
        public IranCreditScoringModel Score { get; set; }
        public bool HasError { get; set; } = false;
        public JObject ErrorResult { get; set; } = null;

    }


    [Serializable]
    public class ValidateInput
    {
        public IranCreditScoringRequestType IranCreditScoringRequestType { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "شناسه درخواست")]
        public int RequestId { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[Display(Name = "AccessToken")]
        public string? AccessToken { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[Display(Name = "ApiKey")]
        public string? ApiKey { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "HashCode")]
        public string HashCode { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "کد مجوز")]
        public string Otp { get; set; }
    }
}
