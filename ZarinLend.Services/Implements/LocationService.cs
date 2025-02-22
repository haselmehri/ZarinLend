using AutoMapper;
using Common;
using Common.Utilities;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Services.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class LocationService : ILocationService, IScopedDependency
    {
        private readonly IMapper mapper;
        private readonly ILogger<LocationService> logger;
        private readonly IBaseRepository<Location> locationRepository;

        public LocationService(IMapper mapper, ILogger<LocationService> logger, IBaseRepository<Location> locationRepository)
        {
            this.mapper = mapper;
            this.logger = logger;
            this.locationRepository = locationRepository;
        }

        public async Task<List<SelectListItem>> SelectCityByProvinceId(int provinceId, CancellationToken cancellationToken)
        {
            var cities = (await locationRepository.SelectByAsync(p => p.ParentId.Equals(provinceId) && p.IsActive.Equals(true) &&
            p.LocationType == LocationTypeEnum.City,
                    p => new SelectListItem
                    {
                        Text = p.Name,
                        Value = p.Id.ToString()
                    }, cancellationToken))
                    .OrderBy(p => p.Text)
                    .ToList();

            return cities;
        }

        public async Task<List<LocationModel>> GetAll(CancellationToken cancellationToken)
        {
            var locations = (await locationRepository.SelectByAsync(p => p.IsActive.Equals(true),
                    p => new LocationModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        ParentId = p.ParentId,
                        LocationType = p.LocationType,
                        IsActive = p.IsActive,
                    }, cancellationToken))
                    .ToList();

            return locations;
        }
    }
}
