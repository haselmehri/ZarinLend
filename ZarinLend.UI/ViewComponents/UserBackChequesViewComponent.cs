using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class UserBackChequesViewComponent : ViewComponent
    {
        private readonly ISamatService samatService;

        public UserBackChequesViewComponent(ISamatService samatService)
        {
            this.samatService = samatService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int requestFacilityId, CancellationToken cancellationToken)
        {
            var userBackCheques = await samatService.GetUserBackChequesFromDB(requestFacilityId, cancellationToken);
            return await Task.FromResult((IViewComponentResult)View("UserBackCheques", userBackCheques));
        }
    }
}
