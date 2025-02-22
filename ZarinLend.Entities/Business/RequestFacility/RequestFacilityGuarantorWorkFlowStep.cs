using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Core.Entities.Business.RequestFacility
{
    public class RequestFacilityGuarantorWorkFlowStep : BaseEntity
    {
        public RequestFacilityGuarantorWorkFlowStep()
        {
            //RequestFacilityWorkFlowStepErrors = new HashSet<RequestFacilityWorkFlowStepError>();
        }
        public short? StatusId { get; set; }
        public Status Status { get; set; }
        public string? StatusDescription { get; set; }

        #region WorkFlowStep
        public int WorkFlowStepId { get; set; }
        public virtual WorkFlowStep WorkFlowStep { get; set; }
        #endregion

        #region RequestFacility Guarantor
        public int RequestFacilityGuarantorId { get; set; }
        public virtual RequestFacilityGuarantor RequestFacilityGuarantor { get; set; }
        #endregion

        #region Oprator
        public Guid OpratorId { get; set; }
        public virtual User Oprator { get; set; }
        #endregion

        //public virtual ICollection<RequestFacilityWorkFlowStepError> RequestFacilityWorkFlowStepErrors { get; set; }
    }

    public class RequestFacilityGuarantorWorkFlowStepConfiguration : BaseEntityTypeConfiguration<RequestFacilityGuarantorWorkFlowStep>
    {
        public override void Configure(EntityTypeBuilder<RequestFacilityGuarantorWorkFlowStep> builder)
        {
            builder.HasOne(p => p.RequestFacilityGuarantor).WithMany(p => p.RequestFacilityGuarantorWorkFlowSteps).HasForeignKey(p => p.RequestFacilityGuarantorId);
            builder.HasOne(p => p.WorkFlowStep).WithMany(p => p.RequestFacilityGuarantorWorkFlowSteps).HasForeignKey(p => p.WorkFlowStepId);
            builder.HasOne(p => p.Oprator).WithMany(p => p.RequestFacilityGuarantorWorkFlowSteps).HasForeignKey(p => p.OpratorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.Status).WithMany(p => p.RequestFacilityGuarantorWorkFlowSteps).HasForeignKey(p => p.StatusId);
            builder.Property(p => p.StatusDescription).HasMaxLength(1000);
        }
    }
}