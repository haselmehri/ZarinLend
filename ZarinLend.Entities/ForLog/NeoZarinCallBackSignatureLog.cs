using Core.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Entities
{
    public class NeoZarinCallBackSignatureLog : BaseEntity
    {
        public NeoZarinCallBackSignatureLog()
        {

        }
        public string? SendTrackId { get; set; }
        public string? ReceiveTrackId { get; set; }
        public int RequestFacilityId { get; set; }
        public string? FileUrl { get; set; }
    }

    public class NeoZarinCallBackSignatureLogConfiguration : BaseEntityTypeConfiguration<NeoZarinCallBackSignatureLog>
    {
        public override void Configure(EntityTypeBuilder<NeoZarinCallBackSignatureLog> builder)
        {
            
        }
    }
}
