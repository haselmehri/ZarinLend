using Core.Entities;
using Services.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IWorkFlowStepErrorService
    {
        Task<List<WorkFlowStepErrorModel>> GetWorkFlowDefaultErrors(WorkFlowEnum workFlow, CancellationToken cancellationToken);
    }
}