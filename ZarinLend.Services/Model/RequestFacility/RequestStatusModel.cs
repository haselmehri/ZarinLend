using Common.Utilities;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    public class RequestGuarantorIsRequiredModel
    {
        public int RequestFacilityId { get; set; }
        public bool GuarantorIsRequired { get; set; }
        public string StatusDescription { get; set; }
    }
    public class RequestStatusModel
    {
        public List<int> WorkFlowStepRejectionReasonIds { get; set; }
        public string RejectionReasonDescription { get; set; }
        public int RequestFacilityId { get; set; }
        public int? LeasingId { get; set; }
        public Guid OpratorId { get; set; }
        public StatusEnum Status { get; set; }
        public string StatusDescription { get; set; }
    }

    public class VerifyByZarinLendRequestStatusModel
    {
        public int RequestFacilityId { get; set; }
        public int? LeasingId { get; set; }
        public Guid OpratorId { get; set; }
        public StatusEnum Status { get; set; }
        public string StatusDescription { get; set; }
        public IEnumerable<int> ErrorMessageIds { get; set; }
        public bool? ValidationMustBeRepeated { get; set; }
    }

    public class PrimaryVerifyModel
    {
        public int RequestFacilityId { get; set; }
        public int? LeasingId { get; set; }
        public Guid OpratorId { get; set; }
        public StatusEnum Status { get; set; }
        public string? StatusDescription { get; set; }
    }

    public class EnterFacilityNumberModel
    {
        public List<int> WorkFlowStepRejectionReasonIds { get; set; }
        public string RejectionReasonDescription { get; set; }
        public int RequestFacilityId { get; set; }
        public int? LeasingId { get; set; }
        public Guid OpratorId { get; set; }
        public StatusEnum Status { get; set; }
        public string StatusDescription { get; set; }

        [StringLength(13,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.StringLengthMessage) )]
        [RegularExpression(RegularExpression.FacilityNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        [Display(Name = "FacilityNumber", ResourceType = typeof(ResourceFile))]
        public string FacilityNumber { get; set; }
    }

    public class VerifyAndPoliceNumberModel
    {
        public List<int> WorkFlowStepRejectionReasonIds { get; set; }
        public string RejectionReasonDescription { get; set; }
        public int RequestFacilityId { get; set; }
        public int? LeasingId { get; set; }
        public Guid OpratorId { get; set; }
        public StatusEnum Status { get; set; }
        public string StatusDescription { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(10,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.StringLengthMessage) )]
        [RegularExpression(RegularExpression.PoliceNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        [Display(Name = "PoliceNumber",ResourceType = typeof(ResourceFile))]
        public string PoliceNumber { get; set; }
    }

    public class RequestStatusAndApplicantValidationResultParam
    {
        public PrimaryVerifyModel RequestStatus { get; set; }
        public ApplicantValidationResultModel ApplicantValidationResult { get; set; }
    }
}
