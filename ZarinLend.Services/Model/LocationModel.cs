using Core.Entities;
using System;

namespace Services.Model
{
    [Serializable]
    public class LocationModel 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public LocationTypeEnum LocationType { get; set; }
        public int? ParentId { get; set; }
        public bool IsActive { get; set; }
    }
}
