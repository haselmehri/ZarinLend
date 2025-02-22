using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Business.RequestFacility
{
    public class RequestFaciliyUsagePlace : BaseEntity
    {
        public RequestFaciliyUsagePlace()
        {

        }

        #region RequestFacility
        public int RequestFacilityId { get; set; }
        public virtual RequestFacility RequestFacility { get; set; }
        #endregion

        #region RequestFacility
        public int UsagePlaceId { get; set; }
        public virtual UsagePlace UsagePlace { get; set; }
        #endregion
    }

    public class RequestFaciliyUsagePlaceConfiguration : BaseEntityTypeConfiguration<RequestFaciliyUsagePlace>
    {
        public override void Configure(EntityTypeBuilder<RequestFaciliyUsagePlace> builder)
        {
            builder.HasOne(p => p.RequestFacility).WithMany(p => p.RequestFaciliyUsagePlaces).HasForeignKey(p => p.RequestFacilityId);
            builder.HasOne(p => p.UsagePlace).WithMany(p => p.RequestFaciliyUsagePlaces).HasForeignKey(p => p.UsagePlaceId);
        }
    }
}