using Core.Entities.Base;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    /// <summary>
    /// اطلاعات این جدول توسط وب سرویس هاس سمات پر میشود-آمار تسهیلات شخص
    /// </summary>
    public class SamatFacilityHeader : BaseEntity
    {
        public SamatFacilityHeader()
        {
            SamatFacilityDetails = new HashSet<SamatFacilityDetail>();
        }
        /// <summary>
        /// full name
        /// </summary>
        public string? Name { get; set; }
        public decimal FacilityTotalAmount { get; set; }
        public decimal FacilityDebtTotalAmount { get; set; }
        public decimal FacilityPastExpiredTotalAmount { get; set; }
        public decimal FacilityDeferredTotalAmount { get; set; }
        public decimal FacilitySuspiciousTotalAmount { get; set; }      

        #region RequestFacility
        public int RequestFacilityId { get; set; }
        public virtual RequestFacility RequestFacility { get; set; }

        #endregion
        #region Creator
        public Guid CreatorId { get; set; }
        public virtual User Creator { get; set; }
        #endregion

        public virtual ICollection<SamatFacilityDetail> SamatFacilityDetails { get; set; }
    }

    public class SamatFacilityHeaderConfiguration : BaseEntityTypeConfiguration<SamatFacilityHeader>
    {
        public override void Configure(EntityTypeBuilder<SamatFacilityHeader> builder)
        {
            builder.HasOne(p => p.RequestFacility).WithMany(p => p.SamatFacilityHeaders).HasForeignKey(p => p.RequestFacilityId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.Creator).WithMany(p => p.CreatorSamatFacilityHeaders).HasForeignKey(p => p.CreatorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
