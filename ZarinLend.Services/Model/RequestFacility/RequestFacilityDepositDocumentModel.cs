using Common.Utilities;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class RequestFacilityDepositDocumentModel
    {
        public List<int> WorkFlowStepRejectionReasonIds { get; set; }
        public string RejectionReasonDescription { get; set; }
        public StatusEnum Status { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "شناسه تسهیلات")]
        public int RequestFacilityId { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "شماره سند")]
        public string? DepositDocumentNumber { get; set; }

        [Display(Name = "تصویر فیش واریزی")]
        public string? DepositDocumentFileName { get; set; }

        [Display(Name = "تاریخ واریز")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public DateTime? DepositDate { get; set; }
        public string? ShamsiDepositDate
        {
            get
            {
                return DepositDate.HasValue ? DepositDate.Value.GregorianToShamsi() : string.Empty;
            }
        }

        public Guid CreatorId { get; set; }

        public string? StepDescription { get; set; }
    }
}
