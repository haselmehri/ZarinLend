using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class JobTitle : BaseEntity
    {
        public JobTitle()
        {
            
        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int Id { get => base.Id; set => base.Id = value; }
        public string Title { get; set; }

        public bool IsActive { get; set; }
        public virtual ICollection<PersonJobInfo> PersonJobInfos { get; set; }
    }

    public class JobTitleConfiguration : IEntityTypeConfiguration<JobTitle>
    {
        public void Configure(EntityTypeBuilder<JobTitle> builder)
        {
            builder.Property(p => p.Title).IsRequired().HasMaxLength(250);
            builder.Property(p => p.IsActive).HasDefaultValue(true);
        }
    }
}
