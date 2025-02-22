using Common.Utilities;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Common.CustomAttribute
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class NationalCodeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Assert.NotNull(value, "کد ملی را وارد کنید.");
            var nationalPersonId = value as string;

            if (string.IsNullOrEmpty(nationalPersonId?.Trim()))
                return new ValidationResult("کد ملی را وارد کنید.");

            // Check if the nationalPersonId is a 10-digit number
            if (!Regex.IsMatch(nationalPersonId, RegularExpression.NationalCode))
                return new ValidationResult("کد ملی صحیح نیست.");

            // Convert the string to an array of integers for easier manipulation
            int[] digits = nationalPersonId.Select(c => int.Parse(c.ToString())).ToArray();

            // Calculate B
            int B = digits[0] * 10 + digits[1] * 9 + digits[2] * 8 + digits[3] * 7 + digits[4] * 6 + digits[5] * 5 + digits[6] * 4 + digits[7] * 3 + digits[8] * 2;

            // Calculate C
            int C = B - (B / 11) * 11;

            // Check the conditions for a valid National Person ID
            if ((C == 0 && digits[9] == C) || (C == 1 && digits[9] == 1) || (C > 1 && digits[9] == 11 - C))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("کد ملی صحیح نیست.");
        }
    }
}
