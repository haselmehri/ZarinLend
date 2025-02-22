using System.ComponentModel.DataAnnotations;

namespace PushNotificationInPWA.Models
{
    [Serializable]
    public class LoginModel : IValidatableObject
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "RequiredValidationMessage")]
        [StringLength(15)]
        [Display(Name = "Mobile")]
        [RegularExpression(@"^09([01239])\d{8}$", ErrorMessage = "فرمت شماره موبایل صحیح نمی باشد")]
        public string Mobile { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "RequiredValidationMessage")]
        [StringLength(5)]
        [Display(Name = "کد تایید")]
        [RegularExpression(@"^[0-9]{5}$", ErrorMessage = "رمز یکبار مصرف باید عددی 5 رقمی باشد!")]
        public string Otp { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}
