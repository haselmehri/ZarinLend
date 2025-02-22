using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Entities.Business.RequestFacility;

public class RequestFacilityWorkFlowStepWorkFlowStepRejectionReason : BaseEntity<short>
{
    #region RequestFacilityWorkFlowStep
    public int RequestFacilityWorkFlowStepId { get; set; }
    public RequestFacilityWorkFlowStep RequestFacilityWorkFlowStep { get; set; }
    #endregion

    #region RejectionReason
    public int WorkFlowStepRejectionReasonId { get; set; }
    public WorkFlowStepRejectionReason WorkFlowStepRejectionReason { get; set; }
    #endregion

    public class RequestFacilityWorkFlowStepWorkFlowStepRejectionReasonConfiguration : BaseEntityTypeConfiguration<RequestFacilityWorkFlowStepWorkFlowStepRejectionReason>
    {
        public override void Configure(EntityTypeBuilder<RequestFacilityWorkFlowStepWorkFlowStepRejectionReason> builder)
        {
            builder.HasOne(p => p.RequestFacilityWorkFlowStep).WithMany(p => p.RequestFacilityWorkFlowStepWorkFlowStepRejectionReasons).HasForeignKey(p => p.RequestFacilityWorkFlowStepId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.WorkFlowStepRejectionReason).WithMany(p => p.RequestFacilityWorkFlowStepWorkFlowStepRejectionReasons).HasForeignKey(p => p.WorkFlowStepRejectionReasonId);

            builder.Property("CreatedDate").HasDefaultValueSql("GetDate()");
        }
    }
}
