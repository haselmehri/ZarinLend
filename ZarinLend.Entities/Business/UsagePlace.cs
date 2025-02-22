using Core.Entities.Base;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class UsagePlace : BaseEntity
    {
        public UsagePlace()
        {
            RequestFaciliyUsagePlaces = new HashSet<RequestFaciliyUsagePlace>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int Id { get => base.Id; set => base.Id = value; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<RequestFaciliyUsagePlace> RequestFaciliyUsagePlaces { get; set; }
    }

    public class UsagePlaceConfiguration : BaseEntityTypeConfiguration<UsagePlace>
    {
        public override void Configure(EntityTypeBuilder<UsagePlace> builder)
        {
            builder.Property(p => p.IsActive).HasDefaultValue(true);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        }
    }
}
