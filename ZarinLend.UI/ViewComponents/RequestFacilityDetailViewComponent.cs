using Microsoft.AspNetCore.Mvc;
using Services.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class RequestFacilityDetailViewComponent : ViewComponent
    {        
        public RequestFacilityDetailViewComponent()
        {
            
        }

        public async Task<IViewComponentResult> InvokeAsync(RequestFacilityDetailModel requestFacilityDetail, CancellationToken cancellationToken)
        {
            return await Task.FromResult((IViewComponentResult)View("RequestFacilityDetail", requestFacilityDetail));
        }
    }
}
