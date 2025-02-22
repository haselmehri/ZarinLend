using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class WorkFlowStepWizardHorizentalViewComponent : ViewComponent
    {
        private readonly IWorkFlowStepService workFlowStepService;
        private readonly IRequestFacilityService requestFacilityService;
        private readonly IRequestFacilityGuarantorService requestFacilityGuarantorService;

        public WorkFlowStepWizardHorizentalViewComponent(IWorkFlowStepService workFlowStepService, IRequestFacilityService requestFacilityService,
            IRequestFacilityGuarantorService requestFacilityGuarantorService)
        {
            this.workFlowStepService = workFlowStepService;
            this.requestFacilityService = requestFacilityService;
            this.requestFacilityGuarantorService = requestFacilityGuarantorService;
        }

        public async Task<IViewComponentResult> InvokeAsync(WorkFlowEnum workFlow, int? requestId = null, CancellationToken cancellationToken = default)
        {
            List<CompletedWorkFlowStepModel> completedWorkFlowSteps = null;
            if (requestId.HasValue)
            {
                switch (workFlow)
                {
                    case WorkFlowEnum.RequestFacility:
                        completedWorkFlowSteps = await requestFacilityService.GetRequestFacilitySteps(requestId.Value, cancellationToken);
                        break;
                    case WorkFlowEnum.RegisterGuarantor:
                        completedWorkFlowSteps = await requestFacilityGuarantorService.GetRequestFacilityGuarantorSteps(requestId.Value, cancellationToken);
                        break;
                }
            }
            var model = new RequestFacilityStepWizardModel()
            {
                WorkFlowSteps = await workFlowStepService.GetWorkFlowAllSteps(workFlow, cancellationToken),
                CompletedWorkFlowStepModels = completedWorkFlowSteps
            };
            return await Task.FromResult((IViewComponentResult)View("StepWizardHorizental", model));
        }
    }
}
