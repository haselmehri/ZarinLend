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
    /// اطلاعات این جدول توسط وب سرویس هاس سمات پر میشود-آمار چکهای برگشتی شخص
    /// </summary>
    public class SamatBackChequeHeader : BaseEntity
    {
        public SamatBackChequeHeader()
        {
            SamatBackChequeDetails = new HashSet<SamatBackChequeDetail>();
        }

        /// <summary>
        /// full name
        /// </summary>
        public string? Name { get; set; }

        #region RequestFacility
        public int RequestFacilityId { get; set; }
        public virtual RequestFacility RequestFacility { get; set; }
        #endregion

        #region Creator
        public Guid CreatorId { get; set; }
        public virtual User Creator { get; set; }
        #endregion

        public virtual ICollection<SamatBackChequeDetail> SamatBackChequeDetails { get; set; }
    }

    public class SamatBackChequeHeaderConfiguration : BaseEntityTypeConfiguration<SamatBackChequeHeader>
    {
        public override void Configure(EntityTypeBuilder<SamatBackChequeHeader> builder)
        {
            builder.HasOne(p => p.RequestFacility).WithMany(p => p.SamatBackChequeHeaders).HasForeignKey(p => p.RequestFacilityId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.Creator).WithMany(p => p.CreatorSamatBackChequeHeaders).HasForeignKey(p => p.CreatorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;        
        }
    }
}
