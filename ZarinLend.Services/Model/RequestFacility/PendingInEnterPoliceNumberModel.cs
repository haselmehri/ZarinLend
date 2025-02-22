namespace ZarinLend.Services.Model
{
    public class PendingInEnterPoliceNumberModel : PendingInEnterFacilityNumberModel
    {
        public string FacilityNumber { get; set; }
        public string PromissoryNumber { get; set; }
        public string PoliceNumber { get; set; }
    }
}
