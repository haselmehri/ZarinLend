using Core.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Business.Transaction
{
    public class TransactionReason : BaseEntity<long>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override long Id { get => base.Id; set => base.Id = value; }
        public virtual Transaction Transaction { get; set; }

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

        #region Payment      
        public long? PaymentId { get; set; }
        public virtual Payment.Payment Payment { get; set; }
        #endregion
    }

    public class TransactionReasonConfiguration : BaseEntityTypeConfiguration<TransactionReason>
    {
        public override void Configure(EntityTypeBuilder<TransactionReason> builder)
        {
            builder.HasOne(p => p.Transaction).WithOne(p => p.TransactionReason).HasForeignKey<TransactionReason>(p => p.Id).OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);
            builder.HasOne(p => p.RequestFacility).WithMany(p => p.TransactionReasons).HasForeignKey(p => p.RequestFacilityId);
            builder.HasOne(p => p.RequestFacilityInstallment).WithMany(p => p.TransactionReasons).HasForeignKey(p => p.RequestFacilityInstallmentId);
            builder.HasOne(p => p.RequestFacilityGuarantor).WithMany(p => p.TransactionReasons).HasForeignKey(p => p.RequestFacilityGuarantorId);
            builder.HasOne(p => p.Payment).WithOne(p => p.TransactionReason).HasForeignKey<TransactionReason>(p => p.PaymentId);
        }
    }
}