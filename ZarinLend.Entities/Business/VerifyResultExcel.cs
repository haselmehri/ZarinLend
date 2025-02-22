using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class VerifyResultExcel : BaseDocument
    {
        public VerifyResultExcel()
        {
            VerifyResultExcelDetails = new HashSet<VerifyResultExcelDetail>();
        }

        public int OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
        public int RowCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int UnknownCount { get; set; }

        #region Creator
        public Guid CreatorId { get; set; }
        public virtual User Creator { get; set; }
        #endregion
        public virtual ICollection<VerifyResultExcelDetail> VerifyResultExcelDetails { get; set; }
    }

    public class VerifyResultExcelConfiguration : BaseEntityTypeConfiguration<VerifyResultExcel>
    {
        public override void Configure(EntityTypeBuilder<VerifyResultExcel> builder)
        {
            builder.HasOne(p => p.Organization).WithMany(p => p.VerifyResultExcels).HasForeignKey(p => p.OrganizationId);
            builder.HasOne(p => p.Creator).WithMany(p => p.VerifyResultExcels).HasForeignKey(p => p.CreatorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.Property(p => p.FilePath).IsRequired();
        }
    }
}