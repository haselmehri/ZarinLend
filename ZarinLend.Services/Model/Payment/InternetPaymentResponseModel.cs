using Core.Entities.Business.Payment;
using System;

namespace Services.Model.Payment
{
    [Serializable]
    public class InternetPaymentResponseModel
    {
        public long Amount { get; set; }
        public string? Authority { get; set; }
        public int? Code { get; set; }
        public string? Message { get; set; }
        public string? Card_Pan { get; set; }
        public string? Ref_Id { get; set; }
        public bool IsSuccess { get; set; }
        public bool InquiryShahkarAndSamatServiceDone { get; set; }
        public int? RequestFacilityId { get; set; }
        public int? RequestFacilityGuarantorId { get; set; }
        public PaymentType PaymentType { get; set; }
    }
}
