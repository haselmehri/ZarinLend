using Core.Entities.Business.Payment;

namespace Services.Model.Payment
{
    public class InternetPaymentModel
    {
        public int? RequestFacilityId { get; set; }
        public int RequestFacilityGuarantorId { get; set; }
        public long Amount { get; set; }
        public string Description { get; set; }
        public string PostAction { get; set; }
        public PaymentType PaymentType { get; set; }
        public  bool ValidationMustBeRepeated { get; set; }
        public  bool ExistValidation { get; set; }
        public string Error { get; set; }
    }
}
