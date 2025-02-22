using Common.Utilities;
using System;

namespace Services.Model
{
    [Serializable]
    public class RequestFacilityInstallmentModel
    {
        public int Id { get; set; }
        public int RequestFacilityId { get; set; }
        public long Amount { get; set; }
        public int PenaltyDays { get; set; }
        public long PenaltyAmount { get; set; }
        public long? RealPayAmount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? RealPayDate { get; set; }
        public DateTime? ReadDate { get; set; }
        public DateTime? StartForPayment { get; set; }
        public bool Paid { get; set; }

        public long FacilityAmount { get; set; }
        public string MonthCountTitle { get; set; }
        public string Requester { get; set; }
        public string NationalCode { get; set; }
        public string ShamsiDueDate
        {
            get
            {
                return DueDate.GregorianToShamsi();
            }
        }
        public string ShamsiRealPayDate
        {
            get
            {
                return RealPayDate.HasValue ? RealPayDate.Value.GregorianToShamsi(showTime:true) : null;
            }
        }
        public string ShamsiReadDate
        {
            get
            {
                return ReadDate.HasValue ? ReadDate.Value.GregorianToShamsi() : null;
            }
        }

        public SamanIntenetBankPaymentExportModel PaymentResult { get; set; }
    }
}
