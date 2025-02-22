using Core.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Common.Enums;

namespace Core.Entities
{

    public class IranCreditScoringDocument : BaseDocument
    {
        public IranCreditScoringDocument()
        {

        }
        public DocumentType DocumentType { get; set; }

        #region IranCreditScoring
        public int IranCreditScoringId { get; set; }
        public virtual IranCreditScoring IranCreditScoring { get; set; }
        #endregion
    }

    public class IranCreditScoringDocumentConfiguration : BaseEntityTypeConfiguration<IranCreditScoringDocument>
    {
        public override void Configure(EntityTypeBuilder<IranCreditScoringDocument> builder)
        {
            builder.HasOne(p => p.IranCreditScoring).WithMany(p => p.IranCreditScoringDocuments).HasForeignKey(p => p.IranCreditScoringId);
            builder.Property(p => p.FilePath).IsRequired();
        }
    }
}
