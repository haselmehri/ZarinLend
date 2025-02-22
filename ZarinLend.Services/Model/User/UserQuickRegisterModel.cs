using Common.CustomAttribute;
using Common.Utilities;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class UserQuickRegisterModel : IValidatableObject
    {
        public bool LoginFromZarinpal { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(100)]
        [Display(Name = "UserName", ResourceType = typeof(ResourceFile))]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(15)]
        [Display(Name = "Mobile", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.Mobile,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string Mobile { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(10,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.StringLengthMessage) )]
        [Display(Name = "NationalCode", ResourceType = typeof(ResourceFile))]
        //[RegularExpression(RegularExpression.NationalCode,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        [NationalCode]
        public string NationalCode { get; set; }


        [Display(Name = "BirthDate", ResourceType = typeof(ResourceFile))]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public DateTime BirthDate { get; set; }

        [Display(Name = "BirthDate", ResourceType = typeof(ResourceFile))]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string ShamsiBirthDate { get; set; }


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


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!NationalCodeHelper.IsValidIranianNationalCode(NationalCode))
                yield return new ValidationResult("فرمت کد ملی صحیح نمی باشد!");
            yield return ValidationResult.Success;
        }
    }
}
