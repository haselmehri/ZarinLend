using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Core.Entities
{
    /// <summary>
    /// user web notification
    /// </summary>
    public class UserNotification : BaseEntity
    {
        public UserNotification()
        {
        }

        public string? Title { get; set; }
        public string Message { get; set; }
        public string? IconUrl { get; set; }
        public string? ImageUrl { get; set; }
        public string? Url { get; set; }
        public bool Seen { get; set; }
        public DateTime? SeenDate { get; set; }

        #region UserVAPID
        public int UserVapidId { get; set; }

        public virtual UserVAPID UserVAPID { get; set; }
        #endregion

        #region User
        public Guid SenderId { get; set; }

        public virtual User Sender { get; set; }
        #endregion

    }

    public class UserNotificationConfiguration : BaseEntityTypeConfiguration<UserNotification>
    {
        public override void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            builder.HasOne(p => p.UserVAPID).WithMany(p => p.UserNotifications).HasForeignKey(p => p.UserVapidId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.Sender).WithMany(p => p.UserNotifications).HasForeignKey(p => p.SenderId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.Property(p => p.Message).HasMaxLength(500).IsRequired();
        }
    }
}
