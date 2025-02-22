using Core.Entities.Base;
using Core.Entities.Business.Payment;
using Core.Entities.Business.RequestFacility;
using Core.Entities.Business.Transaction;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class User : IdentityUser<Guid>, IEntity
    {
        public User()
        {
        }
        public string? Otp { get; set; }
        public DateTime? OtpStartTime { get; set; }
        public DateTime? OtpExpireTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        #region Person
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }
        #endregion

        public virtual ICollection<UserBankAccount> UserBankAccounts { get; set; } = new HashSet<UserBankAccount>();
        public virtual ICollection<UserVAPID> UserVAPIDs { get; set; } = new HashSet<UserVAPID>();
        public virtual ICollection<UserInbox> UserInboxes { get; set; } =new HashSet<UserInbox>();
        public virtual ICollection<UserInbox> SenderUserInboxes { get; set; } = new HashSet<UserInbox>();
        public virtual ICollection<UserSms> UserSms { get; set; } =new HashSet<UserSms>();
        public virtual ICollection<UserNotification> UserNotifications { get; set; } = new HashSet<UserNotification>();
        public virtual ICollection<UserSms> SenderUserSms { get; set; } =new HashSet<UserSms>();
        public virtual ICollection<GlobalSetting> CreatorGlobalSettings { get; set; } = new HashSet<GlobalSetting>();
        public virtual ICollection<GlobalSetting> UpdaterGlobalSettings { get; set; } =new HashSet<GlobalSetting>();
        public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
        public virtual ICollection<RequestFacility> RequestFacilityBuyers { get; set; }=new HashSet<RequestFacility>();
        public virtual ICollection<RequestFacility> OperatorRequestFacilities { get; set; } = new HashSet<RequestFacility>();
        public virtual ICollection<RequestFacilityWorkFlowStep> RequestFacilityWorkFlowSteps { get; set; } = new HashSet<RequestFacilityWorkFlowStep>();
        public virtual ICollection<RequestFacilityGuarantorWorkFlowStep> RequestFacilityGuarantorWorkFlowSteps { get; set; } = new HashSet<RequestFacilityGuarantorWorkFlowStep>();
        public virtual ICollection<WalletTransaction> CreatorWalletTransactions { get; set; } = new HashSet<WalletTransaction>();
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
        public virtual ICollection<Transaction> CreatorTransactions { get; set; } = new HashSet<Transaction>();
        public virtual ICollection<WalletTransaction> PersonWalletTransactions { get; set; } =new HashSet<WalletTransaction>(); 
        public virtual ICollection<Transaction> PersonTransactions { get; set; } = new HashSet<Transaction>();
        public virtual ICollection<Invoice> BuyerInvoices { get; set; } = new HashSet<Invoice>();  
        public virtual ICollection<Invoice> CreatorInvoices { get; set; } = new HashSet<Invoice>();  
        public virtual ICollection<Invoice> SellerInvoices { get; set; } = new HashSet<Invoice>();  
        public virtual ICollection<UserIdentityDocument> UserIdentityDocuments { get; set; } = new HashSet<UserIdentityDocument>(); 
        public virtual ICollection<SamatFacilityHeader> CreatorSamatFacilityHeaders { get; set; } = new HashSet<SamatFacilityHeader>();
        public virtual ICollection<SamatBackChequeHeader> CreatorSamatBackChequeHeaders { get; set; } = new HashSet<SamatBackChequeHeader>(); 
        public virtual ICollection<SmsVerification> SenderSmsVerifications { get; set; } = new HashSet<SmsVerification>(); 
        public virtual ICollection<SmsVerification> ReceptorSmsVerifications { get; set; } = new HashSet<SmsVerification>(); 
        public virtual ICollection<IranCreditScoring> IranCreditScorings { get; set; } = new HashSet<IranCreditScoring>(); 
        public virtual ICollection<ApplicantValidationResult> ApplicantValidationResults { get; set; } = new HashSet<ApplicantValidationResult>(); 
        public virtual ICollection<RequestFacilityInsuranceIssuance> RequestFacilityInsuranceIssuances { get; set; } = new HashSet<RequestFacilityInsuranceIssuance>(); 
        public virtual ICollection<NeoZarinRequestSignatureLog> NeoZarinRequestSignatureLogCreators { get; set; } = new HashSet<NeoZarinRequestSignatureLog>(); 
        public virtual ICollection<AyandehSignRequestSignatureLog> AyandehSignRequestSignatureLogCreators { get; set; } = new HashSet<AyandehSignRequestSignatureLog>(); 
        public virtual ICollection<RequestFacilityGuarantor> ApproverRequestFacilityGuarantors { get; set; } = new HashSet<RequestFacilityGuarantor>();
        public virtual ICollection<RequestFacilityGuarantor> RequestFacilityGuarantors { get; set; } = new HashSet<RequestFacilityGuarantor>();
        public virtual ICollection<VerifyResultExcel> VerifyResultExcels { get; set; } = new HashSet<VerifyResultExcel>();
        public virtual ICollection<VerifyResultExcelDetail> VerifyResultExcelDetails { get; set; } = new HashSet<VerifyResultExcelDetail>();
        public virtual ICollection<FinotechLog> FinotechLogs { get; set; } = new HashSet<FinotechLog>();
        public virtual ICollection<NeginHubLog> NeginHubLogs { get; set; } = new HashSet<NeginHubLog>();
        public virtual ICollection<PlanMember> PlanMembers { get; set; } = new HashSet<PlanMember>();
        public virtual ICollection<IranCreditScoringResultRule> CreatorIranCreditScoringResultRules { get; set; } = new HashSet<IranCreditScoringResultRule>();    
        public virtual ICollection<IranCreditScoringResultRule> UpdaterIranCreditScoringResultRules { get; set; } = new HashSet<IranCreditScoringResultRule>();    
        public virtual ICollection<PaymentInfo> CreatorPaymentInfos { get; set; } = new HashSet<PaymentInfo>();    
        public virtual ICollection<PaymentInfo> SellerPaymentInfos { get; set; } = new HashSet<PaymentInfo>();    
        public virtual ICollection<PaymentInfo> BuyerPaymentInfos { get; set; } = new HashSet<PaymentInfo>();    
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(p => p.UserName).IsRequired();
            builder.Property(p => p.NormalizedUserName).IsRequired();
            builder.HasIndex(p => p.UserName).IsUnique();
            builder.HasIndex(p => p.Email).IsUnique();

            builder.Property(p => p.IsActive).HasDefaultValue(true);
            builder.Property(p => p.Otp).HasMaxLength(50);
            //builder.HasOne(p => p.Person).WithMany(p => p.Users).HasForeignKey(p => p.PersonId);
            builder.HasOne(p => p.Person).WithOne(p => p.User).HasForeignKey<User>(p => p.PersonId);
            //builder.HasMany(p => p.IranCreditScorings).WithOne(p => p.Creator).HasForeignKey(p => p.CreatorId);
        }
    }
}
