using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Core.Entities
{
    /// <summary>
    /// user inbox
    /// </summary>
    public class UserInbox : BaseEntity
    {
        public UserInbox()
        {
        }

        public string Message { get; set; }
        public bool Seen { get; set; }
        public DateTime? SeenDate { get; set; }

        #region User
        public Guid UserId { get; set; }

        public virtual User User { get; set; }
        #endregion

        #region User
        public Guid SenderId { get; set; }

        public virtual User Sender { get; set; }
        #endregion

    }

    public class UserInboxConfiguration : BaseEntityTypeConfiguration<UserInbox>
    {
        public override void Configure(EntityTypeBuilder<UserInbox> builder)
        {
            builder.HasOne(p => p.User).WithMany(p => p.UserInboxes).HasForeignKey(p => p.UserId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.Sender).WithMany(p => p.SenderUserInboxes).HasForeignKey(p => p.SenderId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.Property(p=>p.Message).HasMaxLength(500).IsRequired();
        }
    }
}
