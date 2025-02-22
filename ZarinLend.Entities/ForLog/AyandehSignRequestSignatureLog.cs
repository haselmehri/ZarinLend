using Core.Entities.Base;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Core.Entities
{
    public enum AyandehSignServiceName
    {
        GetSigningToken,
        GetData,
        CheckStatus
    }
    public class AyandehSignRequestSignatureLog : BaseEntity
    {
        public AyandehSignRequestSignatureLog()
        {

        }
        public string? ServiceName { get; set; }
        public string? Url { get; set; }
        public string? Body { get; set; }
        public MethodType MethodType { get; set; }
        public string? ResponseMessage { get; set; }
        public string? Curl { get; set; }
        public string? TrackId { get; set; }

        #region RequestFacility
        public int RequestFacilityId { get; set; }
        public virtual RequestFacility RequestFacility { get; set; }
        #endregion

        #region Creator
        public Guid? CreatorId { get; set; }

        public virtual User Creator { get; set; }
        #endregion      
    }

    public class AyandehSignRequestSignatureLogConfiguration : BaseEntityTypeConfiguration<AyandehSignRequestSignatureLog>
    {
        public override void Configure(EntityTypeBuilder<AyandehSignRequestSignatureLog> builder)
        {
            builder.Property(p => p.Url).IsRequired().HasMaxLength(1000);
            builder.Property(p => p.TrackId).HasMaxLength(50);
            builder.Property(p => p.ServiceName).IsRequired().HasMaxLength(100);
            builder.HasOne(p => p.Creator).WithMany(p => p.AyandehSignRequestSignatureLogCreators).HasForeignKey(p => p.CreatorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.RequestFacility).WithMany(p => p.AyandehSignRequestSignatureLogs).HasForeignKey(p => p.RequestFacilityId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
