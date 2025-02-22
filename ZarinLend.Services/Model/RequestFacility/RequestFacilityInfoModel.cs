using Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class RequestFacilityInfoModel
    {
        public int Id { get; set; }
        public WorkFlowFormEnum CurrentStepForm { get; set; }
        public bool WaitingForZarinLend { get; set; } 
        public bool WaitingForLeasing { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "Leasing", ResourceType = typeof(ResourceFile))]
        public int? LeasingId { get; set; }

        [Display(Name = nameof(ResourceFile.OperatorId), ResourceType = typeof(ResourceFile))]
        public Guid? OperatorId { get; set; }

        [Display(Name = "Leasing", ResourceType = typeof(ResourceFile))]
        public IEnumerable<SelectListItem>? Leasings { get; set; }

        #region Request Facility
        public RequestFacilityDetailModel? RequestFacilityDetail { get; set; }
        #endregion

        #region Buyer Identity Documnet
        public IEnumerable<DocumentModel>? BuyerIdentityDocuments { get; set; }

        #endregion

        #region Buyer Identity Info
        public Guid UserId { get; set; }
        public UserIdentityInfoModel UserIdentityInfo { get; set; }
        #endregion

        //public RequestFacilityInsuranceIssuanceModel InsuranceIssuanceModel { get; set; }
        public RequestFacilityCardIssuanceModel? CardIssuanceModel { get; set; }
        public RequestFacilityEditCardIssuanceModel? BonCardEditModel { get; set; }
        public RequestFacilityDepositDocumentModel DepositModel { get; set; }
        public List<WorkFlowStepListModel>? RequestFacilityWorkFlowStepList { get; set; }
        public string LastStatusDescription
        {
            get
            {
                if (RequestFacilityWorkFlowStepList != null && RequestFacilityWorkFlowStepList.Any())
                {
                    if (RequestFacilityWorkFlowStepList.Any(p => p.IsApproveFinalStep && p.IsLastStep))
                    {
                        if (RequestFacilityWorkFlowStepList.Any(p => p.IsApproveFinalStep && p.IsLastStep && p.StatusId.HasValue && p.StatusId.Value == (short)StatusEnum.Approved))
                            return "تایید نهایی-تخصیص اعتبار";
                        else
                            return RequestFacilityWorkFlowStepList.First(p => p.IsApproveFinalStep && p.IsLastStep).WorkFlowStepName;
                    }
                    else if (RequestFacilityWorkFlowStepList.Any(p => p.IsLastStep))
                        return RequestFacilityWorkFlowStepList.First(p => p.IsLastStep).WorkFlowStepName;
                    else if (RequestFacilityWorkFlowStepList.Any(p => !p.StatusId.HasValue))
                        return RequestFacilityWorkFlowStepList.First(p => !p.StatusId.HasValue).WorkFlowStepName;
                    else
                        return RequestFacilityWorkFlowStepList.OrderByDescending(p => p.CreateDate).First().WorkFlowStepName;
                }
                else
                    return null;
            }
        }
    }
}