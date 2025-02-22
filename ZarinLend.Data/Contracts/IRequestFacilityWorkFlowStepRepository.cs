using Core.Entities;
using Core.Entities.Business.RequestFacility;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface IRequestFacilityWorkFlowStepRepository : IBaseRepository<RequestFacilityWorkFlowStep>
    {
        Task ChangeCurrentStepAndGoToNextStep(int requestFacilityId, WorkFlowFormEnum workFlowFormEnum, StatusEnum statusEnum,
            Guid opratorId, string? statusDescription, CancellationToken cancellationToken = default, bool autoSave = true);

        Task ChangeCurrentStepAndGoToNextStep(int requestFacilityId, WorkFlowFormEnum workFlowFormEnum, StatusEnum statusEnum,
            Guid buyerId, Guid opratorId, string? statusDescription = null, CancellationToken cancellationToken = default,bool autoSave = true);
    }
}