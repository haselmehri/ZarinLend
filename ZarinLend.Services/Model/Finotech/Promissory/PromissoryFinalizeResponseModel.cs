using System;

namespace ZarinLend.Services.Model.Promissory
{
    [Serializable]
    public class PromissoryFinalizeResponseModel
    {
        public string Message { get; set; }
        public string PromissoryId { get; set; }
        public string MultiSignedPdf { get; set; }           
    }
}
