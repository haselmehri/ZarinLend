using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class LocationController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ILocationService locationService;

        public LocationController(IMapper mapper, ILocationService locationService)
        {
            _mapper = mapper;
            this.locationService = locationService;
        }

        [HttpGet("[action]/{provinceId:int}")]
        [AllowAnonymous]
        public virtual async Task<List<SelectListItem>> SelectCityByProvince(int provinceId, CancellationToken cancellationToken)
        {
            return await locationService.SelectCityByProvinceId(provinceId, cancellationToken);
        }
    }
}
