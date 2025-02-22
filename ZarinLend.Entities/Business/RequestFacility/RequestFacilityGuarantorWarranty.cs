using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using static Common.Enums;

namespace Core.Entities.Business.RequestFacility
{
    public class RequestFacilityGuarantorWarranty : BaseDocument
    {
        public RequestFacilityGuarantorWarranty()
        {

        }

        public DateTime WarantyDate { get; set; }
        public long WarantyAmount { get; set; }
        public DocumentType DocumentType { get; set; }
        public string? DocumentNumber { get; set; }

        #region RequestFacility
        public int RequestFacilityGuarantorId { get; set; }
        public virtual RequestFacilityGuarantor RequestFacilityGuarantor { get; set; }
        #endregion
    }

    public class RequestFacilityGuarantorWarrantyConfiguration : BaseEntityTypeConfiguration<RequestFacilityGuarantorWarranty>
    {
        public override void Configure(EntityTypeBuilder<RequestFacilityGuarantorWarranty> builder)
        {
            builder.HasOne(p => p.RequestFacilityGuarantor).WithOne(p => p.RequestFacilityGuarantorWarranty)
                .HasForeignKey<RequestFacilityGuarantorWarranty>(p => p.RequestFacilityGuarantorId)
                .Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.Property(p => p.DocumentType).HasDefaultValue(DocumentType.Other);
            builder.Property(p => p.DocumentNumber).HasMaxLength(50);
            builder.Property(p => p.FilePath).IsRequired();
        }
    }
}