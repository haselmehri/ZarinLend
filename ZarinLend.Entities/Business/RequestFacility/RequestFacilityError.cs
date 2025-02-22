using Core.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Entities.Business.RequestFacility
{
    public class RequestFacilityError : BaseEntity
    {
        public RequestFacilityError()
        {

        }

        #region RequestFacility
        public int RequestFacilityId { get; set; }
        public virtual RequestFacility RequestFacility { get; set; }
        #endregion

        #region WorkFlowStepError
        public int WorkFlowStepErrorId { get; set; }
        public virtual WorkFlowStepError WorkFlowStepError { get; set; }
        #endregion

        public bool IsDone { get; set; }
    }

    public class RequestFacilityErrorConfiguration : BaseEntityTypeConfiguration<RequestFacilityError>
    {
        public override void Configure(EntityTypeBuilder<RequestFacilityError> builder)
        {
            builder.HasOne(p => p.RequestFacility).WithMany(p => p.RequestFacilityErrors).HasForeignKey(p => p.RequestFacilityId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(p => p.WorkFlowStepError).WithMany(p => p.RequestFacilityErrors).HasForeignKey(p => p.WorkFlowStepErrorId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
