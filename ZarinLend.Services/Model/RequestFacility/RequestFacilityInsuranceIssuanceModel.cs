using Common.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class RequestFacilityInsuranceIssuanceModel
    {
        public int RequestFacilityId { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "شماره بیمه")]
        public string InsuranceNumber { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
        public string ShamsiCreatedDate
        {
            get
            {
                return CreatedDate.GregorianToShamsi(showTime: true);
            }
        }
        public Guid CreatorId { get; set; }
        public string Creator { get; set; }
    }
}
