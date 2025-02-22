using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Business.Transaction
{
    public enum TransactionTypeEnum
    {
        /// <summary>
        /// برداشت
        /// </summary>
        [Display(Name = "برداشت")]
        Withdraw = 1,

        /// <summary>
        /// واریز
        /// </summary>
        [Display(Name = "واریز")]
        Deposit = 2,

        /// <summary>
        /// کارمزد
        /// </summary>
        [Display(Name = "کارمزد")]
        Fee = 3
    }

    public class Transaction : BaseEntity<long>
    {
        public Transaction()
        {

        }
        public long Amount { get; set; }
        public string? Description { get; set; }
        public TransactionTypeEnum TransactionType { get; set; }

        #region Payment
        //public long? PaymentId { get; set; }
        //public virtual Payment Payment { get; set; }
        #endregion Payment

        #region RequestFacility
        /// <summary>
        /// This property indicates that the buyer's credit has been increased upon request
        /// </summary>
        //public int? RequestFacilityId { get; set; }
        //public virtual RequestFacility RequestFacility { get; set; }
        #endregion

        #region RequestFacilityGuarantor
        /// <summary>
        /// This property indicates that the buyer's credit has been increased upon request
        /// </summary>
        //public int? RequestFacilityGuarantorId { get; set; }
        //public virtual RequestFacilityGuarantor RequestFacilityGuarantor { get; set; }
        #endregion

        #region Buyer/Person
        /// <summary>
        /// if we will get buyer's/seller's balance using this property
        /// If the buyer is a real person fill this Property,else fill 'NULL'
        /// </summary>
        public Guid? UserId { get; set; }
        public virtual User Person { get; set; }
        #endregion

        #region Buyer/Organization/Company
        /// <summary>
        /// if we will get buyer's/seller's balance using this property
        /// If the buyer is a legal entity,else fill 'NULL'
        /// </summary>
        public int? OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
        #endregion

        #region Creator
        public Guid CreatorId { get; set; }

        public virtual User Creator { get; set; }
        #endregion

        public  virtual TransactionReason TransactionReason { get; set; }
    }

    public class TransactionConfiguration : BaseEntityTypeConfiguration<Transaction>
    {
        public override void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasOne(p => p.Creator).WithMany(p => p.CreatorTransactions).HasForeignKey(p => p.CreatorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            //builder.HasOne(p => p.Payment).WithOne(p => p.Transaction).HasForeignKey<Transaction>(p => p.PaymentId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.Person).WithMany(p => p.PersonTransactions).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Organization).WithMany(p => p.Transactions).HasForeignKey(p => p.OrganizationId).OnDelete(DeleteBehavior.Restrict);
            //builder.HasOne(p => p.RequestFacility).WithMany(p => p.Transactions).HasForeignKey(p => p.RequestFacilityId);
            //builder.HasOne(p => p.RequestFacilityGuarantor).WithMany(p => p.Transactions).HasForeignKey(p => p.RequestFacilityGuarantorId);
        }
    }
}
