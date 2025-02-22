using Common.Utilities;
using Core.Entities.Business.Payment;
using System;

namespace Services.Model.Payment
{
    [Serializable]
    public class IntenetBankPaymentModel
    {
        //public long Id { get; set; }
        public IpgType IpgType { get; set; }
        public PaymentType PaymentType { get; set; }
        public long Amount { get; set; }
        public bool? IsSuccess { get; set; }
        public string MaskedPan { get; set; }
        public string RefNum { get; set; }
        public string Rrn { get; set; }
        public string TraceNum { get; set; }
        public Guid UserId { get; set; }
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
