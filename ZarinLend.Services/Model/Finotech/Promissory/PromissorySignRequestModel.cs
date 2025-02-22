using Common.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace ZarinLend.Services.Model.Promissory
{
    [Serializable]
    public class PromissorySignRequestModel : IValidatableObject
    {
        [JsonProperty("application")]
        public string Application { get; set; } = "01";

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "ترک آی دی مرحله صدور سفته")]
        [JsonProperty("registrationId")]
        public string RegistrationId { get; set; }

        [Display(Name = "آدرس بازگشتی")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [JsonProperty("callbackUrl")]
        public string CallbackUrl { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("letSignerDownload")]
        public bool LetSignerDownload { get; set; } = true;

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(10,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.StringLengthMessage) )]
        [Display(Name = "کد ملی صادر کننده سفته")]
        [RegularExpression(RegularExpression.NationalCode,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        [JsonProperty("idCode")]
        public string IdCode { get; set; }

       
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!NationalCodeHelper.IsValidIranianNationalCode(IdCode))
                yield return new ValidationResult("فرمت کد ملی صادر کننده سفته صحیح نمی باشد!");

            yield return ValidationResult.Success!;
        }

     
    }
}
