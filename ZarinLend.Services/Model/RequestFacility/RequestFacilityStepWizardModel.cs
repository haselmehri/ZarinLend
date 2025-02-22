using System.Collections.Generic;

namespace Services.Model
{
    public class RequestFacilityStepWizardModel
    {
        public List<CompletedWorkFlowStepModel> CompletedWorkFlowStepModels { get; set; }
        public List<WorkFlowStepModel> WorkFlowSteps { get; set; }
    }
}
