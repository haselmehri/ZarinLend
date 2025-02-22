using Core.Entities;
using System;

namespace Services.Model
{
    public class RequestFacilityGuarantorStatusModel
    {
        public int RequestFacilityGuarantorId { get; set; }
        public Guid OpratorId { get; set; }
        public StatusEnum Status { get; set; }
        public string StatusDescription { get; set; }
    }
}
