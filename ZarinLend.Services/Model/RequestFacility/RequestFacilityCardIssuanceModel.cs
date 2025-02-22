using Common.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    public class RequestFacilityCardIssuanceModel
    {
        public int Id { get; set; }
        public int RequestFacilityId { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(13, MinimumLength = 13,ErrorMessage ="شماره حساب بن باید 13 رقمی باشد")]
        [Display(Name = "AccountNumber", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.AccountNumber_Length13, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.AccountNumberMessage)  )]
        public string AccountNumber { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(20)]
        [Display(Name = "CardNumber", ResourceType = typeof(ResourceFile))]
        //[RegularExpression(RegularExpression.BankCardNumberWithDash,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        [RegularExpression(RegularExpression.AyandehBankCardNumberWithDash, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.AyandehBankCardNumberWithDashMessage))]
        public string? CardNumber { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(maximumLength: 5, MinimumLength = 3, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.MinMaxLengthValidationMessage))]
        [RegularExpression(RegularExpression.IntegerNumber, ErrorMessage = "{0} باید عددی 3 یا 5 رقمی باشد")]
        [Display(Name = "Cvv")]
        public string? Cvv { get; set; }

        [Display(Name = "ExpireYear", ResourceType = typeof(ResourceFile))]
        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(maximumLength: 4, MinimumLength = 2, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.MinMaxLengthValidationMessage))]
        [RegularExpression(RegularExpression.ExpireYear, ErrorMessage = "{0} باید عددی 2 یا 4 رقمی باشد")]
        public string? ExpireYear { get; set; }

        [Display(Name = "ExpireMonth", ResourceType = typeof(ResourceFile))]
        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Range(1, 12, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RangeValidationMessage))]
        [RegularExpression(RegularExpression.ExpireMonth, ErrorMessage = "{0} باید عددی بین 1 تا 12 باشد")]
        public string? ExpireMonth { get; set; }

        [Display(Name = "SecondPassword", ResourceType = typeof(ResourceFile))]
        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(maximumLength: 15, MinimumLength = 4, ErrorMessage = "{0} باید عددی بین 4 یا 15 رقمی باشد")]
        [RegularExpression(RegularExpression.IntegerNumber, ErrorMessage = "{0} باید عددی بین 4 یا 15 رقمی باشد")]
        public string? SecondPassword { get; set; }

        public Guid CreatorId { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? ShamsiCreatedDate
        {
            get
            {
                return CreatedDate.GregorianToShamsi();
            }
        }
    }
}
