using Core.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Entities
{

    public class InvoiceDocument : BaseDocument
    {
        public InvoiceDocument()
        {

        }

        #region Invoice
        public int InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }
        #endregion
    }

    public class InvoiceDocumentConfiguration : BaseEntityTypeConfiguration<InvoiceDocument>
    {
        public override void Configure(EntityTypeBuilder<InvoiceDocument> builder)
        {
            builder.HasOne(p => p.Invoice).WithMany(p => p.InvoiceDocuments).HasForeignKey(p => p.InvoiceId);
            builder.Property(p => p.FilePath).IsRequired();
        }
    }
}
