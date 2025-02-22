using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Model
{
    [Serializable]
    public class RequestFacilityGuarantorDetailModel
    {
        public int? RequestFacilityGuarantorId { get; set; }
        public string? GuarantorFullName { get; set; }
        public bool CancelByUser { get; set; }
        public List<WorkFlowStepListModel>? RequestFacilityGuarantorWorkFlowStepList { get; set; }
        public string LastStatusDescription
        {
            get
            {
                if (RequestFacilityGuarantorWorkFlowStepList != null && RequestFacilityGuarantorWorkFlowStepList.Any())
                {
                    if (RequestFacilityGuarantorWorkFlowStepList.Any(p => p.IsApproveFinalStep && p.IsLastStep))
                    {
                        if (RequestFacilityGuarantorWorkFlowStepList.Any(p => p.IsApproveFinalStep && 
                                                                              p.IsLastStep && 
                                                                              p.StatusId.HasValue && 
                                                                              p.StatusId.Value == (short)StatusEnum.Approved))
                            return "تایید نهایی ضامن";
                        else
                            return RequestFacilityGuarantorWorkFlowStepList.First(p => p.IsApproveFinalStep && p.IsLastStep).WorkFlowStepName;
                    }
                    else if (RequestFacilityGuarantorWorkFlowStepList.Any(p => p.IsLastStep))
                        return RequestFacilityGuarantorWorkFlowStepList.First(p => p.IsLastStep).WorkFlowStepName;
                    else if (RequestFacilityGuarantorWorkFlowStepList.Any(p => !p.StatusId.HasValue))
                        return RequestFacilityGuarantorWorkFlowStepList.First(p => !p.StatusId.HasValue).WorkFlowStepName;
                    else
                        return RequestFacilityGuarantorWorkFlowStepList.OrderByDescending(p => p.CreateDate).First().WorkFlowStepName;
                }
                else
                    return null;
            }
        }
    }
}
