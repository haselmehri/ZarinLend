using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Utilities;
using Microsoft.AspNetCore.Mvc;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class LoginModel : IValidatableObject
    {
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name ="نام کاربری/کد ملی")]
        //[Display(Name = "NationalCode", ResourceType = typeof(ResourceFile))]
        //[StringLength(10)]
        //[RegularExpression(RegularExpression.NationalCode,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        //[Remote()] : TODO : remember to use this feature
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "Password", ResourceType = typeof(ResourceFile))]
        [DataType(DataType.Password)]
        public string Password { get; set; }

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

        [Display(Name = "مرا بخاطر بسپار")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        //برای چک کردن منطق تجاری برنامه مناسب است نه مثلا چک ردن نام کاربری تکراری
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }

    [Serializable]
    public class LoginWithUserPassModel : IValidatableObject
    {
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "نام کاربری/کد ملی")]
        //[Display(Name = "NationalCode", ResourceType = typeof(ResourceFile))]
        //[StringLength(10)]
        //[RegularExpression(RegularExpression.NationalCode,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        //[Remote()] : TODO : remember to use this feature
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "Password", ResourceType = typeof(ResourceFile))]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "مرا بخاطر بسپار")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }

        public IList<AuthenticationScheme>? ExternalLogins { get; set; }

        //برای چک کردن منطق تجاری برنامه مناسب است نه مثلا چک ردن نام کاربری تکراری
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }

    [Serializable]
    public class OtpModel : IValidatableObject
    {
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(15)]
        [Display(Name = "Mobile", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.Mobile,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string Mobile { get; set; }

        //برای چک کردن منطق تجاری برنامه مناسب است نه مثلا چک ردن نام کاربری تکراری
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }

    [Serializable]
    public class LoginWithOtpModel : IValidatableObject
    {
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(15)]
        [Display(Name = "Mobile")]
        [RegularExpression(RegularExpression.Mobile,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string Mobile { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(5)]
        [Display(Name = "کد تایید")]
        [RegularExpression(RegularExpression.Otp5,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string Otp { get; set; }

        public string? ReturnUrl { get; set; }

        public IList<AuthenticationScheme>? ExternalLogins { get; set; }

        //برای چک کردن منطق تجاری برنامه مناسب است نه مثلا چک ردن نام کاربری تکراری
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
