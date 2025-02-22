using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class UserActiveFacilitiesViewComponent : ViewComponent
    {
        private readonly ISamatService samatService;

        public UserActiveFacilitiesViewComponent(ISamatService samatService)
        {
            this.samatService = samatService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int requestFacilityId, CancellationToken cancellationToken)
        {
            var userFacilitiesHistory = await samatService.GetUserFacilitiesFromDB(requestFacilityId, cancellationToken);
            return await Task.FromResult((IViewComponentResult)View("UserActiveFacilities", userFacilitiesHistory));
        }
    }
}
