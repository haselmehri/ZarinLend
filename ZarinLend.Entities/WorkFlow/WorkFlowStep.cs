using Core.Entities.Base;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class WorkFlowStep : BaseEntity
    {
        public WorkFlowStep()
        {
            ApproveNextSteps = new HashSet<WorkFlowStep>();
            RejectNextSteps = new HashSet<WorkFlowStep>();
            ReturnToCorrectionNextSteps = new HashSet<WorkFlowStep>();
            WorkFlowStepRoles = new HashSet<WorkFlowStepRole>();
            RequestFacilityWorkFlowSteps = new HashSet<RequestFacilityWorkFlowStep>();
            RequestFacilityGuarantorWorkFlowSteps = new HashSet<RequestFacilityGuarantorWorkFlowStep>();
            WorkFlowStepErrors = new HashSet<WorkFlowStepError>();
            CurrentStepWorkFlowStepCorrections = new HashSet<WorkFlowStepCorrection>();
            ReturnToCorrectionNextStepWorkFlowStepCorrections = new HashSet<WorkFlowStepCorrection>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int Id { get => base.Id; set => base.Id = value; }
        /// <summary>
        /// step name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// step description
        /// </summary>
        public string? Description { get; set; }

        public bool IsActive { get; set; }
        public bool IsFirstStep { get; set; }
        public bool IsLastStep { get; set; }
        public bool IsApproveFinalStep { get; set; }
        public bool StepIsManual { get; set; }
        public bool CanBeIncomplete { get; set; }

        #region WorkFlow
        public int WorkFlowId { get; set; }
        public virtual WorkFlow WorkFlow { get; set; }
        #endregion

        #region Work Flow Form
        public WorkFlowFormEnum? WorkFlowFormId { get; set; }
        public virtual WorkFlowForm WorkFlowForm { get; set; }
        #endregion

        #region Approve Next Step
        public int? ApproveNextStepId { get; set; }
        public virtual WorkFlowStep ApproveNextStep { get; set; }
        public virtual ICollection<WorkFlowStep> ApproveNextSteps { get; set; }
        #endregion

        #region Reject Next Step
        public int? RejectNextStepId { get; set; }
        public virtual WorkFlowStep RejectNextStep { get; set; }
        public virtual ICollection<WorkFlowStep> RejectNextSteps { get; set; }
        #endregion

        #region Return to Correction Step
        public bool ConditionalOrOptionalCorrectionStep { get; set; }
        public int? ReturnToCorrectionNextStepId { get; set; }
        public virtual WorkFlowStep? ReturnToCorrectionNextStep { get; set; }
        public virtual ICollection<WorkFlowStep> ReturnToCorrectionNextSteps { get; set; }
        #endregion
        public virtual ICollection<WorkFlowStepRole> WorkFlowStepRoles { get; set; }
        public virtual ICollection<RequestFacilityWorkFlowStep> RequestFacilityWorkFlowSteps { get; set; }
        public virtual ICollection<RequestFacilityGuarantorWorkFlowStep> RequestFacilityGuarantorWorkFlowSteps { get; set; }
        public virtual ICollection<WorkFlowStepError> WorkFlowStepErrors { get; set; }
        public virtual ICollection<WorkFlowStepCorrection> CurrentStepWorkFlowStepCorrections { get; set; }
        public virtual ICollection<WorkFlowStepCorrection> ReturnToCorrectionNextStepWorkFlowStepCorrections { get; set; }

        public virtual ICollection<WorkFlowStepRejectionReason> WorkFlowStepRejectionReasons { get; set; }
    }

    public class WorkFlowStepConfiguration : BaseEntityTypeConfiguration<WorkFlowStep>
    {
        public override void Configure(EntityTypeBuilder<WorkFlowStep> builder)
        {
            builder.HasOne(p => p.WorkFlow).WithMany(p => p.WorkFlowSteps).HasForeignKey(p => p.WorkFlowId);
            builder.HasOne(p => p.WorkFlowForm).WithMany(p => p.WorkFlowSteps).HasForeignKey(p => p.WorkFlowFormId).IsRequired(false);
            builder.HasOne(p => p.ApproveNextStep).WithMany(p => p.ApproveNextSteps).HasForeignKey(p => p.ApproveNextStepId).IsRequired(false);
            builder.HasOne(p => p.RejectNextStep).WithMany(p => p.RejectNextSteps).HasForeignKey(p => p.RejectNextStepId).IsRequired(false);
            builder.HasOne(p => p.ReturnToCorrectionNextStep).WithMany(p => p.ReturnToCorrectionNextSteps).HasForeignKey(p => p.ReturnToCorrectionNextStepId).IsRequired(false);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(250);
            builder.Property(p => p.IsActive).HasDefaultValue(true);
            builder.Property(p => p.StepIsManual).HasDefaultValue(true);
            builder.Property(p => p.CanBeIncomplete).HasDefaultValue(false);
            builder.HasMany(p => p.WorkFlowStepRejectionReasons).WithOne(p => p.WorkFlowStep).HasForeignKey(p => p.WorkFlowStepId);
        }
    }
}
