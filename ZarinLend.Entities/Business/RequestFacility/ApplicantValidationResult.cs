using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Core.Entities.Business.RequestFacility
{
    /// <summary>
    /// استعلامات اعتبارسنجی درخواست دهنده وام از جمله استعلام کد پستی،موبایل... در این جدول ذخیره میشود
    /// </summary>
    public class ApplicantValidationResult : BaseEntity
    {
        public ApplicantValidationResult()
        {

        }
        /// <summary>
        /// استعلام ثبت احوال
        /// </summary>
        public bool? CivilRegistryInquiry { get; set; }

        /// <summary>
        /// استعلام چک برگشتی
        /// </summary>
        public bool? ReturnedCheckInquiry { get; set; }

        /// <summary>
        /// استعلام تسهیلات
        /// </summary>
        public bool? FacilityInquiry { get; set; }

        /// <summary>
        /// استعلام کد پستی
        /// </summary>
        public bool? PostalCodeInquiry { get; set; }

        /// <summary>
        /// استعلام تحریم شورا امنیت
        /// </summary>
        public bool? SecurityCouncilSanctionsInquiry { get; set; }

        /// <summary>
        /// استعلام شاهکار
        /// </summary>
        public bool? ShahkarInquiry { get; set; }

        /// <summary>
        /// استعلام نظام وظیفه
        /// </summary>
        public bool? MilitaryInquiry { get; set; }

        /// <summary>
        /// استعلام لیست سیاه
        /// </summary>
        public bool? BlackListInquiry { get; set; }

        public string? Description { get; set; }

        #region Organization/ لیزینگی/بانک که میخواهد از آن وام بگیرد این فرم را پر میکند
        public int OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
        #endregion

        #region RequestFacility
        public int RequestFacilityId { get; set; }
        public virtual RequestFacility RequestFacility { get; set; }
        #endregion

        #region Creator
        public Guid CreatorId { get; set; }
        public virtual User Creator { get; set; }
        #endregion
    }

    public class ApplicantValidationResultConfiguration : BaseEntityTypeConfiguration<ApplicantValidationResult>
    {
        public override void Configure(EntityTypeBuilder<ApplicantValidationResult> builder)
        {
            builder.HasOne(p => p.RequestFacility).WithMany(p => p.ApplicantValidationResults).HasForeignKey(p => p.RequestFacilityId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Organization).WithMany(p => p.ApplicantValidationResults).HasForeignKey(p => p.OrganizationId);
            builder.HasOne(p => p.Creator).WithMany(p => p.ApplicantValidationResults).HasForeignKey(p => p.CreatorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.Property(p => p.Description).HasMaxLength(2000);
        }
    }
}
