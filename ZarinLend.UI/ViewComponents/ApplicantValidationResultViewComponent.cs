using Microsoft.AspNetCore.Mvc;
using Services;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class ApplicantValidationResultViewComponent : ViewComponent
    {
        private readonly IApplicantValidationResultService applicantValidationResultService;

        public ApplicantValidationResultViewComponent(IApplicantValidationResultService applicantValidationResultService)
        {
            this.applicantValidationResultService = applicantValidationResultService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int requestFacilityId, int? leasingId, CancellationToken cancellationToken)
        {
           
            var validationResult = leasingId.HasValue
                 ? await applicantValidationResultService.SelectValidationResult(requestFacilityId, leasingId.Value, cancellationToken)
                 : await applicantValidationResultService.SelectValidationResult(requestFacilityId, cancellationToken);

            return await Task.FromResult((IViewComponentResult)View("ApplicantValidationResult", validationResult));
        }
    }
}
