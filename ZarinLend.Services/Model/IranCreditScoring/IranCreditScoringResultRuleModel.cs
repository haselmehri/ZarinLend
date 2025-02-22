using Common.Utilities;
using System;

namespace Services.Model.IranCreditScoring
{

    public class IranCreditScoringResultRuleModel
    {
        public string Risk { get; set; }
        public long? MinimumAmount { get; set; }
        public long? MaximumAmount { get; set; }
        public bool GuarantorIsRequired { get; set; }
        public DateTime CreateDate { get; set; }
        public string ShamsiCreateDate
        {
            get
            {
                return CreateDate.GregorianToShamsi(showTime: true);
            }
        }
    }
}
