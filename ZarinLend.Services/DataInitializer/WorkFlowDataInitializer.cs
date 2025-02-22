using Core.Data.Repositories;
using Core.Entities;
using Services.Contracts;
using System.Linq;

namespace Services.DataInitializer
{
    public class WorkFlowDataInitializer : IDataInitializer
    {
        private readonly IBaseRepository<WorkFlow> workFlowRepository;

        public WorkFlowDataInitializer(IBaseRepository<WorkFlow> workFlowRepository)
        {
            this.workFlowRepository = workFlowRepository;
        }

        public int Order => 1;      
        public void InitializeData()
        {
            if (!workFlowRepository.TableNoTracking.Any(p => p.Id.Equals((int)WorkFlowEnum.RequestFacility)))
            {
                workFlowRepository.Add(new WorkFlow
                {
                    Id = (int)WorkFlowEnum.RequestFacility,
                    Name = "RequestFacility",
                    Description = "تسهیلات با شارژ کیف پول"
                });
            }
            if (!workFlowRepository.TableNoTracking.Any(p => p.Id.Equals((int)WorkFlowEnum.RequestFacility_OldVersion)))
            {
                workFlowRepository.Add(new WorkFlow
                {
                    Id = (int)WorkFlowEnum.RequestFacility_OldVersion,
                    Name = "RequestFacility_OldVersion",
                    Description = "تسهیلات با شارژ بن کارت"
                });
            }
            if (!workFlowRepository.TableNoTracking.Any(p => p.Id.Equals((int)WorkFlowEnum.RegisterGuarantor)))
            {
                workFlowRepository.Add(new WorkFlow
                {
                    Id = (int)WorkFlowEnum.RegisterGuarantor,
                    Name = "RegisterGuarantor",
                    Description = "ثبت ضامن"
                });
            }
        }
    }
}