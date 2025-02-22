using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    /// <summary>
    /// اطلاعات این جدول توسط وب سرویس هاس سمات پر میشود-جزئیات چکهای برگشتی شخص
    /// </summary>
    public class SamatBackChequeDetail : BaseEntity
    {
        public SamatBackChequeDetail()
        {
            
        }
        public string? AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string? BackDate { get; set; }
        public string? Date { get; set; }
        public string? BankCode { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchDescription { get; set; }
        public string? Number { get; set; }

        #region 
        public int SamatBackChequeHeaderId { get; set; }
        public virtual SamatBackChequeHeader SamatBackChequeHeader { get; set; }
        #endregion
    }

    public class SamatBackChequeDetailConfiguration : BaseEntityTypeConfiguration<SamatBackChequeDetail>
    {
        public override void Configure(EntityTypeBuilder<SamatBackChequeDetail> builder)
        {
            builder.HasOne(p => p.SamatBackChequeHeader).WithMany(p => p.SamatBackChequeDetails).HasForeignKey(p => p.SamatBackChequeHeaderId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
