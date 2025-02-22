using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public enum OrganizationTypeEnum
    {
        /// <summary>
        ///  خریداران/مشتریان حقیقی
        /// </summary>
        Company = 1,
        ///// <summary>
        ///// خریداران/مشتریان حقیقی
        ///// </summary>
        //Person = 2,
        /// <summary>
        /// فروشگاه-فروشنده
        /// </summary>
        Shop = 3,
        /// <summary>
        /// لیزینگ/بانک
        /// </summary>
        BankLeasing = 4,
        /// <summary>
        /// هیراد-زرین لند
        /// </summary>
        ZarinLend = 5,
    }
    public class OrganizationType : BaseEntity<short>
    {
        public OrganizationType()
        {
            Organizations = new HashSet<Organization>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override short Id { get => base.Id; set => base.Id = value; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<Organization> Organizations { get; set; }
    }

    public class OrganizationTypeConfiguration : BaseEntityTypeConfiguration<OrganizationType>
    {
        public override void Configure(EntityTypeBuilder<OrganizationType> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.IsActive).HasDefaultValue(true);
        }
    }
}
