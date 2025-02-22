using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    /// <summary>
    /// *زمانی که برای مرحله ای بخواهیم بصورت شرطی یا انتخابی 'مرحله برگشت جهت اصلاح' را مشخص کنیم ،در این جدول باید تعریف کنیم
    /// همچنین زمانی که 'برگشت جهت اصلاح' برای یک مرحله ای بیش از یک مورد باشد در این جدول باید تعریف شود
    /// </summary>
    public class WorkFlowStepCorrection : BaseEntity
    {
        public WorkFlowStepCorrection()
        {

        }
        /// <summary>
        /// step name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// step description
        /// </summary>
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        #region Work Flow Form
        public WorkFlowFormEnum WorkFlowFormId { get; set; }
        public virtual WorkFlowForm WorkFlowForm { get; set; }
        #endregion

        #region Current Step
        public int CurrentStepId { get; set; }
        public virtual WorkFlowStep CurrentStep { get; set; }
        #endregion

        #region Return to Correction Step
        public int ReturnToCorrectionNextStepId { get; set; }
        public virtual WorkFlowStep ReturnToCorrectionNextStep { get; set; }
        #endregion
    }

    public class WorkFlowStepCorrectionConfiguration : BaseEntityTypeConfiguration<WorkFlowStepCorrection>
    {
        public override void Configure(EntityTypeBuilder<WorkFlowStepCorrection> builder)
        {
            builder.HasOne(p => p.WorkFlowForm).WithMany(p => p.WorkFlowStepCorrections).HasForeignKey(p => p.WorkFlowFormId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.CurrentStep).WithMany(p => p.CurrentStepWorkFlowStepCorrections).HasForeignKey(p => p.CurrentStepId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.ReturnToCorrectionNextStep).WithMany(p => p.ReturnToCorrectionNextStepWorkFlowStepCorrections).HasForeignKey(p => p.ReturnToCorrectionNextStepId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(250);
            builder.Property(p => p.IsActive).HasDefaultValue(true);
        }
    }
}
