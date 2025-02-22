using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class WorkFlowDefaultError : BaseEntity
    {
        public WorkFlowDefaultError()
        {
            WorkFlowStepErrors = new HashSet<WorkFlowStepError>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// خطا/اصلاح برای کدام فیلد هست؟
        /// </summary>
        public string? ErrorField { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<WorkFlowStepError> WorkFlowStepErrors { get; set; }
    }

    public class WorkFlowDefaultErrorConfiguration : BaseEntityTypeConfiguration<WorkFlowDefaultError>
    {
        public override void Configure(EntityTypeBuilder<WorkFlowDefaultError> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(250);
            builder.Property(p => p.Message).IsRequired().HasMaxLength(500);
            builder.Property(p => p.IsActive).HasDefaultValue(true);
        }
    }
}
