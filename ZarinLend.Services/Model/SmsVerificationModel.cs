using System;

namespace Services.Model
{
    public class SmsVerificationModel
    {
        public long MessageId { get; set; }
        public string Message { get; set; }
        public string MessageTemplate { get; set; }
        public string Token { get; set; }
        public string Token2 { get; set; }
        public string Token3 { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public string Sender { get; set; }
        public string Receptor { get; set; }
        public int Cost { get; set; }
        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public DateTime ExpireTime { get; set; }   
        public Guid SenderId { get; set; }    
        public Guid ReceptorId { get; set; }
    }
}
