using Common;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class RequestFacilityWarrantyViewComponent : ViewComponent
    {
        private readonly IRequestFacilityWarrantyService requestFacilityWarrantyService;

        public RequestFacilityWarrantyViewComponent(IRequestFacilityWarrantyService requestFacilityWarrantyService)
        {
            this.requestFacilityWarrantyService = requestFacilityWarrantyService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid userId, int requestFacilityId, CancellationToken cancellationToken)
        {
            return await Task.FromResult((IViewComponentResult)View("ShowRequestFacilityWarranty",
                await requestFacilityWarrantyService.GetRequestFacilityWarantyDocument(userId, requestFacilityId, cancellationToken)));
        }
    }
}
