using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Core.Entities.Business.Transaction
{
    public enum WalletTransactionTypeEnum
    {
        /// <summary>
        /// برداشت
        /// </summary>
        Withdraw = 1,

        /// <summary>
        /// واریز
        /// </summary>
        Deposit = 2,

        /// <summary>
        /// کارمزد
        /// </summary>
        Fee = 3
    }
    public class WalletTransaction : BaseEntity<long>
    {
        public WalletTransaction()
        {

        }
        public long Amount { get; set; }
        public string? Description { get; set; }
        public WalletTransactionTypeEnum WalletTransactionType { get; set; }

        #region RequestFacility
        /// <summary>
        /// This property indicates that the buyer's credit has been increased upon request
        /// </summary>
        public int? RequestFacilityId { get; set; }
        public virtual RequestFacility.RequestFacility RequestFacility { get; set; }
        #endregion

        #region Buyer/Person
        /// <summary>
        /// if we will get buyer's/seller's balance using this property
        /// If the buyer is a real person fill this Property,else fill 'NULL'
        /// </summary>
        public Guid? UserId { get; set; }
        public virtual User User { get; set; }
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

        #region Parent Wallet Transaction
        public long? ParentId { get; set; }
        public virtual WalletTransaction Parent { get; set; }
        public virtual WalletTransaction Child { get; set; }
        #endregion
        public virtual ICollection<Invoice> Invoices { get; set; }
    }

    public class WalletTransactionConfiguration : BaseEntityTypeConfiguration<WalletTransaction>
    {
        public override void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            builder.HasOne(p => p.Creator).WithMany(p => p.CreatorWalletTransactions).HasForeignKey(p => p.CreatorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.User).WithMany(p => p.PersonWalletTransactions).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Organization).WithMany(p => p.WalletTransactions).HasForeignKey(p => p.OrganizationId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.RequestFacility).WithOne(p => p.WalletTransaction).HasForeignKey<WalletTransaction>(p => p.RequestFacilityId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Parent).WithOne(p => p.Child).HasForeignKey<WalletTransaction>(p => p.ParentId);
        }
    }
}
