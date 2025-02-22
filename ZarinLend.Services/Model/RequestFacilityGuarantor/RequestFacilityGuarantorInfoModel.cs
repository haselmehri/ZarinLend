using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Services.Model
{
    [Serializable]
    public class RequestFacilityGuarantorInfoModel
    {
        public int Id { get; set; }
        public WorkFlowFormEnum CurrentStepForm { get; set; }
        public bool WaitingForZarinLend { get; set; } 

        //[Display(Name = "OperatorId")]
        //public Guid? OperatorId { get; set; }

        #region Request Facility
        public RequestFacilityDetailModel RequestFacilityDetail { get; set; }
        #endregion

        #region Guarantor Identity Documnet
        public IEnumerable<DocumentModel>? BuyerIdentityDocuments { get; set; }

        #endregion

        #region Guarantor Identity Info
        public Guid UserId { get; set; }
        public UserIdentityInfoModel UserIdentityInfo { get; set; }
        #endregion
        public List<WorkFlowStepListModel>? RequestFacilityGuarantorWorkFlowStepList { get; set; }
        public string LastStatusDescription
        {
            get
            {
                if (RequestFacilityGuarantorWorkFlowStepList != null && RequestFacilityGuarantorWorkFlowStepList.Any())
                {
                    if (RequestFacilityGuarantorWorkFlowStepList.Any(p => p.IsApproveFinalStep && p.IsLastStep))
                    {
                        if (RequestFacilityGuarantorWorkFlowStepList.Any(p => p.IsApproveFinalStep && p.IsLastStep && p.StatusId.HasValue && p.StatusId.Value == (short)StatusEnum.Approved))
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