using Core.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface IWorkFlowStepRepository : IBaseRepository<WorkFlowStep>
    {

        Task<int?> GetFirstStepId(WorkFlowEnum workFlowId, CancellationToken cancellationToken);

        Task<int?> GetFirstStepId(WorkFlowEnum workFlowId, List<RoleEnum> roles, CancellationToken cancellationToken);

        Task<WorkFlowStep> GetFirstStep(WorkFlowEnum workFlowId, List<RoleEnum> roles, CancellationToken cancellationToken);

        Task<WorkFlowStep> GetFirstStep(WorkFlowEnum workFlowId, CancellationToken cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workFlowStepId">curent stepId</param>
        /// <param name="statusEnum"></param>
        /// <param name="roleId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int?> GetNextStepId(int workFlowStepId, StatusEnum statusEnum, List<RoleEnum> roles, CancellationToken cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workFlowStepId">current stepId</param>
        /// <param name="statusEnum"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int?> GetNextStepId(int workFlowStepId, StatusEnum statusEnum, CancellationToken cancellationToken);

        Task<int?> GetLastStepId(WorkFlowEnum workFlowId, CancellationToken cancellationToken);

        Task<int?> GetApproveFinalStepId(WorkFlowEnum workFlowId, CancellationToken cancellationToken);
    }
}