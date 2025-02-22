using Microsoft.AspNetCore.Mvc;
using Services.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class CardIssuanceDetailViewComponent : ViewComponent
    {        
        public CardIssuanceDetailViewComponent()
        {
            
        }

        public async Task<IViewComponentResult> InvokeAsync(RequestFacilityCardIssuanceModel cardIssuance, string facilityNumber, string policeNumber,
            string signedContractByBankFileName,CancellationToken cancellationToken)
        {
            ViewBag.FacilityNumber = facilityNumber;
            ViewBag.PoliceNumber = policeNumber;
            ViewBag.SignedContractByBankFileName = signedContractByBankFileName;
            return await Task.FromResult((IViewComponentResult)View("CardIssuanceDetail", cardIssuance));
        }
    }
}
