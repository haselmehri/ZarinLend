using Services.Dto;
using System.Collections.Generic;

namespace Services.Model
{
    public class RequestFacilityIdsForSignModel
    {
        public PagingFilterDto Filter { get; set; }
        public List<int> CheckedRequestFacilityIds { get; set; }
    }
}
