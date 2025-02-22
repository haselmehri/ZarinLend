using Core.Entities;
using Core.Entities.Business.RequestFacility;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface IRequestFacilityGuarantorWorkFlowStepRepository : IBaseRepository<RequestFacilityGuarantorWorkFlowStep>
    {
        Task ChangeCurrentStepAndGoToNextStep(int requestFacilityGuarantorId, WorkFlowFormEnum workFlowFormEnum, StatusEnum statusEnum,
            Guid opratorId, string statusDescription = null, CancellationToken cancellationToken = default, bool autSave = true);

        Task ChangeCurrentStepAndGoToNextStep(int requestFacilityGuarantorId, WorkFlowFormEnum workFlowFormEnum, StatusEnum statusEnum,
            Guid guarantorUserId, Guid opratorId, string statusDescription = null, CancellationToken cancellationToken = default, bool autoSave = true);
    }
}