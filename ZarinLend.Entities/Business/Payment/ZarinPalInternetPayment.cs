using Core.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Entities.Business.Payment
{

    public class ZarinPalInternetPayment : Payment
    {
        public ZarinPalInternetPayment()
        {

        }

        #region Response From 'Payment Gateway'
        public string? Merchant_Id { get; set; }
        public string? Authority { get; set; }
        public int? Code { get; set; }
        public string? Message { get; set; }
        public string? Card_Hash { get; set; }
        public string? Card_Pan { get; set; }
        public string? Ref_Id { get; set; }
        public string? Fee_Type { get; set; }
        public int? Fee { get; set; }
        public string? Errors { get; set; }
        #endregion
    }

    public class ZarinPalInternetPaymentConfiguration : BaseEntityTypeConfiguration<ZarinPalInternetPayment>
    {
        public override void Configure(EntityTypeBuilder<ZarinPalInternetPayment> builder)
        {
            builder.HasIndex(p => p.Authority).IsUnique();
            builder.Property(p => p.Authority).HasMaxLength(450);
            builder.Property(p => p.Ref_Id).HasMaxLength(200);
        }
    }
}
