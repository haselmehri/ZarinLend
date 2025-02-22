using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface ILocationService
    {
        Task<List<SelectListItem>> SelectCityByProvinceId(int provinceId, CancellationToken cancellationToken);
        Task<List<LocationModel>> GetAll(CancellationToken cancellationToken);
    }
}