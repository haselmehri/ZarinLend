using Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Model.IranCreditScoring;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class UserIranCreditScoringHistoryViewComponent : ViewComponent
    {
        private readonly IIranCreditScoringService iranCreditScoringService;

        public UserIranCreditScoringHistoryViewComponent(IIranCreditScoringService iranCreditScoringService)
        {
            this.iranCreditScoringService = iranCreditScoringService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? requestFacilityId, int? requestFacilityGuarantorId,Guid? userId, bool showFileLinks = true, CancellationToken cancellationToken = default)
        {
            ViewBag.ShowFileLinks = showFileLinks;
            IranCreditScoringModel model;
            if (requestFacilityId.HasValue)
                model = await iranCreditScoringService.GetVerifyResult(requestFacilityId.Value, cancellationToken);
            else if (requestFacilityGuarantorId.HasValue)
                model = await iranCreditScoringService.GetVerifyResultByRequestFacilityGaurantor(requestFacilityGuarantorId.Value, cancellationToken);
            else if (userId.HasValue)
                model = await iranCreditScoringService.GetVerifyResult(userId.Value, cancellationToken);
            else
                throw new AppException("UserIranCreditScoringHistoryViewComponent :all three of parameters cannot be null. requestFacilityId,requestFacilityGuarantorId and userId params cannot be null");

            return await Task.FromResult((IViewComponentResult)View("UserIranCreditScoringHistory", model));
        }
    }
}
