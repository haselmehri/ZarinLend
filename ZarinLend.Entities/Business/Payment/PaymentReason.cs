using Core.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Business.Payment
{
    public class PaymentReason : BaseEntity<long>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override long Id { get => base.Id; set => base.Id = value; }
        public virtual Payment Payment { get; set; }

        #region RequestFacility        
        public int? RequestFacilityId { get; set; }
        public virtual RequestFacility.RequestFacility RequestFacility { get; set; }
        #endregion

        #region RequestFacilityInstallment        
        public int? RequestFacilityInstallmentId { get; set; }
        public virtual RequestFacility.RequestFacilityInstallment RequestFacilityInstallment { get; set; }
        #endregion

        #region RequestFacilityGuarantor      
        public int? RequestFacilityGuarantorId { get; set; }
        public virtual RequestFacility.RequestFacilityGuarantor RequestFacilityGuarantor { get; set; }
        #endregion

        #region PaymentInfo        
        public Guid? PaymentInfoId { get; set; }
        public virtual PaymentInfo PaymentInfo { get; set; }
        #endregion
    }

    public class PaymentReasonConfiguration : BaseEntityTypeConfiguration<PaymentReason>
    {
        public override void Configure(EntityTypeBuilder<PaymentReason> builder)
        {
            builder.HasOne(p => p.Payment).WithOne(p => p.PaymentReason).HasForeignKey<PaymentReason>(p => p.Id);
            builder.HasOne(p => p.RequestFacility).WithMany(p => p.PaymentReasons).HasForeignKey(p => p.RequestFacilityId);
            builder.HasOne(p => p.RequestFacilityInstallment).WithMany(p => p.PaymentReasons).HasForeignKey(p => p.RequestFacilityInstallmentId);
            builder.HasOne(p => p.RequestFacilityGuarantor).WithMany(p => p.PaymentReasons).HasForeignKey(p => p.RequestFacilityGuarantorId);
            builder.HasOne(p => p.PaymentInfo).WithMany(p => p.PaymentReasons).HasForeignKey(p => p.PaymentInfoId);
        }
    }
}