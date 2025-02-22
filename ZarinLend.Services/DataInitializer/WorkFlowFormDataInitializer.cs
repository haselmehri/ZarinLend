using Common.Utilities;
using Core.Data.Repositories;
using Core.Entities;
using Services.Contracts;
using System.Linq;

namespace Services.DataInitializer
{
    public class WorkFlowFormDataInitializer : IDataInitializer
    {
        private readonly IBaseRepository<WorkFlowForm> workFlowFormRepository;

        public WorkFlowFormDataInitializer(IBaseRepository<WorkFlowForm> workFlowFormRepository)
        {
            this.workFlowFormRepository = workFlowFormRepository;
        }

        public int Order => 0;
        public void InitializeData()
        {
            foreach (var item in WorkFlowFormEnum.DefaultForm.ToDictionary())
            {
                if (!workFlowFormRepository.TableNoTracking.Any(p => p.Id == (WorkFlowFormEnum)item.Key))
                {
                    workFlowFormRepository.Add(new WorkFlowForm
                    {
                        Id = (WorkFlowFormEnum)item.Key,
                        Title = item.Value,
                        Url = ((WorkFlowFormEnum)item.Key).ToDisplay(DisplayProperty.Description),
                    });
                }
            }
        }
    }
}