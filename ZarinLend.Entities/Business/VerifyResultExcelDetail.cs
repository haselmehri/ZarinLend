using Core.Entities.Base;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Core.Entities
{
    public class VerifyResultExcelDetail : BaseEntity
    {
        public VerifyResultExcelDetail()
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
        public bool? FinalResult { get; set; }
        public string? Description { get; set; }

        #region VerifyResultExcel
        public int VerifyResultExcelId { get; set; }
        public virtual VerifyResultExcel VerifyResultExcel { get; set; }
        #endregion

        #region RequestFacility
        public int? RequestFacilityId { get; set; }
        public virtual RequestFacility RequestFacility { get; set; }
        #endregion

        #region Creator
        public Guid CreatorId { get; set; }
        public virtual User Creator { get; set; }
        #endregion
    }

    public class VerifyResultExcelDetailConfiguration : BaseEntityTypeConfiguration<VerifyResultExcelDetail>
    {
        public override void Configure(EntityTypeBuilder<VerifyResultExcelDetail> builder)
        {
            builder.HasOne(p => p.VerifyResultExcel).WithMany(p => p.VerifyResultExcelDetails).HasForeignKey(p => p.VerifyResultExcelId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.RequestFacility).WithMany(p => p.VerifyResultExcelDetails).HasForeignKey(p => p.RequestFacilityId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.Creator).WithMany(p => p.VerifyResultExcelDetails).HasForeignKey(p => p.CreatorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
