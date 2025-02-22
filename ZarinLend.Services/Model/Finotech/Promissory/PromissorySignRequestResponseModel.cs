using System;

namespace ZarinLend.Services.Model.Promissory
{
    [Serializable]
    public class PromissorySignRequestResponseModel 
    {
        public string SigningTrackId { get; set; }
        public string SignerName { get; set; }
        public string NationalCode { get; set; }
        public string Title { get; set; }                  
        public NotificationResponseModel NotificationResponse { get; set; }                  
    }

    public class NotificationResponseModel
    {
        public string Result { get; set; }
        public int Status { get; set; }
        public bool Success { get; set; }
    }
}
