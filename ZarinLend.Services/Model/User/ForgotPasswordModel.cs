using Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class ForgotPasswordModel : IValidatableObject
    {

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "کد ملی")]
        //[Display(Name = "NationalCode", ResourceType = typeof(ResourceFile))]
        [StringLength(10)]
        [RegularExpression(RegularExpression.NationalCode,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        //[Remote()] : TODO : remember to use this feature
        public string NationalCode { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(15)]
        [Display(Name = "Mobile", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.Mobile,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string Mobile { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(5)]
        [Display(Name = "کد تایید")]
        [RegularExpression(RegularExpression.Otp5,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string Otp { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[StringLength(100, ErrorMessage = "{0} باید حداقل  {2} کاراکتر و حداکثر {1} کاراکتر باشد.", MinimumLength = 6)]
        [StringLength(100, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.PasswordMinLength), MinimumLength = 6)]
        [Display(Name = "Password", ResourceType = typeof(ResourceFile))]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [StringLength(100)]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Compare(nameof(Password),ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.PasswordCompareError))]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(ResourceFile))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string? ReturnUrl { get; set; }
        //برای چک کردن منطق تجاری برنامه مناسب است نه مثلا چک ردن نام کاربری تکراری
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
