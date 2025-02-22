using Core.Entities.Base;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Core.Entities
{
    /// <summary>
    /// اطلاعات اعتبارسنجی به ازای هر درخواست تسهیلات در این حدول ذخیره میشود
    /// </summary>
    public class IranCreditScoring : BaseEntity
    {
        public IranCreditScoring()
        {
            IranCreditScoringDocuments = new HashSet<IranCreditScoringDocument>();
        }
        /// <summary>
        /// Score
        /// </summary>
        public int? Score { get; set; }

        /// <summary>
        /// risk to pay facility is shown to A,B,C...
        /// </summary>
        public string? Risk { get; set; }

        /// <summary>
        /// description about 'Risk'
        /// </summary>
        public string? Description { get; set; }

        #region RequestFacility
        public int? RequestFacilityId { get; set; }
        public virtual RequestFacility RequestFacility { get; set; }
        #endregion

        #region RequestFacilityGuarantor
        public int? RequestFacilityGuarantorId { get; set; }
        public virtual RequestFacilityGuarantor RequestFacilityGuarantor { get; set; }
        #endregion

        #region Creator
        public Guid CreatorId { get; set; }
        public virtual User Creator { get; set; }
        #endregion

        public virtual ICollection<IranCreditScoringDocument> IranCreditScoringDocuments { get; set; }
    }

    public class IranCreditScoringConfiguration : BaseEntityTypeConfiguration<IranCreditScoring>
    {
        public override void Configure(EntityTypeBuilder<IranCreditScoring> builder)
        {
            builder.HasOne(p => p.RequestFacilityGuarantor).WithMany(p => p.IranCreditScorings).HasForeignKey(p => p.RequestFacilityGuarantorId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.RequestFacility).WithMany(p => p.IranCreditScorings).HasForeignKey(p => p.RequestFacilityId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Creator).WithMany(p => p.IranCreditScorings).HasForeignKey(p => p.CreatorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.Property(p => p.Risk).HasMaxLength(250);
            builder.Property(p => p.Description).HasMaxLength(2000);
        }
    }
}
