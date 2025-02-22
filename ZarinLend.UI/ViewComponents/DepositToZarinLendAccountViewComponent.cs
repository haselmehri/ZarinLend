using Microsoft.AspNetCore.Mvc;
using Services.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class DepositToZarinLendAccountViewComponent : ViewComponent
    {
        public DepositToZarinLendAccountViewComponent()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync(RequestFacilityDepositDocumentModel model, CancellationToken cancellationToken)
        {
            if (model != null && !model.DepositDate.HasValue)
                model.DepositDate = DateTime.Now;
            return await Task.FromResult((IViewComponentResult)View("DepositToZarinLendAccount", model));
        }
    }
}
