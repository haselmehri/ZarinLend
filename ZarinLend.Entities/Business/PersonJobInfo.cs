using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Entities
{
    public class PersonJobInfo : BaseEntity
    {
        public PersonJobInfo()
        {
            
        }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        #region Person
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        #endregion

        #region Salary Range
        public int SalaryRangeId { get; set; }
        public virtual SalaryRange SalaryRange { get; set; }
        #endregion

        #region Salary Range
        public int JobTitleId { get; set; }
        public virtual JobTitle JobTitle { get; set; }
        #endregion

        public bool IsActive { get; set; }
    }

    public class PersonJobInfoConfiguration : IEntityTypeConfiguration<PersonJobInfo>
    {
        public void Configure(EntityTypeBuilder<PersonJobInfo> builder)
        {
            builder.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Address).IsRequired().HasMaxLength(500);
            builder.Property(p => p.IsActive).HasDefaultValue(true);
            builder.HasOne(p => p.Person).WithMany(p => p.PersonJobInfos).HasForeignKey(p => p.PersonId);
            builder.HasOne(p => p.SalaryRange).WithMany(p => p.PersonJobInfos).HasForeignKey(p => p.SalaryRangeId);
            builder.HasOne(p => p.JobTitle).WithMany(p => p.PersonJobInfos).HasForeignKey(p => p.JobTitleId);
        }
    }
}
