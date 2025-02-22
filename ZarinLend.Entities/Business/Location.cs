using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Core.Entities
{
    public enum LocationTypeEnum
    {
        /// <summary>
        /// استان
        /// </summary>
        Province = 1,
        /// <summary>
        /// شهر
        /// </summary>
        City = 2,
    }
    /// <summary>
    /// استان - شهر
    /// </summary>
    public class Location : BaseEntity
    {
        public Location()
        {
            Childs = new HashSet<Location>();
            CityPeople = new HashSet<Person>();
            //CityOfIssuePeople = new HashSet<Person>();
            //BirthLocationPeople = new HashSet<Person>();
            CityPlanMembers = new HashSet<PlanMember>();
            CityOfIssuePlanMembers = new HashSet<PlanMember>();
            BirthLocationPlanMembers = new HashSet<PlanMember>();
        }

        public string Name { get; set; }
        public LocationTypeEnum LocationType { get; set; }
        public int? ParentId { get; set; }
        public virtual Location Parent { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Location> Childs { get; set; }
        public virtual ICollection<Person> CityPeople { get; set; }
        //public virtual ICollection<Person> CityOfIssuePeople { get; set; }
        //public virtual ICollection<Person> BirthLocationPeople { get; set; }
        public virtual ICollection<PlanMember> CityPlanMembers { get; set; }
        public virtual ICollection<PlanMember> CityOfIssuePlanMembers { get; set; }
        public virtual ICollection<PlanMember> BirthLocationPlanMembers { get; set; }
    }

    public class LocationConfiguration : BaseEntityTypeConfiguration<Location>
    {
        public override void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.HasOne(p => p.Parent).WithMany(p => p.Childs).HasForeignKey(p => p.ParentId);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.IsActive).HasDefaultValue(true);
        }
    }
}
