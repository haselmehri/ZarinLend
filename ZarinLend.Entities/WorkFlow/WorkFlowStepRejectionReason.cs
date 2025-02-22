using Core.Entities.Base;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Core.Entities;

public class WorkFlowStepRejectionReason : BaseEntity
{
    #region RejectionReason
    public int RejectionReasonId { get; set; }
    public RejectionReason RejectionReason { get; set; }
    #endregion

    #region WorkFlowStep
    public int WorkFlowStepId { get; set; }
    public WorkFlowStep WorkFlowStep { get; set; }
    #endregion

    public bool IsActive { get; set; }

    public ICollection<RequestFacilityWorkFlowStepWorkFlowStepRejectionReason> RequestFacilityWorkFlowStepWorkFlowStepRejectionReasons { get; set; }

    public class WorkFlowStepRejectionReasonConfiguration : BaseEntityTypeConfiguration<WorkFlowStepRejectionReason>
    {
        public override void Configure(EntityTypeBuilder<WorkFlowStepRejectionReason> builder)
        {
            builder.HasOne(p => p.WorkFlowStep).WithMany(p => p.WorkFlowStepRejectionReasons).HasForeignKey(p => p.WorkFlowStepId);
            builder.HasOne(p => p.RejectionReason).WithMany(p => p.WorkFlowStepRejectionReasons).HasForeignKey(p => p.RejectionReasonId);
            builder.Property(p => p.IsActive).HasDefaultValue(true);
        }
    }
}