using Core.Entities.Base;
using Core.Entities.Business.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Core.Entities.Business.RequestFacility
{
    public enum RequestFacilityGuarantorStatus
    {
        WaitingRequest = 1,
        ApprovedRequest = 2,
        RejectCancelRequest = 3,
        OpenRequest = 4,
        AllRequest = -1
    }
    public class RequestFacilityGuarantor : BaseEntity
    {
        public RequestFacilityGuarantor()
        {

        }

        public bool IsConfirm { get; set; }

        #region GuarantorUser
        public User Guarantor { get; set; }
        public Guid GuarantorUserId { get; set; }
        #endregion

        #region Approver
        public User Approver { get; set; }
        public Guid? ApproverUserId { get; set; }
        #endregion

        #region RequestFacility
        public int RequestFacilityId { get; set; }
        public virtual RequestFacility RequestFacility { get; set; }

        #endregion

        #region IranCreditScoringResultRule
        public int IranCreditScoringResultRuleId { get; set; }
        public virtual IranCreditScoringResultRule IranCreditScoringResultRule { get; set; }
        #endregion

        public bool CancelByUser { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<RequestFacilityGuarantorWorkFlowStep> RequestFacilityGuarantorWorkFlowSteps { get; set; } = new HashSet<RequestFacilityGuarantorWorkFlowStep>();
        public virtual ICollection<PaymentReason> PaymentReasons { get; set; } = new HashSet<PaymentReason>();
        public virtual ICollection<Transaction.TransactionReason> TransactionReasons { get; set; } = new HashSet<Transaction.TransactionReason>();
        public virtual ICollection<IranCreditScoring> IranCreditScorings { get; set; } = new HashSet<IranCreditScoring>();
        public virtual RequestFacilityGuarantorWarranty RequestFacilityGuarantorWarranty { get; set; }
    }

    public class RequestFacilityGuarantorConfiguration : BaseEntityTypeConfiguration<RequestFacilityGuarantor>
    {
        public override void Configure(EntityTypeBuilder<RequestFacilityGuarantor> builder)
        {
            builder.HasOne(p => p.RequestFacility).WithMany(p => p.RequestFacilityGuarantors).HasForeignKey(p => p.RequestFacilityId);
            builder.HasOne(p => p.Approver).WithMany(p => p.ApproverRequestFacilityGuarantors).HasForeignKey(p => p.ApproverUserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Guarantor).WithMany(p => p.RequestFacilityGuarantors).HasForeignKey(p => p.GuarantorUserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.IranCreditScoringResultRule).WithMany(p => p.RequestFacilityGuarantors).HasForeignKey(p => p.IranCreditScoringResultRuleId);
            builder.Property(p => p.Description).HasMaxLength(1000);
        }
    }
}