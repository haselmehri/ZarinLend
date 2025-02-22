using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class SalaryRange : BaseEntity
    {
        public SalaryRange()
        {
            
        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int Id { get => base.Id; set => base.Id = value; }
        public string Title { get; set; }

        public bool IsActive { get; set; }
        public virtual ICollection<PersonJobInfo> PersonJobInfos { get; set; }
    }

    public class SalaryRangeConfiguration : IEntityTypeConfiguration<SalaryRange>
    {
        public void Configure(EntityTypeBuilder<SalaryRange> builder)
        {
            builder.Property(p => p.Title).IsRequired().HasMaxLength(250);
            builder.Property(p => p.IsActive).HasDefaultValue(true);
        }
    }
}
