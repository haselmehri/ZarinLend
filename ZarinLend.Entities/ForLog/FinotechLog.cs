using Core.Entities.Base;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Core.Entities
{
    public enum MethodType
    {
        Get = 1,
        Post = 2
    }
    public enum FinotechServiceName
    {
        ChargeCard,
        CifInquiry,
        CardToIban,
        AccountStatement,
        /// <summary>
        /// درخواست صدور سفته
        /// </summary>
        PromissoryPublishRequest,
        /// <summary>
        /// ثبت درخواست امضا دیجیتال توسط کاربر نهایی
        /// </summary>
        PromissorySignRequest,
        /// <summary>
        /// نهایی کردن سفته الکترونیک
        /// </summary>
        PromissoryFinalize,
        /// <summary>
        /// استعلام وضعیت امضا کاربر نهایی
        /// </summary>
        StatusInquiry,
        /// <summary>
        /// حذف سفته الکترونیک
        /// </summary>
        PromissoryDelete,
        /// <summary>
        /// دریافت سند امضا شده
        /// </summary>
        PromissorySignedDocument,
        /// <summary>
        /// استعلام پیش نویس سفته الکترونیک
        /// </summary>
        PromissoryPublishRequestInquiry
    }
    public class FinotechLog : BaseEntity<long>
    {
        public FinotechLog()
        {

        }
        #region RequestFacility
        public int? RequestFacilityId { get; set; }
        public virtual RequestFacility RequestFacility { get; set; }
        #endregion
        public MethodType MethodType { get; set; }
        public string TrackId { get; set; }
        public string ServiceName { get; set; }
        public string Url { get; set; }
        public string? Body { get; set; }
        public string? Result { get; set; }
        public string? Curl { get; set; }

        #region Oprator
        public Guid? OpratorId { get; set; }
        public virtual User Oprator { get; set; }
        #endregion
    }

    public class FinotechLogConfiguration : BaseEntityTypeConfiguration<FinotechLog>
    {
        public override void Configure(EntityTypeBuilder<FinotechLog> builder)
        {
            builder.Property(p => p.Url).IsRequired().HasMaxLength(1000);
            builder.Property(p => p.TrackId).IsRequired().HasMaxLength(50);
            builder.Property(p => p.ServiceName).IsRequired().HasMaxLength(100);
            builder.HasOne(p => p.RequestFacility).WithMany(p => p.FinotechLogs).HasForeignKey(p => p.RequestFacilityId);
            builder.HasOne(p => p.Oprator).WithMany(p => p.FinotechLogs).HasForeignKey(p => p.OpratorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
