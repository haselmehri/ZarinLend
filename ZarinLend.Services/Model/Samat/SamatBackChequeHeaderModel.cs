using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Model
{
    public class SamatBackChequeHeaderModel
    {
        public int RequestFacilityId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ShamsiCreatedDate
        {
            get
            {
                return CreatedDate.GregorianToShamsi(showTime: true);
            }
        }
        public string CreatorName { get; set; }
        public List<SamatBackChequeDetailModel> BackChequeList { get; set; }
    }
}
