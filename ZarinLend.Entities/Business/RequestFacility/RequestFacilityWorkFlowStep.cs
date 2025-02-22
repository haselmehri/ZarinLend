using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Core.Entities.Business.RequestFacility
{
    public class RequestFacilityWorkFlowStep : BaseEntity
    {
        public RequestFacilityWorkFlowStep()
        {
        }
        public short? StatusId { get; set; }
        public Status Status { get; set; }
        public string? StatusDescription { get; set; }

        #region WorkFlowStep
        public int WorkFlowStepId { get; set; }
        public virtual WorkFlowStep WorkFlowStep { get; set; }
        #endregion

        #region Request Facility
        public int RequestFacilityId { get; set; }
        public virtual RequestFacility RequestFacility { get; set; }
        #endregion

        #region Oprator
        public Guid OpratorId { get; set; }
        public virtual User Oprator { get; set; }
        #endregion

        #region RequestFacilityWorkFlowStepRejectionReason
        public ICollection<RequestFacilityWorkFlowStepWorkFlowStepRejectionReason> RequestFacilityWorkFlowStepWorkFlowStepRejectionReasons { get; set; }
        #endregion
    }

    public class RequestFacilityWorkFlowStepConfiguration : BaseEntityTypeConfiguration<RequestFacilityWorkFlowStep>
    {
        public override void Configure(EntityTypeBuilder<RequestFacilityWorkFlowStep> builder)
        {
            builder.HasOne(p => p.RequestFacility).WithMany(p => p.RequestFacilityWorkFlowSteps).HasForeignKey(p => p.RequestFacilityId);
            builder.HasOne(p => p.WorkFlowStep).WithMany(p => p.RequestFacilityWorkFlowSteps).HasForeignKey(p => p.WorkFlowStepId);
            builder.HasOne(p => p.Oprator).WithMany(p => p.RequestFacilityWorkFlowSteps).HasForeignKey(p => p.OpratorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.Status).WithMany(p => p.RequestFacilityWorkFlowSteps).HasForeignKey(p => p.StatusId);
            builder.Property(p => p.StatusDescription).HasMaxLength(1000);
            builder.HasMany(p => p.RequestFacilityWorkFlowStepWorkFlowStepRejectionReasons).WithOne(p => p.RequestFacilityWorkFlowStep).HasForeignKey(p => p.RequestFacilityWorkFlowStepId);
        }
    }
}
