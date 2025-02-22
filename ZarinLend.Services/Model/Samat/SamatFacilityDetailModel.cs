using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Model
{
    public class SamatFacilityDetailModel
    {
        public string BranchDescription { get; set; }
        public string FacilityBankCode { get; set; }
        public string FacilityBranchCode { get; set; }
        public string FacilityBranch { get; set; }
        public decimal FacilityAmountOrginal { get; set; }
        public decimal FacilityBenefitAmount { get; set; }
        public decimal FacilityDebtorTotalAmount { get; set; }
        public decimal FacilityDeferredAmount { get; set; }
        public decimal FacilityPastExpiredAmount { get; set; }
        public decimal FacilitySuspiciousAmount { get; set; }
        public string FacilitySetDate { get; set; }
        public string FacilityEndDate { get; set; }
    }
}
