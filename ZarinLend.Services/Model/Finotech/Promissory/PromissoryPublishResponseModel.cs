using System;

namespace ZarinLend.Services.Model.Promissory
{
    [Serializable]
    public class PromissoryPublishResponseModel
    {
        public string RequestId { get; set; }
        public string PromissoryId { get; set; }
        public string UnSignedPdf { get; set; }           
    }
}
