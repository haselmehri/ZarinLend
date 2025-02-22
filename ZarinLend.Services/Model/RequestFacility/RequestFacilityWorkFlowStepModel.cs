using Common.Utilities;
using System;

namespace Services.Model
{
    [Serializable]
    public class CompletedWorkFlowStepModel
    {
        public int WorkFlowStepId { get; set; }
        public short? StatusId { get; set; }
        public string StatusDescription { get; set; }
        public DateTime CreateDate { get; set; }
        public string ShamsiCreateDate
        {
            get
            {
                return CreateDate.GregorianToShamsi(showTime: true);
            }
        }

        public DateTime? UpdateDate { get; set; }
        public string ShamsiUpdateDate
        {
            get
            {
                return UpdateDate.HasValue ? UpdateDate.Value.GregorianToShamsi(showTime: true) : null;
            }
        }
    }
}
