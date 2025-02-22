using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Core.Entities
{
    public enum SmsStatus
    {

    }
    /// <summary>
    /// user sms
    /// </summary>
    public class UserSms : BaseEntity
    {
        public UserSms()
        {
        }

        public string? Mobile { get; set; }
        public string Message { get; set; }
        public long MessageId { get; set; }
        public SmsStatus? SmsStatus { get; set; }

        #region User
        public Guid UserId { get; set; }

        public virtual User User { get; set; }
        #endregion

        #region User
        public Guid SenderId { get; set; }

        public virtual User Sender { get; set; }
        #endregion
    }

    public class UserSmsConfiguration : BaseEntityTypeConfiguration<UserSms>
    {
        public override void Configure(EntityTypeBuilder<UserSms> builder)
        {
            builder.HasOne(p => p.User).WithMany(p => p.UserSms).HasForeignKey(p => p.UserId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.Sender).WithMany(p => p.SenderUserSms).HasForeignKey(p => p.SenderId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.Property(p => p.Message).HasMaxLength(500).IsRequired();
            builder.Property(p => p.Mobile).HasMaxLength(15);
        }
    }
}
