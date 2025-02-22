using Core.Data.Repositories;
using Core.Entities;
using Services.Contracts;
using System.Linq;

namespace Services.DataInitializer
{
    public class WorkFlowStepErrorDataInitializer : IDataInitializer
    {
        private readonly IBaseRepository<WorkFlowStepError> workFlowStepErroRepository;

        public WorkFlowStepErrorDataInitializer(IBaseRepository<WorkFlowStepError> workFlowStepErroRepository)
        {
            this.workFlowStepErroRepository = workFlowStepErroRepository;
        }

        public int Order => 3;
        public void InitializeData()
        {
            if (!workFlowStepErroRepository.TableNoTracking.Any(p => p.Id == 1))
            {
                workFlowStepErroRepository.Add(new WorkFlowStepError
                {
                    Id = 1,
                    WorkFlowDefaultErrorId =1,
                    WorkFlowStepId = 3,
                });
            }
            if (!workFlowStepErroRepository.TableNoTracking.Any(p => p.Id == 2))
            {
                workFlowStepErroRepository.Add(new WorkFlowStepError
                {
                    Id = 2,
                    WorkFlowDefaultErrorId = 2,
                    WorkFlowStepId = 3,
                });
            }
            if (!workFlowStepErroRepository.TableNoTracking.Any(p => p.Id == 3))
            {
                workFlowStepErroRepository.Add(new WorkFlowStepError
                {
                    Id = 3,
                    WorkFlowDefaultErrorId = 3,
                    WorkFlowStepId = 3,
                });
            }
            if (!workFlowStepErroRepository.TableNoTracking.Any(p => p.Id == 4))
            {
                workFlowStepErroRepository.Add(new WorkFlowStepError
                {
                    Id = 4,
                    WorkFlowDefaultErrorId = 4,
                    WorkFlowStepId = 4,
                });
            }
            if (!workFlowStepErroRepository.TableNoTracking.Any(p => p.Id == 5))
            {
                workFlowStepErroRepository.Add(new WorkFlowStepError
                {
                    Id = 5,
                    WorkFlowDefaultErrorId = 5,
                    WorkFlowStepId = 4,
                });
            }
        }
    }
}