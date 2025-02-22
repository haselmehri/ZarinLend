using Common.Utilities;
using Services.Model.IranCreditScoring;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class RequestFacilityGuarantorAddModel
    {
        public Guid GuarantorId { get; set; }
        public bool UserHasOpenRequest { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(10)]
        [Display(Name = "NationalCode", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.NationalCode,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string NationalCode { get; set; }

        public bool IsEditMode { get; set; } = false;
        public List<IranCreditScoringResultRuleModel> IranCreditScoringResultRules { get; set; }
    }
}
