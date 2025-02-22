using Common;
using Core.Entities;
using Core.Entities.Business.RequestFacility;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public class RequestFacilityWorkFlowStepRepository : BaseRepository<RequestFacilityWorkFlowStep>, IRequestFacilityWorkFlowStepRepository, IScopedDependency
    {
        private readonly IWorkFlowStepRepository workFlowStepRepository;

        public RequestFacilityWorkFlowStepRepository(ApplicationDbContext dbContext, IWorkFlowStepRepository workFlowStepRepository) : base(dbContext)
        {
            this.workFlowStepRepository = workFlowStepRepository;
        }

        public async Task ChangeCurrentStepAndGoToNextStep(int requestFacilityId, WorkFlowFormEnum workFlowFormEnum, StatusEnum statusEnum,
            Guid opratorId, string? statusDescription = null, CancellationToken cancellationToken = default, bool autSave = true)
        {
            var currentStep = await GetByConditionAsync(p => p.RequestFacilityId == requestFacilityId &&
            !p.StatusId.HasValue &&
            p.WorkFlowStep.WorkFlowFormId.Equals(workFlowFormEnum),
            cancellationToken);
            if (currentStep != default(RequestFacilityWorkFlowStep))
            {
                currentStep.StatusId = (short)statusEnum;
                currentStep.OpratorId = opratorId;
                currentStep.StatusDescription = statusDescription;
                await UpdateCustomPropertiesAsync(currentStep, cancellationToken, false,
                    nameof(RequestFacilityWorkFlowStep.OpratorId),
                    nameof(RequestFacilityWorkFlowStep.StatusId),
                    nameof(RequestFacilityWorkFlowStep.StatusDescription),
                    nameof(RequestFacilityWorkFlowStep.UpdateDate));

                var nextStepId = await workFlowStepRepository.GetNextStepId(currentStep.WorkFlowStepId, statusEnum, cancellationToken);

                if (nextStepId.HasValue)
                {
                    if (statusEnum == StatusEnum.Rejected)
                    {
                        var lastOrRejectStepId = await workFlowStepRepository.GetLastStepId(WorkFlowEnum.RequestFacility, cancellationToken);
                        await AddAsync(new RequestFacilityWorkFlowStep()
                        {
                            OpratorId = opratorId,
                            RequestFacilityId = requestFacilityId,
                            WorkFlowStepId = nextStepId.Value,
                            StatusDescription = lastOrRejectStepId.HasValue && lastOrRejectStepId.Value == nextStepId.Value ? statusDescription : null,
                            StatusId = lastOrRejectStepId.HasValue && lastOrRejectStepId.Value == nextStepId.Value
                                    ? (short?)StatusEnum.Rejected
                                    : null,
                        }, cancellationToken, false);
                    }
                    else
                    {
                        await AddAsync(new RequestFacilityWorkFlowStep()
                        {
                            OpratorId = opratorId,
                            RequestFacilityId = requestFacilityId,
                            WorkFlowStepId = nextStepId.Value
                        }, cancellationToken, false);
                    }
                }
                if (autSave)
                    await SaveChangesAsync(cancellationToken);
            }
        }

        public async Task ChangeCurrentStepAndGoToNextStep(int requestFacilityId, WorkFlowFormEnum workFlowFormEnum, StatusEnum statusEnum,
            Guid buyerId, Guid opratorId, string? statusDescription = null, CancellationToken cancellationToken = default, bool autoSave = true)
        {
            var currentStep = await GetByConditionAsync(p => p.RequestFacilityId == requestFacilityId &&
            !p.StatusId.HasValue &&
            p.RequestFacility.BuyerId.Equals(buyerId) &&
            p.WorkFlowStep.WorkFlowFormId.Equals(workFlowFormEnum),
            cancellationToken);
            if (currentStep != default(RequestFacilityWorkFlowStep))
            {
                currentStep.StatusId = (short)statusEnum;
                currentStep.OpratorId = opratorId;
                currentStep.StatusDescription = statusDescription;
                await UpdateCustomPropertiesAsync(currentStep, cancellationToken, false,
                    nameof(RequestFacilityWorkFlowStep.OpratorId),
                    nameof(RequestFacilityWorkFlowStep.StatusId),
                    nameof(RequestFacilityWorkFlowStep.StatusDescription),
                    nameof(RequestFacilityWorkFlowStep.UpdateDate));

                var nextStepId = await workFlowStepRepository.GetNextStepId(currentStep.WorkFlowStepId, statusEnum, cancellationToken);

                if (nextStepId.HasValue)
                {
                    if (statusEnum == StatusEnum.Rejected)
                    {
                        var lastOrRejectStepId = await workFlowStepRepository.GetLastStepId(WorkFlowEnum.RequestFacility, cancellationToken);
                        await AddAsync(new RequestFacilityWorkFlowStep()
                        {
                            OpratorId = opratorId,
                            RequestFacilityId = requestFacilityId,
                            WorkFlowStepId = nextStepId.Value,
                            StatusId = lastOrRejectStepId.HasValue && lastOrRejectStepId.Value == nextStepId.Value
                                    ? (short?)StatusEnum.Rejected
                                    : null,
                        }, cancellationToken, false);
                    }
                    else
                    {
                        await AddAsync(new RequestFacilityWorkFlowStep()
                        {
                            OpratorId = opratorId,
                            RequestFacilityId = requestFacilityId,
                            WorkFlowStepId = nextStepId.Value
                        }, cancellationToken, false);
                    }
                }
                if (autoSave)
                    await SaveChangesAsync(cancellationToken);
            }
        }
    }
}
