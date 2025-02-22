using Core.Entities.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Core.Entities
{
    public class WorkFlowStepRole : BaseEntity
    {
        public int WorkFlowStepId { get; set; }
        public virtual WorkFlowStep WorkFlowStep { get; set; }
        public Guid RoleId { get; set; }
        public virtual Role Role { get; set; }
    }

    public class WorkFlowStepRoleConnfiguration : IEntityTypeConfiguration<WorkFlowStepRole>
    {
        public void Configure(EntityTypeBuilder<WorkFlowStepRole> builder)
        {
            builder.HasOne(p => p.WorkFlowStep).WithMany(c => c.WorkFlowStepRoles).HasForeignKey(p => p.WorkFlowStepId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Role).WithMany(c => c.WorkFlowStepRoles).HasForeignKey(p => p.RoleId);
        }
    }
}
