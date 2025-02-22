using Common.Utilities;
using Core.Entities.Business.Transaction;
using System;

namespace Services.Model.Transaction
{
    [Serializable]
    public class TransactionModel
    {
        public TransactionTypeEnum TransactionType { get; set; }
        public long Amount { get; set; }
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
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
                return UpdateDate.HasValue ? UpdateDate.Value.GregorianToShamsi(showTime: true) : null;
            }
        }
    }
}
