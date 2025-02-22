using Core.Data.Repositories;
using Core.Entities;
using Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Services.DataInitializer
{
    public class UsagePlaceDataInitializer : IDataInitializer
    {
        private readonly IBaseRepository<UsagePlace> usagePlaceRepository;

        public UsagePlaceDataInitializer(IBaseRepository<UsagePlace> usagePlaceRepository)
        {
            this.usagePlaceRepository = usagePlaceRepository;
        }

        public int Order => 1;      
        public void InitializeData()
        {
            if (!usagePlaceRepository.TableNoTracking.Any(p => p.Id.Equals(1)))
            {
                usagePlaceRepository.Add(new UsagePlace
                {
                    Id=1,
                    Name = "تلفن همراه و تبلت"
                });
            }

            if (!usagePlaceRepository.TableNoTracking.Any(p => p.Id.Equals(2)))
            {
                usagePlaceRepository.Add(new UsagePlace
                {
                    Id = 2,
                    Name = "لب تاپ و کامپیوتر"
                });
            }

            if (!usagePlaceRepository.TableNoTracking.Any(p => p.Id.Equals(10000)))
            {
                usagePlaceRepository.Add(new UsagePlace
                {
                    Id = 10000,
                    Name = "سایر"
                });
            }
        }
    }
}