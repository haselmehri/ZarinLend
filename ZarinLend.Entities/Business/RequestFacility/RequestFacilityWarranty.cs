using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using static Common.Enums;

namespace Core.Entities.Business.RequestFacility
{
    public class RequestFacilityWarranty : BaseDocument
    {
        public RequestFacilityWarranty()
        {

        }

        public DateTime WarantyDate { get; set; }
        public long WarantyAmount { get; set; }
        public DocumentType DocumentType { get; set; }
        public string? DocumentNumber { get; set; }

        #region RequestFacility
        public int RequestFacilityId { get; set; }
        public virtual RequestFacility RequestFacility { get; set; }
        #endregion
    }

    public class RequestFacilityWarrantyConfiguration : BaseEntityTypeConfiguration<RequestFacilityWarranty>
    {
        public override void Configure(EntityTypeBuilder<RequestFacilityWarranty> builder)
        {
            builder.HasOne(p => p.RequestFacility).WithMany(p => p.RequestFacilityWarranties).HasForeignKey(p => p.RequestFacilityId);
            builder.Property(p => p.DocumentType).HasDefaultValue(DocumentType.Other);
            builder.Property(p => p.DocumentNumber).HasMaxLength(50);
            builder.Property(p => p.FilePath).IsRequired();
        }
    }
}