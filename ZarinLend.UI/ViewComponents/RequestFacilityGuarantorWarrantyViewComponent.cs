using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class RequestFacilityGuarantorWarrantyViewComponent : ViewComponent
    {
        private readonly IRequestFacilityGuarantorWarrantyService requestFacilityGuarantorWarrantyService;

        public RequestFacilityGuarantorWarrantyViewComponent(IRequestFacilityGuarantorWarrantyService requestFacilityGuarantorWarrantyService)
        {
            this.requestFacilityGuarantorWarrantyService = requestFacilityGuarantorWarrantyService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid userId, int requestFacilityGuarantorId, CancellationToken cancellationToken)
        {
            return await Task.FromResult((IViewComponentResult)View("ShowRequestFacilityGuarantorWarranty",
                await requestFacilityGuarantorWarrantyService.GetWarantyDocument(userId, requestFacilityGuarantorId, cancellationToken)));
        }
    }
}
