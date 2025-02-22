using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Core.Entities
{
    /// <summary>
    /// طرح ها
    /// </summary>
    public class Plan : BaseEntity
    {
        public Plan()
        {
            PlanMembers = new HashSet<PlanMember>();
            PlanFiles = new HashSet<PlanFile>();

        }

        /// <summary>
        /// plan name/title
        /// </summary>
        public string Name { get; set; }
        public long? FacilityAmount { get; set; }
        public long AmountWaranty { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        #region GlobalSetting
        public int GlobalSettingId { get; set; }
        public virtual GlobalSetting GlobalSetting { get; set; }
        #endregion

        #region Organization
        public int? OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
        #endregion

        #region FacilityType
        public int FacilityTypeId { get; set; }
        public virtual FacilityType FacilityType { get; set; }
        #endregion

        #region Import Result
        public bool? ImportDone { get; set; }

        #endregion Import Result

        public virtual ICollection<PlanMember> PlanMembers { get; set; }
        public virtual ICollection<PlanFile> PlanFiles { get; set; }
    }

    public class PlanConfiguration : BaseEntityTypeConfiguration<Plan>
    {
        public override void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.HasOne(p => p.Organization).WithMany(p => p.Plans).HasForeignKey(p => p.OrganizationId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.FacilityType).WithMany(p => p.Plans).HasForeignKey(p => p.FacilityTypeId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.GlobalSetting).WithMany(p => p.Plans).HasForeignKey(p => p.GlobalSettingId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.Property(p => p.Name).IsRequired().HasMaxLength(250);
            builder.Property(p => p.Description).HasMaxLength(1000);
        }
    }
}
