using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class UserVAPID : BaseEntity
    {
        public UserVAPID()
        {
            UserNotifications=new HashSet<UserNotification>();
        }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        public string  Endpoint { get; set; }
        public string P256dh { get; set; }
        public string Auth { get; set; }
        public bool IsActive { get; set; }
        public string? Platform { get; set; }
        public string? AppVersion { get; set; }
        public string? AppCodeName { get; set; }
        public string? AppName { get; set; }
        public string? UserAgent { get; set; }
        public bool? IsMobile { get; set; }
        public string? OsName { get; set; }
        public virtual ICollection<UserNotification> UserNotifications { get; set; }
    }

    public class UserVAPIDConnfiguration : IEntityTypeConfiguration<UserVAPID>
    {
        public void Configure(EntityTypeBuilder<UserVAPID> builder)
        {
            builder.Property(p=>p.Endpoint).HasMaxLength(500).IsRequired();
            builder.Property(p=>p.Auth).HasMaxLength(500).IsRequired();
            builder.Property(p=>p.P256dh).HasMaxLength(500).IsRequired();
            builder.HasOne(p => p.User).WithMany(c => c.UserVAPIDs).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}