using Common.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    public class PoliceNumberResultModel
    {
        [RegularExpression(RegularExpression.NationalCode,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string NationalCode { get; set; }

        [RegularExpression(RegularExpression.PoliceNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string PoliceNumber { get; set; }       

        public string Description { get; set; }

        public int RequestFacilityId { get; set; }
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
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
    }
}
