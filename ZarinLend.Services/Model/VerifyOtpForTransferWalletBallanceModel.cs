using Common.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    public class VerifyOtpForTransferWalletBallanceModel
    {
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public Guid BuyerId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} را جهت انتقال وارد کنید")]
        [Display(Name = "میزان اعتبار(ريال)")]
        [RegularExpression(RegularExpression.IntegerNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public long Amount { get; set; }

        [Display(Name = "شماره کارت")]
        [StringLength(16,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.StringLengthMessage) )]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.BankCardNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        public string CardNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} را جهت انتقال وارد کنید")]
        [Display(Name = "کد تایید")]
        [RegularExpression(RegularExpression.IntegerNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        [MaxLength(6)]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "{0} باید عددی شش(6) رقمی باشد")]
        public string VerificationCode { get; set; }

        #region fillautomatic

        public Guid SellerId { get; set; }
        public int OrganizationId { get; set; }
        public string  BuyerMobile { get; set; }

        #endregion
    }
}
