using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public enum WorkFlowEnum
    {
        RequestFacility = 3,
        RegisterGuarantor = 2,
        RequestFacility_OldVersion = 1
    }
    public class WorkFlow : BaseEntity<int>
    {
        public WorkFlow()
        {
            //WorkFlowSteps = new HashSet<WorkFlowStep>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int Id { get => base.Id; set => base.Id = value; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        //public long MaxAmount { get; set; }
        #region Organization
        /// <summary>
        /// Which leasing is this workflow for?
        /// null : for all leasing
        /// </summary>
        public int? OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
        #endregion
        public virtual ICollection<WorkFlowStep> WorkFlowSteps { get; set; } = new HashSet<WorkFlowStep>();
    }

    public class WorkFlowConfiguration : BaseEntityTypeConfiguration<WorkFlow>
    {
        public override void Configure(EntityTypeBuilder<WorkFlow> builder)
        {
            builder.HasOne(p => p.Organization).WithMany(p => p.WorkFlows).HasForeignKey(p => p.OrganizationId);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(250);
            builder.Property(p => p.IsActive).HasDefaultValue(true);
        }
    }
}
