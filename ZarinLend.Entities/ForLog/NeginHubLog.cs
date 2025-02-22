using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Core.Entities
{
    public enum NeginHubServiceName
    {
        GetToken,
        GetCivilRegistryData,
        GetCivilRegistryDataV4,
        GetCivilRegistryDataIncludePersonPhotoV4,
        SanaInquiryV5,
        PostalCodeInquiry,
        NationalCodeAndCardVerification,
        CheckShahkar,
        CardToIban,
        CreditScore_Request,
        CreditScore_Validate,
        CreditScore_Status,
        CreditScore_Pdf,
        CreditScore_Json,
    }
    public class NeginHubLog : BaseEntity<long>
    {
        public NeginHubLog()
        {

        }
        #region RequestFacility
        //public int? RequestFacilityId { get; set; }
        //public virtual RequestFacility RequestFacility { get; set; }
        #endregion
        public MethodType MethodType { get; set; }
        public string TrackId { get; set; }
        public string ServiceName { get; set; }
        public string Url { get; set; }
        public string? Body { get; set; }
        public string? Result { get; set; }
        public string? Curl { get; set; }

        #region Oprator
        public Guid? OpratorId { get; set; }
        public virtual User Oprator { get; set; }
        #endregion
    }

    public class NeginHubLogConfiguration : BaseEntityTypeConfiguration<NeginHubLog>
    {
        public override void Configure(EntityTypeBuilder<NeginHubLog> builder)
        {
            builder.Property(p => p.Url).IsRequired().HasMaxLength(1000);
            builder.Property(p => p.TrackId).IsRequired().HasMaxLength(50);
            builder.Property(p => p.ServiceName).IsRequired().HasMaxLength(100);
            //builder.HasOne(p => p.RequestFacility).WithMany(p => p.NeginHubLogs).HasForeignKey(p => p.RequestFacilityId);
            builder.HasOne(p => p.Oprator).WithMany(p => p.NeginHubLogs).HasForeignKey(p => p.OpratorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
