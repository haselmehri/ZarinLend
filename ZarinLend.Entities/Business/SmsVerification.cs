using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Core.Entities
{
    /// <summary>
    /// پیامکهای اعتبار سنجی در اینجا ذخیره میشود
    /// </summary>
    public class SmsVerification : BaseEntity
    {
        public SmsVerification()
        {

        }

        public long MessageId { get; set; }
        public string? Message { get; set; }
        public string? MessageTemplate { get; set; }
        /// <summary>
        /// VerficationCode
        /// </summary>
        public string? VerficationCode { get; set; }
        public string? Token { get; set; }
        public string? Token2 { get; set; }      
        public string? Token3 { get; set; }
        public int Status { get; set; }
        public string? StatusText { get; set; }
        public string? Sender { get; set; }
        public string? Receptor { get; set; }
        public int Cost { get; set; }
        public int ReturnCode { get; set; }
        public string? ReturnMessage { get; set; }
        public bool IsUsed { get; set; }
        public long Amount { get; set; }
        public DateTime ExpireTime { get; set; }

        #region SenderId(Seller)        
        public Guid SenderId { get; set; }
        public virtual User SenderUser { get; set; }
        #endregion

        #region ReceptorId(Buyer)        
        public Guid ReceptorId { get; set; }
        public virtual User ReceptorUser { get; set; }
        #endregion
    }

    public class SmsVerificationConfiguration : BaseEntityTypeConfiguration<SmsVerification>
    {
        public override void Configure(EntityTypeBuilder<SmsVerification> builder)
        {
            builder.HasOne(p => p.SenderUser).WithMany(p => p.SenderSmsVerifications).HasForeignKey(p => p.SenderId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.ReceptorUser).WithMany(p => p.ReceptorSmsVerifications).HasForeignKey(p => p.ReceptorId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
