using Core.Entities.Base;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Core.Entities
{
    public class NeoZarinRequestSignatureLog : BaseEntity
    {
        public NeoZarinRequestSignatureLog()
        {

        }
        public string? TrackId { get; set; }
        public string? Mobile { get; set; }
        public string? Url { get; set; }
        public string? CleanTextParam { get; set; }
        public string? Result { get; set; }

        #region RequestFacility
        /// <summary>
        /// This property indicates that the buyer's credit has been increased upon request
        /// </summary>
        public int RequestFacilityId { get; set; }
        public virtual RequestFacility RequestFacility { get; set; }
        #endregion

        #region Creator
        public Guid CreatorId { get; set; }

        public virtual User Creator { get; set; }
        #endregion      
    }

    public class NeoZarinRequestSignatureLogConfiguration : BaseEntityTypeConfiguration<NeoZarinRequestSignatureLog>
    {
        public override void Configure(EntityTypeBuilder<NeoZarinRequestSignatureLog> builder)
        {
            builder.HasIndex(p => p.TrackId).IsUnique();
            builder.HasOne(p => p.Creator).WithMany(p => p.NeoZarinRequestSignatureLogCreators).HasForeignKey(p => p.CreatorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.RequestFacility).WithMany(p => p.NeoZarinRequestSignatureLogs).HasForeignKey(p => p.RequestFacilityId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
