using Core.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Entities.Business.RequestFacility
{
    public class RequestFacilityPromissory : BaseEntity
    {
        public RequestFacilityPromissory()
        {

        }

        #region RequestFacility
        public int RequestFacilityId { get; set; }
        public virtual RequestFacility RequestFacility { get; set; }
        #endregion
        public bool IsActive { get; set; }
        public string IssuerName { get; set; }
        public string IssuerIdCode { get; set; }
        public string BirthDate { get; set; }
        public string IssuerIban { get; set; }
        public string IssuerIdType { get; set; }
        public string IssuerPostalCode { get; set; }
        public string IssuerAddress { get; set; }
        public string IssuerMobile { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverIdCode { get; set; }
        public string ReceiverBirthDate { get; set; }
        public string ReceiverIdType { get; set; }
        public string ReceiverMobile { get; set; }
        public string PaymentPlace { get; set; }
        public long Amount { get; set; }
        public string? DuDate { get; set; }
        public string Description { get; set; }
        public bool Transferable { get; set; }
        public string TrackId { get; set; }

        #region Response-> PromissoryPublishRequest api
        public string? RequestId { get; set; }
        public string? PromissoryId { get; set; }
        public string? UnSignedPdf { get; set; }
        #endregion Response-> PromissoryPublishRequest api

        #region request body -> request sign api
        public string? Application { get; set; }
        public string? CallbackUrl { get; set; }
        public bool? LetSignerDownload { get; set; }
        public string? State { get; set; }
        #endregion request body -> request sign api

        #region Response -> request sign api
        public string? SigningTrackId { get; set; }
        #endregion Response body -> request sign api

        #region Response -> promissoryFinalize api
        public string? MultiSignedPdf { get; set; }
        #endregion Response -> promissoryFinalize api

        #region Response -> statusInquiry api
        public int? SigningStatus { get; set; }
        #endregion Response -> statusInquiry api
    }

    public class RequestFacilityPromissoryConfiguration : BaseEntityTypeConfiguration<RequestFacilityPromissory>
    {
        public override void Configure(EntityTypeBuilder<RequestFacilityPromissory> builder)
        {
            builder.HasOne(p => p.RequestFacility).WithMany(p => p.RequestFacilityPromissories).HasForeignKey(p => p.RequestFacilityId);
            builder.Property(p => p.IssuerPostalCode).HasMaxLength(50);
            builder.Property(p => p.IssuerAddress).HasMaxLength(500);
            builder.Property(p => p.IssuerIban).HasMaxLength(50);
            builder.Property(p => p.IssuerIdCode).HasMaxLength(50);
            builder.Property(p => p.IssuerMobile).HasMaxLength(50);
            builder.Property(p => p.IssuerName).HasMaxLength(200);
            builder.Property(p => p.BirthDate).HasMaxLength(50);
            builder.Property(p => p.IssuerIdType).HasMaxLength(50);
            builder.Property(p => p.ReceiverBirthDate).HasMaxLength(50);
            builder.Property(p => p.ReceiverIdCode).HasMaxLength(50);
            builder.Property(p => p.ReceiverMobile).HasMaxLength(50);
            builder.Property(p => p.ReceiverIdType).HasMaxLength(50);
            builder.Property(p => p.ReceiverName).HasMaxLength(50);
            builder.Property(p => p.Application).HasMaxLength(50);
            builder.Property(p => p.State).HasMaxLength(50);
            builder.Property(p => p.CallbackUrl).HasMaxLength(500);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.DuDate).HasMaxLength(50);
            builder.Property(p => p.PaymentPlace).HasMaxLength(500);
            builder.Property(p => p.RequestId).HasMaxLength(200);
            builder.Property(p => p.PromissoryId).HasMaxLength(200);
            builder.Property(p => p.SigningTrackId).HasMaxLength(200);
            builder.Property(p => p.TrackId).HasMaxLength(50);
        }
    }
}