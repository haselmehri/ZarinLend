using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class RequestFacilityWorkFlowStepsHistoryViewComponent : ViewComponent
    {
        private readonly IRequestFacilityService requestFacilityService;

        public RequestFacilityWorkFlowStepsHistoryViewComponent(IRequestFacilityService requestFacilityService)
        {
            this.requestFacilityService = requestFacilityService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int requestFacilityId, CancellationToken cancellationToken)
        {
            var facilityStepsHistory = await requestFacilityService.GetRequestFacilityStepsHistory(requestFacilityId, cancellationToken);
            return await Task.FromResult((IViewComponentResult)View("RequestFacilityWorkFlowStepsHistory", facilityStepsHistory));
        }
    }
}
