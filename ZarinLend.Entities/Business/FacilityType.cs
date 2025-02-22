using Core.Entities.Base;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Core.Entities
{
    /// <summary>
    /// نوع اعتبار/مدت باز پرداخت/کدام لیزینگ؟
    /// </summary>
    public class FacilityType : BaseEntity
    {
        public FacilityType()
        {
            RequestFacilities = new HashSet<RequestFacility>();
            Plans = new HashSet<Plan>();
        }

        /// <summary>
        /// تعداد ماه برای باز پرداخت تسهیلات
        /// </summary>
        public int MonthCount { get; set; }
        public string MonthCountTitle { get; set; }
        /// <summary>
        /// کارمزد/درصد
        /// </summary>
        public long Fee { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        #region Organization
        public int OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
        #endregion

        public virtual ICollection<RequestFacility> RequestFacilities { get; set; }
        public virtual ICollection<Plan> Plans { get; set; }
    }

    public class FacilityTypeConfiguration : BaseEntityTypeConfiguration<FacilityType>
    {
        public override void Configure(EntityTypeBuilder<FacilityType> builder)
        {
            builder.HasOne(p => p.Organization).WithMany(p => p.FacilityTypes).HasForeignKey(p => p.OrganizationId);
            builder.Property(p => p.MonthCountTitle).IsRequired().HasMaxLength(250);
            builder.Property(p => p.Description).HasMaxLength(250);
            builder.Property(p => p.IsActive).HasDefaultValue(true);
        }
    }
}
