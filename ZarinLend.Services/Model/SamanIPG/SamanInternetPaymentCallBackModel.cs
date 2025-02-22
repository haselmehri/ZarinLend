using Core.Entities.Business.Payment;

namespace Services.Model
{
    public class SamanInternetPaymentCallBackModel
    {
        public long Amount { get; set; }
        public string TraceNo { get; set; }
        public string Rrn { get; set; }
        public int Status { get; set; }
        public string State { get; set; }
        public string Message { get; set; }
        public string SecurePan { get; set; }
        public string RefNum { get; set; }
        public bool IsSuccess { get; set; }
        public bool InquiryShahkarAndSamatServiceDone { get; set; }
        public int? RequestFacilityId { get; set; }
        public int? RequestFacilityGuarantorId { get; set; }
        public PaymentType PaymentType { get; set; }
    }
}
