using Services.Model.WalletTransaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class UserSearchResultModel
    {
        public Guid Id { get; set; }

        [Display(Name = "FName", ResourceType = typeof(ResourceFile))]
        public string FName { get; set; }

        [Display(Name = "LName", ResourceType = typeof(ResourceFile))]
        public string LName { get; set; }


        [Display(Name = "Mobile", ResourceType = typeof(ResourceFile))]
        public string Mobile { get; set; }

        [Display(Name = "PhoneNumber", ResourceType = typeof(ResourceFile))]
        public string PhoneNumber { get; set; }

        [Display(Name = "NationalCode", ResourceType = typeof(ResourceFile))]
        public string NationalCode { get; set; }

        public List<WalletBalanceBaseCardNumber> WalletBalanceBaseCardNumber { get; set; }
    }
}