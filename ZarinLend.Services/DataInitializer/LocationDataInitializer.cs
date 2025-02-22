using Core.Data.Repositories;
using Core.Entities;
using Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Services.DataInitializer
{
    public class LocationDataInitializer : IDataInitializer
    {
        private readonly IBaseRepository<Location> locationRepository;

        public LocationDataInitializer(IBaseRepository<Location> locationRepository)
        {
            this.locationRepository = locationRepository;
        }

        public int Order => 1;      
        public void InitializeData()
        {
            if (!locationRepository.TableNoTracking.Any(p => p.Name == "تهران"))
            {
                locationRepository.Add(new Location
                {
                   Name = "تهران",
                   LocationType = LocationTypeEnum.Province,
                   Childs = new List<Location>()
                   {
                       new Location(){Name="تهران", LocationType = LocationTypeEnum.City}
                   }
                });
            }            
        }
    }
}