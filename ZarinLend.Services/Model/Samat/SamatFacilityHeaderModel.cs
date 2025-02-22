using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Model
{
    public class SamatFacilityHeaderModel
    {
        public int RequestFacilityId { get; set; }
        public string Name { get; set; }
        public decimal FacilityTotalAmount { get; set; }
        public decimal FacilityDebtTotalAmount { get; set; }
        public decimal FacilityPastExpiredTotalAmount { get; set; }
        public decimal FacilityDeferredTotalAmount { get; set; }
        public decimal FacilitySuspiciousTotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ShamsiCreatedDate
        {
            get
            {
                return CreatedDate.GregorianToShamsi(showTime: true);
            }
        }
        public string CreatorName { get; set; }
        public List<SamatFacilityDetailModel> FacilityList { get; set; }
    }
}
