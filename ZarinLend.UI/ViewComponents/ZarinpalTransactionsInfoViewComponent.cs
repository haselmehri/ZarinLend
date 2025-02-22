using Microsoft.AspNetCore.Mvc;
using Services;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class ZarinpalTransactionsInfoViewComponent : ViewComponent
    {
        private readonly IPersonService personService;

        public ZarinpalTransactionsInfoViewComponent(IPersonService personService)
        {
            this.personService = personService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string hashCardNumber, CancellationToken cancellationToken)
        {
            return await Task.FromResult((IViewComponentResult)View("ZarinpalTransactionsInfo",
                await personService.GetZarinpalTransactionInfo(hashCardNumber, cancellationToken)));
        }
    }
}
