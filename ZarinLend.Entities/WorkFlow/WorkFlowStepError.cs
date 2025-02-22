using Core.Entities.Base;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class WorkFlowStepError : BaseEntity
    {
        public WorkFlowStepError()
        {
            RequestFacilityErrors = new HashSet<RequestFacilityError>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int Id { get => base.Id; set => base.Id = value; }

        #region WorkFlowStep
        public int WorkFlowStepId { get; set; }
        public virtual WorkFlowStep WorkFlowStep { get; set; }
        #endregion

        #region WorkFlowDefaultError
        public int WorkFlowDefaultErrorId { get; set; }
        public virtual WorkFlowDefaultError WorkFlowDefaultError { get; set; }
        #endregion

        public virtual ICollection<RequestFacilityError> RequestFacilityErrors { get; set; }
    }

    public class WorkFlowStepErrorConfiguration : BaseEntityTypeConfiguration<WorkFlowStepError>
    {
        public override void Configure(EntityTypeBuilder<WorkFlowStepError> builder)
        {
            builder.HasOne(p => p.WorkFlowStep).WithMany(p => p.WorkFlowStepErrors).HasForeignKey(p => p.WorkFlowStepId);
            builder.HasOne(p => p.WorkFlowDefaultError).WithMany(p => p.WorkFlowStepErrors).HasForeignKey(p => p.WorkFlowDefaultErrorId);
        }
    }
}
