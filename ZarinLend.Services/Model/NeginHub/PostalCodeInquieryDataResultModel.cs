namespace ZarinLend.Services.Model.NeginHub
{
    public class PostalCodeInquieryDataResultModel
    {
        public bool IsSuccess { get; set; }
        public string? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public int Code { get; set; }
        public string? ErrorType { get; set; }
    }
}
