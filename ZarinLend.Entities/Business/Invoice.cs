using Core.Entities.Base;
using Core.Entities.Business.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public enum InvoiceStatus
    {
        [Display(Name ="ثبت اولیه")]
        Register = 1,

        [Display(Name = "بارگذاری فاکتور")]
        UploadInvoice = 2,

        [Display(Name = "در انتظار تایید زرین لند")]
        WaitingVerify = 3,

        [Display(Name = "رد شده")]
        Reject = 4,

        [Display(Name = "تایید نهایی")]
        Approve = 5
    }
    /// <summary>
    /// اطلاعات این جدول توسط فروشنده پر میشود
    /// </summary>
    public class Invoice : BaseEntity
    {
        public Invoice()
        {
            InvoiceDocuments = new HashSet<InvoiceDocument>();
        }

        public InvoiceStatus Status { get; set; }
        /// <summary>
        /// invoice number
        /// </summary>
        public string? Number { get; set; }
        public long Amount { get; set; }
        public string? Description { get; set; }

        #region WalletTransaction
        /// <summary>
        /// if we will get buyer's balance using this property
        /// </summary>
        public long? WalletTransactionId { get; set; }
        public virtual WalletTransaction WalletTransaction { get; set; }
        #endregion

        #region Creator
        public Guid CreatorId { get; set; }

        public virtual User Creator { get; set; }
        #endregion

        #region Buyer : someone who made a purchase
        public Guid BuyerId { get; set; }

        public virtual User Buyer { get; set; }
        #endregion

        #region if 'Seller' is a person this property have to filled
        public Guid SellerId { get; set; }

        public virtual User Seller { get; set; }
        #endregion

        #region Shop/Organization : if 'Seller' is a 'Legal person' this property have to filled

        public int OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }

        #endregion Shop/Organization

        #region GlobalSetting
        public int GlobalSettingId { get; set; }
        public virtual GlobalSetting GlobalSetting { get; set; }
        #endregion

        public virtual ICollection<InvoiceDocument> InvoiceDocuments { get; set; }
    }

    public class InvoiceConfiguration : BaseEntityTypeConfiguration<Invoice>
    {
        public override void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasOne(p => p.GlobalSetting).WithMany(p => p.Invoices).HasForeignKey(p => p.GlobalSettingId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.Buyer).WithMany(p => p.BuyerInvoices).HasForeignKey(p => p.BuyerId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.Creator).WithMany(p => p.CreatorInvoices).HasForeignKey(p => p.CreatorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.Seller).WithMany(p => p.SellerInvoices).HasForeignKey(p => p.SellerId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.WalletTransaction).WithMany(p => p.Invoices).HasForeignKey(p => p.WalletTransactionId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Organization).WithMany(p => p.Invoices).HasForeignKey(p => p.OrganizationId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(p => p.Description).HasMaxLength(500);
        }
    }
}
