using Common;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class WorkFlowStepWizardVerticalViewComponent : ViewComponent
    {
        private readonly IWorkFlowStepService workFlowStepService;
        private readonly IRequestFacilityService requestFacilityService;

        public WorkFlowStepWizardVerticalViewComponent(IWorkFlowStepService workFlowStepService, IRequestFacilityService requestFacilityService)
        {
            this.workFlowStepService = workFlowStepService;
            this.requestFacilityService = requestFacilityService;
        }

        public async Task<IViewComponentResult> InvokeAsync(WorkFlowEnum workFlow, int? requestFacilityId = null, CancellationToken cancellationToken = default)
        {
            var model = new RequestFacilityStepWizardModel()
            {
                WorkFlowSteps = await workFlowStepService.GetWorkFlowAllSteps(workFlow, cancellationToken),
                CompletedWorkFlowStepModels = requestFacilityId.HasValue
                    ? await requestFacilityService.GetRequestFacilitySteps(requestFacilityId.Value, cancellationToken)
                    : null
            };
            return await Task.FromResult((IViewComponentResult)View("StepWizardVertical", model));
        }
    }
}
