using Common.Utilities;
using Core.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Business.RequestFacility
{
    public class RequestFacilityCardIssuance : BaseEntity
    {
        public RequestFacilityCardIssuance()
        {

        }
        public string AccountNumber { get; set; }
        public string CardNumber { get; set; }
        public string? Cvv { get; set; }
        public string? ExpireYear { get; set; }
        public string? ExpireMonth { get; set; }
        public string? SecondPassword { get; set; }

        #region RequestFacility
        public int RequestFacilityId { get; set; }
        public virtual RequestFacility RequestFacility { get; set; }
        #endregion
    }

    public class RequestFacilityCardIssuanceConfiguration : BaseEntityTypeConfiguration<RequestFacilityCardIssuance>
    {
        public override void Configure(EntityTypeBuilder<RequestFacilityCardIssuance> builder)
        {
            builder.HasOne(p => p.RequestFacility).WithOne(p => p.RequestFacilityCardIssuance).HasForeignKey<RequestFacilityCardIssuance>(p => p.RequestFacilityId);
            builder.Property(p => p.AccountNumber).HasMaxLength(20).IsRequired();
            builder.Property(p => p.CardNumber).HasMaxLength(16).IsRequired();
            builder.Property(p => p.Cvv).HasMaxLength(10);
            builder.Property(p => p.ExpireYear).HasMaxLength(4);
            builder.Property(p => p.ExpireMonth).HasMaxLength(2);
            builder.Property(p => p.SecondPassword).HasMaxLength(20);
        }
    }
}