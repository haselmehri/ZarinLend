using Core.Entities.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Core.Entities
{
    public class UserRole : IdentityUserRole<Guid>, IEntity
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }

    public class UserRoleConnfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasOne(p => p.User).WithMany(c => c.UserRoles).HasForeignKey(p => p.UserId);
            builder.HasOne(p => p.Role).WithMany(c => c.UserRoles).HasForeignKey(p => p.RoleId);
        }
    }
}
