using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class WorkFlowStepErrorService : IWorkFlowStepErrorService, IScopedDependency
    {
        private readonly ILogger<WorkFlowStepErrorService> logger;
        private readonly IBaseRepository<WorkFlowStepError> workFlowStepErrorRepository;

        public WorkFlowStepErrorService(ILogger<WorkFlowStepErrorService> logger, IBaseRepository<WorkFlowStepError> workFlowStepErrorRepository)
        {
            this.logger = logger;
            this.workFlowStepErrorRepository = workFlowStepErrorRepository;
        }


        public async Task<List<WorkFlowStepErrorModel>> GetWorkFlowDefaultErrors(WorkFlowEnum workFlow, CancellationToken cancellationToken)
        {
            return await workFlowStepErrorRepository.TableNoTracking
                .Where(p => p.WorkFlowStep.WorkFlowId == (int)workFlow && p.WorkFlowDefaultError.IsActive)
                .OrderBy(p => p.WorkFlowStepId)
                .Select(p => new WorkFlowStepErrorModel()
                {
                    Id = p.Id,
                    //Message = $"{p.WorkFlowDefaultError.Message}(مرحله : {p.WorkFlowStep.Name})"
                    Message = p.WorkFlowDefaultError.Message,
                    StepName = p.WorkFlowStep.Name
                })
                .ToListAsync(cancellationToken);
        }
    }
}
