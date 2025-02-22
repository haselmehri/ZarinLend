using Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class UserIdentityInfoModel : IValidatableObject
    {
        public int PersonId { get; set; }

        [Display(Name = nameof(ResourceFile.ZP_Id), ResourceType = typeof(ResourceFile))]
        public long? ZP_Id { get; set; }
        [Display(Name = nameof(ResourceFile.UserName), ResourceType = typeof(ResourceFile))]
        public string UserName { get; set; }

        [Display(Name = nameof(ResourceFile.FName), ResourceType = typeof(ResourceFile))]
        public string FName { get; set; }

        [Display(Name = nameof(ResourceFile.LName), ResourceType = typeof(ResourceFile))]
        public string LName { get; set; }

        [Display(Name = nameof(ResourceFile.FatherName), ResourceType = typeof(ResourceFile))]
        public string FatherName { get; set; }

        [Display(Name = nameof(ResourceFile.SSID), ResourceType = typeof(ResourceFile))]
        public string SSID { get; set; }

        //[Display(Name = "BirthCertificateSerial")]
        //public string? BirthCertificateSerial { get; set; }

        [Display(Name = nameof(ResourceFile.BirthDate), ResourceType = typeof(ResourceFile))]
        public DateTime BirthDate { get; set; }

        [Display(Name = nameof(ResourceFile.BirthDate), ResourceType = typeof(ResourceFile))]
        public string ShamsiBirthDate
        {
            get
            {
                return BirthDate.GregorianToShamsi();
            }
        }

        [Display(Name = nameof(ResourceFile.Address), ResourceType = typeof(ResourceFile))]
        public string? Address { get; set; }

        public string? ValidatedAddress { get; set; }

        [Display(Name = nameof(ResourceFile.Mobile), ResourceType = typeof(ResourceFile))]
        public string Mobile { get; set; }

        [Display(Name = nameof(ResourceFile.PhoneNumber), ResourceType = typeof(ResourceFile))]
        public string? PhoneNumber { get; set; }

        [Display(Name = nameof(ResourceFile.NationalCode), ResourceType = typeof(ResourceFile))]
        public string NationalCode { get; set; }

        [Display(Name = nameof(ResourceFile.Email), ResourceType = typeof(ResourceFile))]
        public string? Email { get; set; }

        [Display(Name = nameof(ResourceFile.PostalCode), ResourceType = typeof(ResourceFile))]
        public string? PostalCode { get; set; }

        //[Display(Name = "BirthPlaceProvince")]
        //public string BirthPlaceProvince { get; set; }

        //[Display(Name = "BirthPlaceCity")]
        //public string BirthPlaceCity { get; set; }

        //[Display(Name = "ProvinceOfIssue")]
        //public string ProvinceOfIssue { get; set; }

        //[Display(Name = "CityOfIssue")]
        //public string CityOfIssue { get; set; }

        [Display(Name = nameof(ResourceFile.AddressProvinceName), ResourceType = typeof(ResourceFile))]
        public string AddressProvinceName { get; set; }

        [Display(Name = nameof(ResourceFile.AddressCityName), ResourceType = typeof(ResourceFile))]
        public string AddressCityName { get; set; }

        public string PersonSanaTrackingId { get; set; }
        public bool IsSanaInquiryVisible { get; set; }
        public bool IsAddressInquiryVisible { get; set; }

        #region Bank Info
        [Display(Name = nameof(ResourceFile.AccountNumber), ResourceType = typeof(ResourceFile))]
        public string? AccountNumber { get; set; }

        [Display(Name = nameof(ResourceFile.CardNumber), ResourceType = typeof(ResourceFile))]
        public string? CardNumber { get; set; }

        [Display(Name = nameof(ResourceFile.CustomerNumber), ResourceType = typeof(ResourceFile))]
        public string CustomerNumber { get; set; }

        public string? HashCardNumber { get; set; }

        [Display(Name = nameof(ResourceFile.IBAN), ResourceType = typeof(ResourceFile))]
        public string? IBAN { get; set; }

        [Display(Name = nameof(ResourceFile.BankName), ResourceType = typeof(ResourceFile))]
        public string? BankName { get; set; }

        [Display(Name = nameof(ResourceFile.DepositOwners), ResourceType = typeof(ResourceFile))]
        public string? DepositOwners { get; set; }

        [Display(Name = nameof(ResourceFile.PlaceOfIssue), ResourceType = typeof(ResourceFile))]
        public string? PlaceOfBirth { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!NationalCodeHelper.IsValidIranianNationalCode(NationalCode))
                yield return new ValidationResult("فرمت کد ملی صحیح نمی باشد!");

            yield return ValidationResult.Success!;
        }

    }
}
