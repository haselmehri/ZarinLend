using Common;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.Extensions.Logging;
using Services.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class WorkFlowStepService : IWorkFlowStepService, IScopedDependency
    {
        private readonly ILogger<WorkFlowStepService> logger;
        private readonly IWorkFlowStepRepository workFlowStepRepository;

        public WorkFlowStepService(ILogger<WorkFlowStepService> logger, IWorkFlowStepRepository workFlowStepRepository)
        {
            this.logger = logger;
            this.workFlowStepRepository = workFlowStepRepository;
        }

        public async Task<List<WorkFlowStepModel>> GetWorkFlowAllSteps(WorkFlowEnum workFlow, CancellationToken cancellationToken)
        {
            var result = await workFlowStepRepository.SelectByAsync(p => p.WorkFlowId.Equals((int)workFlow) &&
             p.IsActive.Equals(true),
               p => new
               {
                   p.Id,
                   p.IsApproveFinalStep,
                   p.ApproveNextStepId,
                   p.IsFirstStep,
                   p.IsLastStep,
                   p.Name,
                   p.StepIsManual,
               }, cancellationToken);

            var workFlowSteps = new List<WorkFlowStepModel>();
            int? nextStepId = result.First(p => p.IsFirstStep).Id;
            while (nextStepId.HasValue)
            {
                var step = result.First(p => p.Id == nextStepId);
                workFlowSteps.Add(new WorkFlowStepModel()
                {
                    Id = step.Id,
                    Name = step.Name,
                    StepIsManual = step.StepIsManual,
                    IsApproveFinalStep = step.IsApproveFinalStep,
                });

                nextStepId = step.ApproveNextStepId;
            }

            return workFlowSteps;
        }
    }
}
