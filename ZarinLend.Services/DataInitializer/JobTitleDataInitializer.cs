using Core.Data.Repositories;
using Core.Entities;
using Services.Contracts;
using System.Linq;

namespace Services.DataInitializer
{
    public class JobTitleDataInitializer : IDataInitializer
    {
        private readonly IBaseRepository<JobTitle> jobTitleRepository;

        public JobTitleDataInitializer(IBaseRepository<JobTitle> jobTitleRepository)
        {
            this.jobTitleRepository = jobTitleRepository;
        }

        public int Order => 1;
        public void InitializeData()
        {
            if (!jobTitleRepository.TableNoTracking.Any(p => p.Id == 1))
            {
                jobTitleRepository.Add(new JobTitle
                {
                    Id = 1,
                    Title = "کارمند دولتی"                    
                });
            }
            if (!jobTitleRepository.TableNoTracking.Any(p => p.Id == 2))
            {
                jobTitleRepository.Add(new JobTitle
                {
                    Id = 2,
                    Title = "شغل آزاد"
                });
            }
            if (!jobTitleRepository.TableNoTracking.Any(p => p.Id == 3))
            {
                jobTitleRepository.Add(new JobTitle
                {
                    Id = 3,
                    Title = "دانشجو"
                });
            }
        }
    }
}