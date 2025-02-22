using Core.Entities.Base;
using Core.Entities.Business.Payment;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Core.Entities.Business.RequestFacility
{
    public class RequestFacilityInstallment : BaseEntity
    {
        public RequestFacilityInstallment()
        {

        }
        public DateTime DueDate { get; set; }
        public long Amount { get; set; }
        public DateTime? RealPayDate { get; set; }
        public int? PenaltyDays { get; set; }
        public long? PenaltyAmount { get; set; }
        public long? RealPayAmount { get; set; }
        public DateTime? StartForPayment { get; set; }
        public bool Paid { get; set; }

        #region RequestFacility
        public int RequestFacilityId { get; set; }
        public virtual RequestFacility RequestFacility { get; set; }
        #endregion

        public virtual ICollection<PaymentReason> PaymentReasons { get; set; } = new HashSet<PaymentReason>();
        public virtual ICollection<Transaction.TransactionReason> TransactionReasons { get; set; } = new HashSet<Transaction.TransactionReason>();
    }

    public class RequestFacilityInstallmentConfiguration : BaseEntityTypeConfiguration<RequestFacilityInstallment>
    {
        public override void Configure(EntityTypeBuilder<RequestFacilityInstallment> builder)
        {
            builder.HasOne(p => p.RequestFacility).WithMany(p => p.RequestFacilityInstallments).HasForeignKey(p => p.RequestFacilityId);
        }
    }
}