using Core.Entities.Business.RequestFacility;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class RequestFacilityAddModel
    {
        public bool RegisterFromZarinpal { get; set; }

        public UserOption UserOption { get; set; }
        public Guid BuyerId { get; set; }
        //public bool UserHasOpenRequest { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "AmountRequest", ResourceType = typeof(ResourceFile))]
        public long AmountRequest { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "PaymentPeriod", ResourceType = typeof(ResourceFile))]
        public int FacilityTypeId { get; set; }

        //[Display(Name = "PaymentPeriods")]
        //public IEnumerable<SelectListItem> PaymentPeriods { get; set; }

        //public IEnumerable<int>? UsagePlaceIds { get; set; }
        //public IEnumerable<SelectListItem>? UsagePlaces { get; set; }

        /// <summary>
        /// if user select 'other' this field is filled. 
        /// </summary>
        //[Display(Name = "محل مصرف تسهیلات-سایر")]
        //[MaxLength(200)]
        //public string? UsagePlaceDescription { get; set; }

        #region GlobalSetting
        public int ActiveGlobalSettingId { get; set; }
        //public long ValidationFee { get; set; }
        //public double WarantyPercentage { get; set; }
        //public double FacilityInterest { get; set; }
        //public double LendTechFacilityFee { get; set; }
        //public double FinancialInstitutionFacilityFee { get; set; }

        #endregion GlobalSetting

        //public IranCreditScoringModel IranCreditScoringResult { get; set; }
        //public List<IranCreditScoringResultRuleModel> IranCreditScoringResultRules { get; set; }
        //public string IranCreditScoringResultRulesJson
        //{
        //    get
        //    {
        //        return JsonConvert.SerializeObject(IranCreditScoringResultRules, settings: new()
        //        {
        //            Formatting = Formatting.Indented,
        //            ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
        //        });
        //    }
        //}
    }
}
