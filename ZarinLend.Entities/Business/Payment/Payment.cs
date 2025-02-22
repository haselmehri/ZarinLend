using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Business.Payment
{
    public enum PaymentType
    {
        /// <summary>
        /// پرداخت هزینه اعتبار سنجی
        /// </summary>
        [Display(Name = "پرداخت هزینه اعتبار سنجی")]
        PayValidationFee = 1,

        /// <summary>
        /// پرداخت کمیسیون فروش
        /// </summary>
        [Display(Name = "پرداخت کمیسیون فروش")]
        PaySalesCommission = 2,

        /// <summary>
        /// پرداخت اقساط تسهیلات
        /// </summary>
        [Display(Name = "پرداخت اقساط تسهیلات")]
        PayInstallment = 3,

        /// <summary>
        /// پرداخت هزینه اعتبار سنجی توسط ضامن
        /// </summary>
        [Display(Name = "پرداخت هزینه اعتبار سنجی توسط ضامن")]
        PayValidationFeeByGuarantor = 4,

        /// <summary>
        /// خرید از فروشگاه توسط خریدار
        /// </summary>
        [Display(Name = "خرید از فروشگاه توسط خریدار")]
        PayForBuyByBuyer = 5
    }

    public enum IpgType
    {
        SamanIPG = 1,
        ZarinpalIPG = 2
    }
    public class Payment : BaseEntity<long>
    {
        public long Amount { get; set; }
        public IpgType IpgType { get; set; }
        public bool? IsSuccess { get; set; }
        public string? Description { get; set; }
        public PaymentType PaymentType { get; set; }

        public virtual Transaction.TransactionReason TransactionReason { get; set; }
        public virtual PaymentReason PaymentReason { get; set; }

        #region User

        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        #endregion User

        #region RequestFacility        
        //public int? RequestFacilityId { get; set; }
        //public virtual RequestFacility RequestFacility { get; set; }
        #endregion

        #region RequestFacilityInstallment        
        //public int? RequestFacilityInstallmentId { get; set; }
        //public virtual RequestFacilityInstallment RequestFacilityInstallment { get; set; }
        #endregion

        #region RequestFacilityGuarantor      
        //public int? RequestFacilityGuarantorId { get; set; }
        //public virtual RequestFacilityGuarantor RequestFacilityGuarantor { get; set; }
        #endregion
    }

    public class PaymentConfiguration : BaseEntityTypeConfiguration<Payment>
    {
        public override void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasOne(p => p.User).WithMany(p => p.Payments).HasForeignKey(p => p.UserId);
            //builder.HasOne(p => p.RequestFacility).WithMany(p => p.Payments).HasForeignKey(p => p.RequestFacilityId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            //builder.HasOne(p => p.RequestFacilityInstallment).WithMany(p => p.Payments).HasForeignKey(p => p.RequestFacilityInstallmentId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            //builder.HasOne(p => p.RequestFacilityGuarantor).WithMany(p => p.Payments).HasForeignKey(p => p.RequestFacilityGuarantorId).Metadata.DeleteBehavior = DeleteBehavior.Restrict;
            builder.UseTptMappingStrategy();
        }
    }
}
