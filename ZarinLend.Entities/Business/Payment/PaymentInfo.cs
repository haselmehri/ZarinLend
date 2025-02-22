using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Core.Entities.Business.Payment
{
    /// <summary>
    /// 
    /// </summary>
    public class PaymentInfo : BaseEntity<Guid>
    {
        public PaymentInfo()
        {
            
        }
        public IpgType IpgType { get; set; }
        public long Amount { get; set; }
        public string CardNumber { get; set; }
        public string BuyerMobile { get; set; }
        public string? Otp { get; set; }
        public DateTime ExpireDate { get; set; }
        public long MessageId {  get; set; }
        public bool IsUsed { get; set; }

        #region Creator
        public Guid CreatorId { get; set; }

        public virtual User Creator { get; set; }
        #endregion

        #region Seller
        public Guid SellerId { get; set; }

        public virtual User Seller { get; set; }
        #endregion

        #region Buyer
        public Guid BuyerId { get; set; }

        public virtual User Buyer { get; set; }
        #endregion

        #region Organization
        public int OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
        #endregion

        public bool IsActive { get; set; }

        public  virtual ICollection<PaymentReason> PaymentReasons { get; set; } =  new HashSet<PaymentReason>();
    }

    public class PaymentLinkConfiguration : BaseEntityTypeConfiguration<PaymentInfo>
    {
        public override void Configure(EntityTypeBuilder<PaymentInfo> builder)
        {
            builder.HasOne(p => p.Organization).WithMany(p => p.PaymentInfos).HasForeignKey(p => p.OrganizationId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.Creator).WithMany(p => p.CreatorPaymentInfos).HasForeignKey(p => p.CreatorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.Seller).WithMany(p => p.SellerPaymentInfos).HasForeignKey(p => p.SellerId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.Buyer).WithMany(p => p.BuyerPaymentInfos).HasForeignKey(p => p.BuyerId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.Property(p => p.Otp).HasMaxLength(50);
            builder.Property(p => p.BuyerMobile).IsRequired().HasMaxLength(15);
            builder.Property(p => p.CardNumber).IsRequired().HasMaxLength(20);
        }
    }
}
