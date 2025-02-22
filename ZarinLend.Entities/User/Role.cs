using Core.Entities.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public enum RoleEnum
    {
        /// <summary>
        /// کاربر سوپر ادمین زرین لند
        /// </summary>
        SuperAdmin = 1,
        /// <summary>
        /// کاربر ادمین زرین لند
        /// </summary>
        Admin = 2,
        /// <summary>
        /// کاربر بانک/لیزینگ-اقدام روی درخواست ها
        /// </summary>
        BankLeasing = 3,
        /// <summary>
        /// کاربر ادمین بانک/لیزینگ-دسترسی کامل،تخصیص درخواست به کارشناسان و سوپروایزر و امضآء
        /// </summary>
        AdminBankLeasing = 6,
        /// <summary>
        /// خریدار حقیقی/حقوقی
        /// </summary>
        Buyer = 4,
        /// <summary>
        /// فروشگاه
        /// </summary>
        Seller = 5,
        /// <summary>
        /// کاربر سرپرست بانک/لیزینگ(دسترسی هر کاری غیر از امضاء و کارهای مربوط به مدیر)
        /// </summary>
        SupervisorLeasing = 7,
        /// <summary>
        /// کارشناس زرین لند
        /// </summary>
        ZarinLendExpert = 8
    }
    public class Role : IdentityRole<Guid>, IEntity
    {
        public Role()
        {
            UserRoles = new HashSet<UserRole>();
            WorkFlowStepRoles = new HashSet<WorkFlowStepRole>();
        }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<WorkFlowStepRole> WorkFlowStepRoles { get; set; }
    }

    public class RoleConnfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(350);
            builder.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
        }
    }
}
