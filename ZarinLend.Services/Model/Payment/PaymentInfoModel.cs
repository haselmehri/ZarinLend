using Common.Utilities;
using Core.Entities.Business.Payment;
using System;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model.Payment
{
    [Serializable]
    public class PaymentInfoModel
    {
        public Guid Id { get; set; }
        public IpgType IpgType { get; set; } = IpgType.SamanIPG;

        [Display(Name = "مبلغ(ريال)")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.IntegerNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        public long Amount { get; set; }

        [Display(Name = "شماره کارت")]
        [StringLength(16,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.StringLengthMessage) )]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.BankCardNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        public string CardNumber { get; set; }

        public string BuyerMobile { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} را جهت انتقال وارد کنید")]
        [Display(Name = "کد تایید")]
        [RegularExpression(RegularExpression.IntegerNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        [MaxLength(6)]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "{0} باید عددی شش(6) رقمی باشد")]
        public string Otp { get; set; }

        public Guid CreatorId { get; set; }
        public Guid SellerId { get; set; }
        public Guid BuyerId { get; set; }
        public int OrganizationId { get; set; }
        public DateTime ExpireDate { get; set; }
        public long MessageId { get; set; }
    }
}
