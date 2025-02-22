using Common;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class RequestFacilityPromissoryViewComponent(IRequestFacilityPromissoryService requestFacilityPromissoryService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Guid userId, int requestFacilityId, CancellationToken cancellationToken)
        {
            return await Task.FromResult((IViewComponentResult)View("ShowRequestFacilityPromissory",
                await requestFacilityPromissoryService.GetFinalizePromissory(userId, requestFacilityId, cancellationToken)));
        }
    }
}
