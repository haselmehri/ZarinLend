using Core.Entities.Base;
using Core.Entities.Business.Payment;
using Core.Entities.Business.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Business.RequestFacility
{
    public enum FacilityStatus
    {
        WaitingRequest = 1,
        ApprovedRequest = 2,
        RejectCancelRequest = 3,
        OpenRequest = 4,
        OpenAndNotSetOperator = 5,
        JustShowWaitingSignatureRequest = 6,
        AllRequest = -1
    }

    public enum UserOption
    {
        /// <summary>
        /// عضویت در باشگاه مشتریان
        /// </summary>
        [Display(Name = "عضویت در باشگاه مشتریان")]
        MembershipInCustomerClub = 1,

        /// <summary>
        /// خرید از فروشگاه های طرف قرارداد
        /// </summary>
        [Display(Name = "خرید از فروشگاه های طرف قرارداد")]
        PurchaseFromContractedStores = 2
    }

    public enum SigningMethod
    {
        HardwareToken = 1,
        MobileApp =2
    }

    public class RequestFacility : BaseEntity
    {
        public RequestFacility()
        {

        }
        public long Amount { get; set; }

        /// <summary>
        /// مبلغ تایید شده اعتبار
        /// </summary>
        public long? ApprovedAmount { get; set; }
        /// <summary>
        /// bank user will fill this field
        /// </summary>
        public string? FacilityNumber { get; set; }

        /// <summary>
        /// bank user will fill this field
        /// </summary>
        public string? PoliceNumber { get; set; }

        /// <summary>
        /// کارمزد اعتبار
        /// </summary>
        public long? FeeAmount { get; set; }

        public bool CancelByUser { get; set; }
        public UserOption UserOption { get; set; }

        /// <summary>
        /// تسهیلات کاملا تسویه شده هست
        /// </summary>
        public bool DoneFacility { get; set; }
        public bool GuarantorIsRequired { get; set; }
        public bool AwaitingIntroductionGuarantor { get; set; }

        /// <summary>
        /// باید اعتبارسنجی ایرانیان مجددا انجام شود
        /// </summary>
        public bool? ValidationMustBeRepeated { get; set; }

        /// <summary>
        /// if user select 'other' this field is filled. 
        /// </summary>
        public string? UsagePlaceDescription { get; set; }

        #region Contract properties
        public string? ContractFileName { get; set; }
        public string? SignedContractByUserFileName { get; set; }
        public string? SignedContractByBankFileName { get; set; }
        public string? Digest { get; set; }
        public string? Certificate { get; set; }
        public string? Signature { get; set; }
        public string? AyandehSignSigningToken { get; set; }
        public string? AyandehSignSigningTokenForAdminBank { get; set; }
        public SigningMethod? SigningMethod { get; set; }

        #endregion Contract properties

        #region Deposit Bon Card properties
        public string? DepositDocumentFileName { get; set; }
        public DateTime? DepositDate { get; set; }
        public string? DepositDocumentNumber { get; set; }

        #endregion Deposit Bon Card properties

        #region Organization/خریدار لیزینگی/بانک که میخواهد از آن وام بگیرد را انتخاب میکند
        public int? OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
        #endregion

        #region GlobalSetting
        public int GlobalSettingId { get; set; }
        public virtual GlobalSetting GlobalSetting { get; set; }
        #endregion

        #region FacilityType
        public int FacilityTypeId { get; set; }
        public virtual FacilityType FacilityType { get; set; }
        #endregion

        #region Plan
        public int? PlanMemberId { get; set; }
        public virtual PlanMember PlanMember { get; set; }
        #endregion

        #region Buyer-خریدار حقیقی/حقوقی-درخواست کننده تسهیلات
        public Guid BuyerId { get; set; }

        public virtual User Buyer { get; set; }
        #endregion

        #region Updater/Oprator/Approver
        public Guid? OperatorId { get; set; }
        public virtual User Operator { get; set; }
        #endregion

        #region IranCreditScoringResultRule
        public int IranCreditScoringResultRuleId { get; set; }
        public virtual IranCreditScoringResultRule IranCreditScoringResultRule { get; set; }
        #endregion

        public string? Description { get; set; }
        public virtual WalletTransaction WalletTransaction { get; set; }
        public virtual ICollection<Transaction.TransactionReason> TransactionReasons { get; set; } = new HashSet<TransactionReason>();
        public virtual ICollection<RequestFacilityWarranty> RequestFacilityWarranties { get; set; } = new HashSet<RequestFacilityWarranty>();
        public virtual ICollection<RequestFacilityInstallment> RequestFacilityInstallments { get; set; } = new HashSet<RequestFacilityInstallment>();
        public virtual ICollection<RequestFacilityWorkFlowStep> RequestFacilityWorkFlowSteps { get; set; } = new HashSet<RequestFacilityWorkFlowStep>();
        public virtual ICollection<PaymentReason> PaymentReasons { get; set; } = new HashSet<PaymentReason>();
        public virtual ICollection<RequestFaciliyUsagePlace> RequestFaciliyUsagePlaces { get; set; } = new HashSet<RequestFaciliyUsagePlace>();
        public virtual ICollection<IranCreditScoring> IranCreditScorings { get; set; } = new HashSet<IranCreditScoring>();
        public virtual ICollection<ApplicantValidationResult> ApplicantValidationResults { get; set; } = new HashSet<ApplicantValidationResult>();
        public virtual ICollection<RequestFacilityInsuranceIssuance> RequestFacilityInsuranceIssuances { get; set; } = new HashSet<RequestFacilityInsuranceIssuance>();
        public virtual ICollection<SamatBackChequeHeader> SamatBackChequeHeaders { get; set; } = new HashSet<SamatBackChequeHeader>();
        public virtual ICollection<SamatFacilityHeader> SamatFacilityHeaders { get; set; } = new HashSet<SamatFacilityHeader>();
        public virtual ICollection<VerifyResultExcelDetail> VerifyResultExcelDetails { get; set; } = new HashSet<VerifyResultExcelDetail>();
        public virtual ICollection<RequestFacilityGuarantor> RequestFacilityGuarantors { get; set; } = new HashSet<RequestFacilityGuarantor>();
        public virtual RequestFacilityCardIssuance RequestFacilityCardIssuance { get; set; }
        public virtual ICollection<NeoZarinRequestSignatureLog> NeoZarinRequestSignatureLogs { get; set; } = new HashSet<NeoZarinRequestSignatureLog>();
        public virtual ICollection<AyandehSignRequestSignatureLog> AyandehSignRequestSignatureLogs { get; set; } = new HashSet<AyandehSignRequestSignatureLog>();
        public virtual ICollection<RequestFacilityError> RequestFacilityErrors { get; set; } = new HashSet<RequestFacilityError>();
        public virtual ICollection<RequestFacilityPromissory> RequestFacilityPromissories { get; set; } = new HashSet<RequestFacilityPromissory>();
        public virtual ICollection<FinotechLog> FinotechLogs { get; set; } = new HashSet<FinotechLog>();
        //public virtual ICollection<NeginHubLog> NeginHubLogs { get; set; }
    }

    public class RequestFacilityConfiguration : BaseEntityTypeConfiguration<RequestFacility>
    {
        public override void Configure(EntityTypeBuilder<RequestFacility> builder)
        {
            builder.HasOne(p => p.Buyer).WithMany(p => p.RequestFacilityBuyers).HasForeignKey(p => p.BuyerId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.PlanMember).WithMany(p => p.RequestFacilities).HasForeignKey(p => p.PlanMemberId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.FacilityType).WithMany(p => p.RequestFacilities).HasForeignKey(p => p.FacilityTypeId);
            builder.HasOne(p => p.Organization).WithMany(p => p.OrganizationRequestFacilities).HasForeignKey(p => p.OrganizationId);
            builder.HasOne(p => p.Operator).WithMany(p => p.OperatorRequestFacilities).HasForeignKey(p => p.OperatorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.GlobalSetting).WithMany(p => p.RequestFacilities).HasForeignKey(p => p.GlobalSettingId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.HasOne(p => p.IranCreditScoringResultRule).WithMany(p => p.RequestFacilities).HasForeignKey(p => p.IranCreditScoringResultRuleId);
            builder.Property(p => p.FacilityNumber).HasMaxLength(25);
            builder.Property(p => p.FacilityNumber).HasMaxLength(25);
            builder.Property(p => p.ContractFileName).HasMaxLength(100);
            builder.Property(p => p.SignedContractByBankFileName).HasMaxLength(100);
            builder.Property(p => p.SignedContractByUserFileName).HasMaxLength(100);
            builder.Property(p => p.DepositDocumentFileName).HasMaxLength(100);
            builder.Property(p => p.FacilityNumber).HasMaxLength(25);
            builder.Property(p => p.DepositDocumentNumber).HasMaxLength(50);
            builder.Property(p => p.UsagePlaceDescription).HasMaxLength(200);
            builder.Property(p => p.Description).HasMaxLength(2000);
        }
    }
}
