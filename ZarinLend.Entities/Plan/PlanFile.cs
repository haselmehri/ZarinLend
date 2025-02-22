using Core.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Entities
{
    public class PlanFile : BaseDocument
    {
        public PlanFile()
        {

        }

        #region Plan
        public int PlanId { get; set; }
        public virtual Plan Plan { get; set; }
        #endregion
    }

    public class PlanFileConfiguration : BaseEntityTypeConfiguration<PlanFile>
    {
        public override void Configure(EntityTypeBuilder<PlanFile> builder)
        {
            builder.HasOne(p => p.Plan).WithMany(p => p.PlanFiles).HasForeignKey(p => p.PlanId);
            builder.Property(p=>p.FilePath).IsRequired();
        }
    }
}
