using Core.Entities.Base;
using Core.Entities.Business.Payment;
using Core.Entities.Business.RequestFacility;
using Core.Entities.Business.Transaction;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Core.Entities
{
    /// <summary>
    /// اطلاعات هویتی خریدار حقوقی
    /// </summary>
    public class Organization : BaseEntity
    {
        public Organization()
        {
            
        }

        public long NationalId { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// تلفن های فروشگاه/لیزینگ
        /// </summary>
        public string? Tel { get; set; }
        public string? Address { get; set; }
        /// <summary>
        /// آدرس سایت فروشگاه
        /// </summary>
        public string? SiteUrl { get; set; }
        public bool IsActive { get; set; }

        #region OrganizationType        
        public short OrganizationTypeId { get; set; }
        public virtual OrganizationType OrganizationType { get; set; }
        #endregion

        public virtual ICollection<WorkFlow> WorkFlows { get; set; } = new HashSet<WorkFlow>();
        public virtual ICollection<FacilityType> FacilityTypes { get; set; } = new HashSet<FacilityType>();
        public virtual ICollection<RequestFacility> OrganizationRequestFacilities { get; set; } = new HashSet<RequestFacility>();
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; } = new HashSet<WalletTransaction>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
        public virtual ICollection<Person> People { get; set; } = new HashSet<Person>();
        public virtual ICollection<ApplicantValidationResult> ApplicantValidationResults { get; set; } = new HashSet<ApplicantValidationResult>();
        public virtual ICollection<VerifyResultExcel> VerifyResultExcels { get; set; } = new HashSet<VerifyResultExcel>();
        public virtual ICollection<Plan> Plans { get; set; } = new HashSet<Plan>();
        public virtual ICollection<PaymentInfo> PaymentInfos { get; set; } = new HashSet<PaymentInfo>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new HashSet<Invoice>();

    }

    public class OrganizationConfiguration : BaseEntityTypeConfiguration<Organization>
    {
        public override void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.HasOne(p => p.OrganizationType).WithMany(p => p.Organizations).HasForeignKey(p => p.OrganizationTypeId);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(250);
            builder.HasIndex(p => p.NationalId).IsUnique();
            builder.Property(p => p.Tel).HasMaxLength(50);
            builder.Property(p => p.Address).HasMaxLength(500);
        }
    }
}
