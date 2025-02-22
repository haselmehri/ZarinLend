using Common;
using Core.Entities;
using Core.Entities.Business.RequestFacility;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public class RequestFacilityGuarantorWorkFlowStepRepository : BaseRepository<RequestFacilityGuarantorWorkFlowStep>, IRequestFacilityGuarantorWorkFlowStepRepository, IScopedDependency
    {
        private readonly IWorkFlowStepRepository workFlowStepRepository;

        public RequestFacilityGuarantorWorkFlowStepRepository(ApplicationDbContext dbContext, IWorkFlowStepRepository workFlowStepRepository) : base(dbContext)
        {
            this.workFlowStepRepository = workFlowStepRepository;
        }

        public async Task ChangeCurrentStepAndGoToNextStep(int requestFacilityGuarantorId, WorkFlowFormEnum workFlowFormEnum, StatusEnum statusEnum,
            Guid opratorId, string statusDescription = null, CancellationToken cancellationToken = default, bool autSave = true)
        {
            var currentStep = await GetByConditionAsync(p => p.RequestFacilityGuarantorId == requestFacilityGuarantorId &&
            !p.StatusId.HasValue &&
            p.WorkFlowStep.WorkFlowFormId.Equals(workFlowFormEnum),
            cancellationToken);
            if (currentStep != default(RequestFacilityGuarantorWorkFlowStep))
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
                        var lastOrRejectStepId = await workFlowStepRepository.GetLastStepId(WorkFlowEnum.RegisterGuarantor, cancellationToken);
                        await AddAsync(new RequestFacilityGuarantorWorkFlowStep()
                        {
                            OpratorId = opratorId,
                            RequestFacilityGuarantorId = requestFacilityGuarantorId,
                            WorkFlowStepId = nextStepId.Value,
                            StatusId = lastOrRejectStepId.HasValue && lastOrRejectStepId.Value == nextStepId.Value
                                    ? (short?)StatusEnum.Rejected
                                    : null,
                        }, cancellationToken, false);
                    }
                    else
                    {
                        await AddAsync(new RequestFacilityGuarantorWorkFlowStep()
                        {
                            OpratorId = opratorId,
                            RequestFacilityGuarantorId = requestFacilityGuarantorId,
                            WorkFlowStepId = nextStepId.Value
                        }, cancellationToken, false);
                    }
                }
                if (autSave)
                    await SaveChangesAsync(cancellationToken);
            }
        }

        public async Task ChangeCurrentStepAndGoToNextStep(int requestFacilityGuarantorId, WorkFlowFormEnum workFlowFormEnum, StatusEnum statusEnum,
            Guid guarantorUserId, Guid opratorId, string statusDescription = null, CancellationToken cancellationToken = default, bool autoSave = true)
        {
            var currentStep = await GetByConditionAsync(p => p.RequestFacilityGuarantorId == requestFacilityGuarantorId &&
            !p.StatusId.HasValue &&
            p.RequestFacilityGuarantor.GuarantorUserId.Equals(guarantorUserId) &&
            p.WorkFlowStep.WorkFlowFormId.Equals(workFlowFormEnum),
            cancellationToken);
            if (currentStep != default(RequestFacilityGuarantorWorkFlowStep))
            {
                currentStep.StatusId = (short)statusEnum;
                currentStep.OpratorId = opratorId;
                currentStep.StatusDescription = statusDescription;
                await UpdateCustomPropertiesAsync(currentStep, cancellationToken, false,
                    nameof(RequestFacilityGuarantorWorkFlowStep.OpratorId),
                    nameof(RequestFacilityGuarantorWorkFlowStep.StatusId),
                    nameof(RequestFacilityGuarantorWorkFlowStep.StatusDescription),
                    nameof(RequestFacilityGuarantorWorkFlowStep.UpdateDate));

                var nextStepId = await workFlowStepRepository.GetNextStepId(currentStep.WorkFlowStepId, statusEnum, cancellationToken);

                if (nextStepId.HasValue)
                {
                    if (statusEnum == StatusEnum.Rejected)
                    {
                        var lastOrRejectStepId = await workFlowStepRepository.GetLastStepId(WorkFlowEnum.RegisterGuarantor, cancellationToken);
                        await AddAsync(new RequestFacilityGuarantorWorkFlowStep()
                        {
                            OpratorId = opratorId,
                            RequestFacilityGuarantorId = requestFacilityGuarantorId,
                            WorkFlowStepId = nextStepId.Value,
                            StatusId = lastOrRejectStepId.HasValue && lastOrRejectStepId.Value == nextStepId.Value
                                    ? (short?)StatusEnum.Rejected
                                    : null,
                        }, cancellationToken, false);
                    }
                    else
                    {
                        await AddAsync(new RequestFacilityGuarantorWorkFlowStep()
                        {
                            OpratorId = opratorId,
                            RequestFacilityGuarantorId = requestFacilityGuarantorId,
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
