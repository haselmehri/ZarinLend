using Common.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class RequestFacilityCardRechargeModel
    {
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "شناسه تسهیلات")]
        public int RequestFacilityId { get; set; }

        [Display(Name = "درخواست دهنده تسهیلات")]
        public string RequesterFullName { get; set; }

        [Display(Name = "شماره بن کارت")]
        public string CardNumber { get; set; }

        [Display(Name = "شماره حساب")]
        public string AccountNumber { get; set; }

        [Display(Name = "مبلغ تسهیلات(شارژ)")]
        public long Amount { get; set; }
        public Guid CreatorId { get; set; }
        public string ShamsiCurentDate
        {
            get
            {
                return DateTime.Now.GregorianToShamsi();
            }
        }
    }
}
