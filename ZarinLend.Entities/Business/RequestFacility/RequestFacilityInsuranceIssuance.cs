using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Core.Entities.Business.RequestFacility
{
    /// <summary>
    /// صدور بیمه برای تسهیلات
    /// </summary>
    public class RequestFacilityInsuranceIssuance : BaseEntity
    {
        public RequestFacilityInsuranceIssuance()
        {
            RequestFacilityInsuranceIssuanceDocuments = new HashSet<RequestFacilityInsuranceIssuanceDocument>();
        }
        /// <summary>
        /// InsuranceNumber
        /// </summary>
        public string? InsuranceNumber { get; set; }

        /// <summary>
        /// description about 'Risk'
        /// </summary>
        public string? Description { get; set; }

        #region RequestFacility
        public int RequestFacilityId { get; set; }
        public virtual RequestFacility RequestFacility { get; set; }
        #endregion

        #region Creator
        public Guid CreatorId { get; set; }
        public virtual User Creator { get; set; }
        #endregion

        public virtual ICollection<RequestFacilityInsuranceIssuanceDocument> RequestFacilityInsuranceIssuanceDocuments { get; set; }
    }

    public class RequestFacilityInsuranceIssuanceConfiguration : BaseEntityTypeConfiguration<RequestFacilityInsuranceIssuance>
    {
        public override void Configure(EntityTypeBuilder<RequestFacilityInsuranceIssuance> builder)
        {
            builder.HasOne(p => p.RequestFacility).WithMany(p => p.RequestFacilityInsuranceIssuances).HasForeignKey(p => p.RequestFacilityId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Creator).WithMany(p => p.RequestFacilityInsuranceIssuances).HasForeignKey(p => p.CreatorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.Property(p => p.InsuranceNumber).HasMaxLength(50);
            builder.Property(p => p.Description).HasMaxLength(2000);
        }
    }
}
