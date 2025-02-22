using Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class ResetPasswordModel : IValidatableObject
    {

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "کد ملی")]
        //[Display(Name = "NationalCode", ResourceType = typeof(ResourceFile))]
        [StringLength(10)]
        [RegularExpression(RegularExpression.NationalCode, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        //[Remote()] : TODO : remember to use this feature
        public string NationalCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(15)]
        [Display(Name = "Mobile", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.Mobile, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string Mobile { get; set; }


        //برای چک کردن منطق تجاری برنامه مناسب است نه مثلا چک ردن نام کاربری تکراری
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
