using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    /// <summary>
    /// اطلاعات این جدول توسط وب سرویس هاس سمات پر میشود-جزئیات تسهیلات شخص
    /// </summary>
    public class SamatFacilityDetail : BaseEntity
    {
        public SamatFacilityDetail()
        {

        }
        public string? BranchDescription { get; set; }
        public string? FacilityBankCode { get; set; }
        public string? FacilityBranchCode { get; set; }
        public string? FacilityBranch { get; set; }
        public decimal FacilityAmountOrginal { get; set; }
        public decimal FacilityBenefitAmount { get; set; }
        public decimal FacilityDebtorTotalAmount { get; set; }
        public decimal FacilityDeferredAmount { get; set; }
        public decimal FacilityPastExpiredAmount { get; set; }
        public decimal FacilitySuspiciousAmount { get; set; }
        public string? FacilitySetDate { get; set; }
        public string? FacilityEndDate { get; set; }
        #region 
        public int SamatFacilityHeaderId { get; set; }
        public virtual SamatFacilityHeader SamatFacilityHeader { get; set; }
        #endregion

        //#region Creator
        //public Guid CreatorId { get; set; }
        //public virtual User Creator { get; set; }
        //#endregion
    }

    public class SamatFacilityDetailConfiguration : BaseEntityTypeConfiguration<SamatFacilityDetail>
    {
        public override void Configure(EntityTypeBuilder<SamatFacilityDetail> builder)
        {
            builder.HasOne(p => p.SamatFacilityHeader).WithMany(p => p.SamatFacilityDetails).HasForeignKey(p => p.SamatFacilityHeaderId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            //builder.HasOne(p => p.User).WithMany(p => p.UserSamatSummaryFacilityResults).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
