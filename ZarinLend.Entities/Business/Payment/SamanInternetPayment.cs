using Core.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Entities.Business.Payment
{
    public class SamanInternetPayment : Payment
    {
        public SamanInternetPayment()
        {

        }

        /// <summary>
        /// card number for payment
        /// </summary>
        public string? CardNumberForPayment { get; set; }

        /// <summary>
        /// hash card number base SHA256
        /// </summary>
        public string? HashCardNumberForPayment { get; set; }

        #region Response From 'Payment Gateway Callback'
        /// <summary>
        /// شماره ترمینال
        /// </summary>
        public string? MId { get; set; }
        /// <summary>
        /// وضعیت تراکنش
        /// </summary>
        public string? State { get; set; }
        /// <summary>
        /// وضعیت تراکنش
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// شماره مرجع
        /// </summary>
        public string? RRN { get; set; }
        /// <summary>
        /// شماره ترمینال
        /// </summary>
        public string? TerminalId { get; set; }
        /// <summary>
        /// شماره رهگیری
        /// </summary>
        public string? TraceNo { get; set; }
        /// <summary>
        /// رسید دیجیتالی خرید
        /// </summary>
        public string? RefNum { get; set; }
        /// <summary>
        /// شماره خرید/OrderId
        /// </summary>
        public string? ResNum { get; set; }
        public long? ReturnAmount { get; set; }
        public int? Wage { get; set; }
        /// <summary>
        /// شماره کارتی که تراکنش با آن انجام شده است
        /// </summary>
        public string? SecurePan { get; set; }
        /// <summary>
        /// شماره کارت هش شده با الگوریتم SHA256
        /// </summary>
        public string? HashedCardNumber { get; set; }
        #endregion

        #region From 'Verify/Reverse Method'
        public string? MethodName { get; set; }
        public int? ResultCode { get; set; }
        public string? ResultDescription { get; set; }
        public string? TransactionDetail_RRN { get; set; }
        public string? TransactionDetail_RefNum { get; set; }
        public string? TransactionDetail_MaskedPan { get; set; }
        public string? TransactionDetail_HashedPan { get; set; }
        public int? TransactionDetail_TerminalNumber { get; set; }
        public long? TransactionDetail_OrginalAmount { get; set; }
        public long? TransactionDetail_AffectiveAmount { get; set; }
        public string? TransactionDetail_StraceDate { get; set; }
        public string? TransactionDetail_StraceNo { get; set; }

        #endregion From 'Verify/Reverse Method'
    }

    public class SamanInternetPaymentConfiguration : BaseEntityTypeConfiguration<SamanInternetPayment>
    {
        public override void Configure(EntityTypeBuilder<SamanInternetPayment> builder)
        {
            builder.HasIndex(p => p.ResNum).IsUnique();
            builder.Property(p => p.TraceNo).HasMaxLength(450);
            builder.Property(p => p.RefNum).HasMaxLength(200);            
            builder.Property(p => p.CardNumberForPayment).HasMaxLength(25);            
            builder.Property(p => p.HashCardNumberForPayment).HasMaxLength(500);            
            builder.Property(p => p.MethodName).HasMaxLength(100);            
        }
    }
}
