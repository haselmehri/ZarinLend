using Core.Entities;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IWorkFlowStepService
    {
        Task<List<WorkFlowStepModel>> GetWorkFlowAllSteps(WorkFlowEnum  workFlow, CancellationToken cancellationToken);
    }
}