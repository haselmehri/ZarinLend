using Common.Utilities;
using Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model.IranCreditScoring
{
    [Serializable]
    public class IranCreditScoringResultRuleAddEditModel
    {
        [Required(AllowEmptyStrings = true,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "جامعه هدف")]
        public IranCreditScoringResultRuleType IranCreditScoringResultRuleType { get; set; }

        [Required(AllowEmptyStrings = true,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.EnglishAndNumberCharacter, ErrorMessage = "EnglishAndNumberRegularExpressionMessage")]
        [StringLength(5,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.StringLengthMessage) )]
        [Display(Name = "امتیاز")]
        public string Risk { get; set; }        

        [Display(Name = "حداقل مبلغ قابل درخواست(ريال)")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.IntegerNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        public long? MinimumAmount { get; set; }

        [Display(Name = "حداقل مبلغ قابل درخواست(ريال)")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.NumberThousandSeparator,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        public string MinimumAmountThousandSeparator { get; set; }

        [Display(Name = "حداکثر مبلغ قابل درخواست(ريال)")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.IntegerNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        public long? MaximumAmount { get; set; }

        [Display(Name = "حداکثر مبلغ قابل درخواست(ريال)")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.NumberThousandSeparator,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.IntegerRegularExpression))]
        public string MaximumAmountThousandSeparator { get; set; }

        [Display(Name = "نیاز به ضامن دارد؟")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public bool GuarantorIsRequired { get; set; }

        [Display(Name = "وضعیت")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public bool IsActive { get; set; }

        public Guid CreatorId { get; set; }
        public Guid UpdaterId { get; set; }
    }
}
