using Core.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Business.RequestFacility;

public class RejectionReason : BaseEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public override int Id { get => base.Id; set => base.Id = value; }

    public string Name { get; set; }

    public virtual ICollection<WorkFlowStepRejectionReason> WorkFlowStepRejectionReasons { get; set; }

    public class RejectionReasonConfiguration : BaseEntityTypeConfiguration<RejectionReason>
    {
        public override void Configure(EntityTypeBuilder<RejectionReason> builder)
        {
            builder.HasMany(p => p.WorkFlowStepRejectionReasons).WithOne(p => p.RejectionReason).HasForeignKey(p => p.RejectionReasonId);
        }
    }
}
