using System.Collections.Generic;

namespace Services.Model
{
    public class PlanListModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long AmountWaranty { get; set; }
        public long? FacilityAmount { get; set; }
        public string FacilityTypeTitle { get; set; }
        public string OrganizationName { get; set; }
        public bool? ImportDone { get; set; }
        public bool IsActive { get; set; }
        public List<string> PlanFileUrlList { get; set; }
    }
}
