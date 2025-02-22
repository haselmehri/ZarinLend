using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class Bank : BaseEntity
    {
        public Bank()
        {
            
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int Id { get => base.Id; set => base.Id = value; }

        public string Name { get; set; }
       
        public bool IsActive { get; set; }

        public virtual ICollection<UserIdentityDocument> UserIdentityDocuments { get; set; } = [];
    }

    public class BankConfiguration : BaseEntityTypeConfiguration<Bank>
    {
        public override void Configure(EntityTypeBuilder<Bank> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.IsActive).HasDefaultValue(true);
        }
    }
}
