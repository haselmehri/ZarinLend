using Microsoft.AspNetCore.Mvc;
using Services.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class CardRechargeViewComponent : ViewComponent
    {
        public CardRechargeViewComponent()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync(RequestFacilityCardRechargeModel model, CancellationToken cancellationToken)
        {
            return await Task.FromResult((IViewComponentResult)View("CardRecharge", model));
        }
    }
}
