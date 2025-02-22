using Services.Model;

namespace ZarinLend.Services.Model
{
    public class PendingInEnterFacilityNumberModel
    {
        public int Id { get; set; }
        public long Amount { get; set; }
        public string Requester { get; set; }
        public string RequesterFName { get; set; }
        public string RequesterLName { get; set; }
        public string NationalCode { get; set; }
        public string CustomerNumber { get; set; }
        public string AccountNumber { get; set; }
    }
}
