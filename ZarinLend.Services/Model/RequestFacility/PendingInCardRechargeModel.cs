namespace ZarinLend.Services.Model
{
    public class PendingInCardRechargeModel : PendingInEnterFacilityNumberModel
    {
        public string FacilityNumber { get; set; }
        public string CardNumber { get; set; }
    }
}
