using Common.Utilities;
using System;

namespace Services.Model
{
    [Serializable]
    public class RequestFacilityWorkFlowStepHistoryModel
    {
        public string WorkFlowStepName { get; set; }
        public short? StatusId { get; set; }
        public string StatusDescription { get; set; }
        public string StatusName { get; set; }
        public bool IsApproveFinalStep { get; set; }
        public bool IsLastStep { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        /// <summary>
        /// operator user with fullname
        /// </summary>
        public string Operator { get; set; }
        public string ShamsiCreateDate
        {
            get
            {
                return CreateDate.GregorianToShamsi(showTime: true);
            }
        }
        public string ShamsiUpdateDate
        {
            get
            {
                return UpdateDate.HasValue ? UpdateDate.Value.GregorianToShamsi(showTime: true) : ShamsiCreateDate;
            }
        }

    }
}
