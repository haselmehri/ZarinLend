using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Core.Entities
{
    public class UserBankAccount : BaseEntity
    {
        public UserBankAccount()
        {

        }

        public string? Deposit { get; set; }
        public string? CardNumber { get; set; }
        public string IBAN { get; set; }
        public string ClientId { get; set; }
        public string? DepositOwner { get; set; }
        public string? BankName { get; set; }

        /// <summary>
        /// 02 : حساب فعال است
        /// ‌03 : حساب مسدود با قابلیت واریز
        /// ‌04 : حساب مسدود بدون قابلیت واریز
        /// ‌05 : حساب راکد است
        /// ‌06 : بروز خطادر پاسخ دهی, شرح خطا در فیلد توضیحات است
        /// ‌07 : سایر موارد
        /// </summary>
        public string? DepositStatus { get; set; }

        //#region  Bank
        //public int? BankId { get; set; }
        //public virtual Bank Bank { get; set; }
        //#endregion  Bank

        #region NeginHubLog
        //public long NeginHubLogId { get; set; }
        //public virtual NeginHubLog NeginHubLog { get; set; }
        #endregion

        #region User
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        #endregion

        public bool IsConfirm { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class UserBankAccountConfiguration : BaseEntityTypeConfiguration<UserBankAccount>
    {
        public override void Configure(EntityTypeBuilder<UserBankAccount> builder)
        {
            builder.HasIndex(p => p.CardNumber).IsUnique();
            builder.HasIndex(p => p.IBAN).IsUnique();
            //builder.HasIndex(p => p.AccountNumber).IsUnique();
            builder.HasOne(p => p.User).WithMany(p => p.UserBankAccounts).HasForeignKey(p => p.UserId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            //builder.HasOne(p => p.NeginHubLog).WithOne(p => p.UserBankAccount).HasForeignKey<UserBankAccount>(p => p.NeginHubLogId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.Property(p => p.DepositOwner).HasMaxLength(500);
            builder.Property(p => p.DepositStatus).HasMaxLength(100);
            builder.Property(p => p.Deposit).HasMaxLength(20);
            builder.Property(p => p.IBAN).IsRequired().HasMaxLength(26);
            builder.Property(p => p.ClientId).IsRequired().HasMaxLength(64);
            builder.Property(p => p.CardNumber).HasMaxLength(16);
        }
    }
}
