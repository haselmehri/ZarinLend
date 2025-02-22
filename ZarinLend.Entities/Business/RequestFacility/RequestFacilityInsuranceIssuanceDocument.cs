using Core.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Common.Enums;

namespace Core.Entities.Business.RequestFacility
{

    public class RequestFacilityInsuranceIssuanceDocument : BaseDocument
    {
        public RequestFacilityInsuranceIssuanceDocument()
        {

        }
        public DocumentType DocumentType { get; set; }
        #region RequestFacilityInsuranceIssuance
        public int RequestFacilityInsuranceIssuanceId { get; set; }
        public virtual RequestFacilityInsuranceIssuance RequestFacilityInsuranceIssuance { get; set; }
        #endregion
    }

    public class RequestFacilityInsuranceIssuanceDocumentConfiguration : BaseEntityTypeConfiguration<RequestFacilityInsuranceIssuanceDocument>
    {
        public override void Configure(EntityTypeBuilder<RequestFacilityInsuranceIssuanceDocument> builder)
        {
            builder.HasOne(p => p.RequestFacilityInsuranceIssuance).WithMany(p => p.RequestFacilityInsuranceIssuanceDocuments).HasForeignKey(p => p.RequestFacilityInsuranceIssuanceId);
            builder.Property(p=>p.FilePath).IsRequired();
        }
    }
}
