using Common;
using Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public class WorkFlowStepRepository : BaseRepository<WorkFlowStep>, IWorkFlowStepRepository, IScopedDependency
    {
        public WorkFlowStepRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<int?> GetFirstStepId(WorkFlowEnum workFlowId, CancellationToken cancellationToken)
        {
            var result = (await SelectByAsync(p => p.WorkFlow.IsActive &&
                p.WorkFlowId.Equals((int)workFlowId) &&
                p.IsActive.Equals(true) &&
                p.IsFirstStep,
                p => new
                {
                    p.Id,
                }, cancellationToken))
                .FirstOrDefault();

            return result?.Id;
        }

        public async Task<int?> GetFirstStepId(WorkFlowEnum workFlow, List<RoleEnum> roles, CancellationToken cancellationToken)
        {
            var stringRoles = roles.Select(p => p.ToString());

            var result = (await SelectByAsync(p => p.WorkFlowId.Equals((int)workFlow) &&
                p.IsActive.Equals(true) &&
                p.IsFirstStep &&
                p.WorkFlowStepRoles.Any(x => stringRoles.Any(c => c == x.Role.Name)),
                p => new
                {
                    p.Id,
                }, cancellationToken))
                .FirstOrDefault();

            return result?.Id;
        }

        public async Task<WorkFlowStep> GetFirstStep(WorkFlowEnum workFlowId, List<RoleEnum> roles, CancellationToken cancellationToken)
        {
            var stringRoles = roles.Select(p => p.ToString());

            var workFlowStep = (await SelectByAsync(p => p.WorkFlow.IsActive &&
                p.WorkFlowId.Equals((int)workFlowId) &&
                p.IsActive.Equals(true) &&
                p.IsFirstStep &&
                p.WorkFlowStepRoles.Any(x => stringRoles.Any(c => c == x.Role.Name)),
                cancellationToken))
                .FirstOrDefault();

            return workFlowStep;
        }

        public async Task<WorkFlowStep> GetFirstStep(WorkFlowEnum workFlowId, CancellationToken cancellationToken)
        {
            var workFlowStep = (await SelectByAsync(p => p.WorkFlow.IsActive &&
                p.WorkFlowId.Equals((int)workFlowId) &&
                p.IsActive.Equals(true) &&
                p.IsFirstStep,
                cancellationToken))
                .FirstOrDefault();

            return workFlowStep;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workFlowStepId">curent stepId</param>
        /// <param name="statusEnum"></param>
        /// <param name="roleId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int?> GetNextStepId(int workFlowStepId, StatusEnum statusEnum, List<RoleEnum> roles, CancellationToken cancellationToken)
        {
            var stringRoles = roles.Select(p => p.ToString());

            var workFlowStep = (await SelectByAsync(p => p.IsActive &&
                p.Id.Equals(workFlowStepId) &&
                p.IsActive.Equals(true) &&
                p.WorkFlowStepRoles.Any(x => stringRoles.Any(c => c == x.Role.Name)),
                cancellationToken))
                .FirstOrDefault();

            if (workFlowStep != default(WorkFlowStep))
            {
                switch (statusEnum)
                {
                    case StatusEnum.ReturnToCorrection:
                        return workFlowStep.ReturnToCorrectionNextStepId;
                    case StatusEnum.Approved:
                        return workFlowStep.ApproveNextStepId;
                    case StatusEnum.Rejected:
                        return workFlowStep.RejectNextStepId;
                    default:
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workFlowStepId">current stepId</param>
        /// <param name="statusEnum"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int?> GetNextStepId(int workFlowStepId, StatusEnum statusEnum, CancellationToken cancellationToken)
        {
            var workFlowStep = (await SelectByAsync(p => p.IsActive &&
                p.Id.Equals(workFlowStepId) &&
                p.IsActive.Equals(true),
                cancellationToken))
                .FirstOrDefault();

            if (workFlowStep != default(WorkFlowStep))
            {
                switch (statusEnum)
                {
                    case StatusEnum.ReturnToCorrection:
                        return workFlowStep.ReturnToCorrectionNextStepId;
                    case StatusEnum.Approved:
                        return workFlowStep.ApproveNextStepId;
                    case StatusEnum.Rejected:
                        return workFlowStep.RejectNextStepId;
                    default:
                        break;
                }
            }

            return null;
        }

        public async Task<int?> GetLastStepId(WorkFlowEnum workFlowId, CancellationToken cancellationToken)
        {
            var workFlowStep = (await SelectByAsync(p => p.WorkFlowId.Equals((int)workFlowId) &&
                p.IsLastStep &&
                !p.IsApproveFinalStep &&
                p.IsActive.Equals(true),
                p => new
                {
                    p.Id,
                },
                cancellationToken))
                .FirstOrDefault();

            return workFlowStep?.Id;
        }

        public async Task<int?> GetApproveFinalStepId(WorkFlowEnum workFlowId, CancellationToken cancellationToken)
        {
            var workFlowStep = (await SelectByAsync(p => p.WorkFlowId.Equals((int)workFlowId) &&
                p.IsApproveFinalStep &&
                p.IsActive.Equals(true),
                p => new
                {
                    p.Id,
                },
                cancellationToken))
                .FirstOrDefault();

            return workFlowStep?.Id;
        }
    }
}
