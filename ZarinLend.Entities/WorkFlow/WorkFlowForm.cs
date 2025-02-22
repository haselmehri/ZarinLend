using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public enum WorkFlowFormEnum
    {
        #region 'Register Facility' forms
        /// <summary>
        /// فرم ثبت درخواست تسهیلات
        /// </summary>
        [Display(Name = "فرم ثبت درخواست تسهیلات", Description = "/Requestfacility/Add")]
        RegisterRequestFacility = 1,

        /// <summary>
        /// فرم تکمیل اطلاعات هویتی
        /// </summary>
        [Display(Name = "فرم تکمیل اطلاعات هویتی", Description = "/User/Edit")]
        RegisterIdentityInfo = 2,

        /// <summary>
        /// فرم بارگذاری مدارک هویتی
        /// </summary>
        [Display(Name = "فرم بارگذاری مدارک هویتی", Description = "/User/UploadIdentityDocuments")]
        UploadIdentityDocuments = 3,

        /// <summary>
        /// ویرایش اطلاعات هویتی و میزان و مدت باز پرداخت اعتبار
        /// </summary>
        //[Display(Name = "ویرایش اطلاعات هویتی و میزان و مدت باز پرداخت اعتبار")]
        //EditRequestAndIdentityInfo = 4,

        /// <summary>
        /// فرم اعتبار سنجی توسط کاربر زرین لند
        /// </summary>
        [Display(Name = "فرم اعتبار سنجی توسط کاربر زرین لند", Description = "/requestfacility/VerifyBuyerByZarinLend")]
        VerifyZarrinLend = 5,

        /// <summary>
        /// فرم استعلام توسط کاربر لیزینگ
        /// </summary>
        [Display(Name = "فرم استعلام توسط کاربر لیزینگ", Description = "/requestfacility/VerifyBuyerByLeasing")]
        VerifyLeasing = 6,

        /// <summary>
        /// فرم بارگذاری و ثبت تضامین
        /// </summary>
        [Display(Name = "فرم بارگذاری و ثبت تضامین", Description = "/RequestFacilityWarranty/RegisterGuarantees")]
        RegisterGuarantees = 7,

        /// <summary>
        /// فرم پرداخت کارمزد اعتبار سنجی
        /// </summary>
        [Display(Name = "فرم پرداخت کارمزد اعتبار سنجی", Description = "/Payment/InternetPayment")]
        PaymentVerifyShahkarAndSamatService = 8,

        /// <summary>
        /// فرم بررسی چک توسط زرین لند
        /// </summary>
        [Display(Name = "بررسی چک توسط زرین لند", Description = "/RequestFacility/VerifyCheckByZarinLend")]
        VerifyCheckByZarinLend = 10,

        /// <summary>
        /// فرم بررسی چک توسط لیزینگ
        /// </summary>
        [Display(Name = "بررسی چک توسط لیزینگ", Description = "/RequestFacility/VerifyCheckByLeasing")]
        VerifyCheckByLeasing = 11,

        /// <summary>
        /// پرداخت کارمزد کمیسیون فروش
        /// </summary>
        [Display(Name = "پرداخت کارمزد کمیسیون فروش", Description = "/Payment/PaySalesCommission")]
        PaySalesCommission = 12,

        /// <summary>
        /// فرم اعتبار سنجی
        /// </summary>
        [Display(Name = "انجام اعتبارسنجی و نمایش به خریدار", Description = "/requestfacility/VerifyShahkarAndSamatService")]
        VerifyShahkarAndSamatService = 13,

        /// <summary>
        /// فرم اعتبار سنجی
        /// </summary>
        [Display(Name = "انجام اعتبارسنجی ایرانیان", Description = "/irancreditscoring/verify")]
        VerifyIranCreditScoring = 14,

        /// <summary>
        /// ارسال به بیمه و اطلاع رسانی به مشتری
        /// </summary>
        [Display(Name = "ارسال به بیمه و اطلاع رسانی به مشتری", Description = "/RequestFacility/InsuranceIssuance")]
        InsuranceIssuance = 15,

        /// <summary>
        /// صدور بن کارت
        /// </summary>
        [Display(Name = "صدور بن کارت", Description = "/RequestFacility/CardIssuance")]
        CardIssuance = 16,

        /// <summary>
        /// بررسی نهایی/ثبت شماره انتظامی
        /// </summary>
        [Display(Name = "بررسی نهایی/ثبت شماره انتظامی", Description = "/RequestFacility/BankLeasingInquiry")]
        BankLeasingInquiry = 17,

        /// <summary>
        /// امضاء قرارداد دیجیتال
        /// </summary>
        [Display(Name = "امضاء قرارداد دیجیتال", Description = "/RequestFacility/AdminBankLeasingSignature")]
        AdminBankLeasingSignature = 18,

        /// <summary>
        /// واریز تسهیلات به حساب زرین لند
        /// </summary>
        [Display(Name = "واریز تسهیلات به حساب زرین لند", Description = "/RequestFacility/DepositFacilityAmount")]
        DepositFacilityAmount = 19,

        /// <summary>
        /// تنظیم شماره قرارداد/تسهیلات
        /// </summary>
        [Display(Name = "تنظیم شماره قرارداد/تسهیلات", Description = "/requestfacility/EnterFacilityNumber")]
        EnterFacilityNumber = 20,

        /// <summary>
        /// امضاء قرارداد تسهیلات توسط درخواست دهنده تسهیلات
        /// </summary>
        [Display(Name = "امضاء قرارداد تسهیلات توسط درخواست دهنده تسهیلات", Description = "/SignContract/SignContractByUser")]
        SignContractByUser = 21,

        /// <summary>
        /// امضاء قرارداد تسهیلات توسط درخواست دهنده تسهیلات
        /// </summary>
        [Display(Name = "در انتظار امضاء قرارداد تسهیلات توسط درخواست دهنده تسهیلات", Description = "")]
        WaitingToSignContractByUser = 22,

        /// <summary>
        /// شارژ بن کارت
        /// </summary>
        [Display(Name = "شارژ بن کارت", Description = "/RequestFacility/CardRecharge")]
        CardRecharge = 23,

        /// <summary>
        /// در انتظار نتیجه استعلام
        /// </summary>
        [Display(Name = "در انتظار نتیجه استعلام", Description = "/requestfacility/PendingForVerifyResult")]
        PendingForVerifyResult = 24,

        /// <summary>
        /// در انتظار تنظیم شماره قرارداد/تسهیلات
        /// </summary>
        [Display(Name = "در انتظار تنظیم شماره قرارداد/تسهیلات", Description = "/requestfacility/PendingEnterFacilityNumber")]
        PendingEnterFacilityNumber = 25,

        /// <summary>
        /// در انتظار بررسی نهایی/ثبت شماره انتظامی
        /// </summary>
        [Display(Name = "در انتظار بررسی نهایی/ثبت شماره انتظامی", Description = "/RequestFacility/PendingBankLeasingInquiry")]
        PendingBankLeasingInquiry = 26,

        /// <summary>
        /// صدور و امضاء سفته
        /// </summary>
        [Display(Name = "صدور و امضاء سفته", Description = "/RequestFacilityPromissory/SignPromissoryByUser")]
        SignPromissoryByUser = 27,

        /// <summary>
        /// صدور و امضاء سفته
        /// </summary>
        [Display(Name = "در انتظار صدور و امضاء سفته", Description = "/RequestFacilityPromissory/SignPromissoryByUser")]
        WaitingSignPromissoryByUser = 28,

        /// <summary>
        ///در انتظار نتیجه واریز تسهیلات به حساب زرین لنددر انتظار 
        /// </summary>
        [Display(Name = "واریز تسهیلات به حساب زرین لند", Description = "/RequestFacility/PendingDepositFacilityAmount")]
        PendingDepositFacilityAmount = 29,

        /// <summary>
        /// تکمیل اطلاعات بن کارت
        /// </summary>
        [Display(Name = "تکمیل اطلاعات بن کارت", Description = "/RequestFacility/CompleteBonCardInfo")]
        CompleteBonCardInfo = 30,

        /// <summary>
        /// فرم تایید/رد پیش فرض
        /// </summary>
        [Display(Name = "فرم تایید/رد پیش فرض")]
        DefaultForm = 100,

        #endregion 'Register Facility' forms

        #region 'Register Guarantors' forms

        /// <summary>
        /// فرم ثبت ضامن
        /// </summary>
        [Display(Name = "فرم ثبت ضامن", Description = "/RequestFacilityGuarantor/RegisterGuarantor")]
        RegisterRequestGuarantor = 1001,

        /// <summary>
        /// فرم تکمیل اطلاعات هویتی برای ضامن
        /// </summary>
        [Display(Name = "فرم تکمیل اطلاعات هویتی", Description = "/User/EditGuarantorInfo")]
        EditGuarantorInfo = 1002,

        /// <summary>
        /// فرم بارگذاری مدارک هویتی
        /// </summary>
        [Display(Name = "فرم بارگذاری مدارک هویتی", Description = "/User/UploadIdentityDocumentsGuarantor")]
        UploadIdentityDocumentsGuarantor = 1003,

        /// <summary>
        /// فرم بررسی ضامن توسط کارشناس زرین لند
        /// </summary>
        [Display(Name = "فرم بررسی ضامن توسط کارشناس زرین لند", Description = "/RequestFacilityGuarantor/VerifyGuarantorByZarinLend")]
        VerifyGuarantorByZarinLend = 1004,

        /// <summary>
        /// فرم بارگذاری و ثبت اطلاعات چک توسط ضامن
        /// </summary>
        [Display(Name = "فرم بارگذاری و ثبت اطلاعات چک توسط ضامن", Description = "/RequestFacilityGuarantorWarranty/RegisterGuaranteesByGuarantor")]
        RegisterGuaranteesByGuarantor = 1005,

        /// <summary>
        /// فرم بررسی ضامن توسط کارشناس زرین لند
        /// </summary>
        [Display(Name = "فرم بررسی اطلاعات چک توسط کارشناس زرین لند", Description = "/RequestFacilityGuarantor/VerifyGuaranteesByZarinLend")]
        VerifyGuaranteesByZarinLend = 1006,

        #endregion 'Register Guarantors' forms
    }
    public class WorkFlowForm : BaseEntity<WorkFlowFormEnum>
    {
        public WorkFlowForm()
        {
            WorkFlowSteps = new HashSet<WorkFlowStep>();
            WorkFlowStepCorrections = new HashSet<WorkFlowStepCorrection>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override WorkFlowFormEnum Id { get => base.Id; set => base.Id = value; }
        public string Title { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<WorkFlowStep> WorkFlowSteps { get; set; }
        public virtual ICollection<WorkFlowStepCorrection> WorkFlowStepCorrections { get; set; }
    }

    public class WorkFlowFormConfiguration : BaseEntityTypeConfiguration<WorkFlowForm>
    {
        public override void Configure(EntityTypeBuilder<WorkFlowForm> builder)
        {
            builder.Property(p => p.Title).IsRequired().HasMaxLength(250);
            builder.Property(p => p.IsActive).HasDefaultValue(true);
        }
    }
}
