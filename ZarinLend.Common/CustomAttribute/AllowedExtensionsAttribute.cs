using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Common.CustomFileAttribute
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] extensions;
        private readonly string errorMessage;
        public AllowedExtensionsAttribute(string[] extensions, string errorMessage = null)
        {
            this.extensions = extensions;
            this.errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage())!;
                }
            }

            return ValidationResult.Success!;
        }

        public string GetErrorMessage()
        {
            if (!string.IsNullOrEmpty(errorMessage))
                return errorMessage;

            return $"فقط فایل هایی با پسوند {string.Join(",", extensions)} می توانید بارگذاری کنید!";
        }
    }
}
