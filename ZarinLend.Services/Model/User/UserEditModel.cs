using Common.Utilities;
using Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class UserEditModel
    {
        public Guid Id { get; set; }
        public bool EditByAdmin { get; set; }
        public int? RequestFacilityId { get; set; }
        public int? RequestFacilityGuarantorId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(100)]
        [Display(Name = nameof(ResourceFile.UserName), ResourceType = typeof(ResourceFile))]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.PersianCharacter, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(200)]
        [Display(Name = nameof(ResourceFile.FName), ResourceType = typeof(ResourceFile))]
        public string FName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.PersianCharacter, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(200)]
        [Display(Name = "LName", ResourceType = typeof(ResourceFile))]
        public string LName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.PersianCharacter, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(200)]
        [Display(Name = "FatherName", ResourceType = typeof(ResourceFile))]
        public string FatherName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(10)]
        [Display(Name = "SSID", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.SSID, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string SSID { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[StringLength(6)]
        //[Display(Name = "BirthCertificateSerial")]
        //[RegularExpression(RegularExpression.BirthCertificateSerial,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        //public string? BirthCertificateSerial { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "Gender", ResourceType = typeof(ResourceFile))]
        public GenderEnum? Gender { get; set; }

        [StringLength(500)]
        [Display(Name = "Address", ResourceType = typeof(ResourceFile))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string? Address { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(11)]
        [Display(Name = "Mobile", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.Mobile, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string Mobile { get; set; }

        [StringLength(100)]
        [Display(Name = "شغل")]
        public string? JobTitle { get; set; }

        [StringLength(100)]
        [Display(Name = "میزان درآمد")]
        public string? SalaryRangeTitle { get; set; }

        [StringLength(11)]
        [Display(Name = "PhoneNumber", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.PhoneNumber, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string? PhoneNumber { get; set; }

        [StringLength(50)]
        [Display(Name = "تلفن محل کار")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string? JobPhoneNumber { get; set; }

        [StringLength(500)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "آدرس محل کار")]
        public string? JobAddress { get; set; }

        [Display(Name = "BirthDate", ResourceType = typeof(ResourceFile))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public DateTime BirthDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(10, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.StringLengthMessage))]
        [MaxLength(10, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.StringLengthMessage))]
        [Display(Name = "NationalCode", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.NationalCode, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string NationalCode { get; set; }

        [Display(Name = "Email", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.Email, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string? Email { get; set; }

        [Display(Name = "PostalCode", ResourceType = typeof(ResourceFile))]
        [StringLength(10)]
        [RegularExpression(RegularExpression.PostalCode, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string? PostalCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "Province", ResourceType = typeof(ResourceFile))]
        public int? ProvinceId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "City", ResourceType = typeof(ResourceFile))]
        public int? CityId { get; set; }

        #region Bank Info

        [Display(Name = "CustomerNumber", ResourceType = typeof(ResourceFile))]
        [StringLength(15)]
        [RegularExpression(RegularExpression.CustomerNumber, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string? CustomerNumber { get; set; }

        [Display(Name = "AccountNumber", ResourceType = typeof(ResourceFile))]
        [StringLength(20)]
        [RegularExpression(RegularExpression.AccountNumber, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string? AccountNumber { get; set; }

        [Display(Name = "ClientId", ResourceType = typeof(ResourceFile))]
        [StringLength(64)]
        [RegularExpression(RegularExpression.ClientId, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string ClientId { get; set; }

        public string? CardNumberWithoutDash { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(20)]
        [Display(Name = "CardNumber", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.BankCardNumberWithDash, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string? CardNumber
        {
            get
            {
                long cardNumber;
                return !string.IsNullOrEmpty(CardNumberWithoutDash) && CardNumberWithoutDash.Length == 16 && long.TryParse(CardNumberWithoutDash, out cardNumber) ? cardNumber.ToString("####-####-####-####") : null;
            }
        }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(26)]
        [Display(Name = "IBAN", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.IBAN, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string? IBAN { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "DepositStatus", ResourceType = typeof(ResourceFile))]
        public string? DepositStatus { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "DepositOwners", ResourceType = typeof(ResourceFile))]
        public string? DepositOwners { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(200,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.StringLengthMessage) )]
        [Display(Name = "BankName", ResourceType = typeof(ResourceFile))]
        public string? BankName { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "Bank", ResourceType = typeof(ResourceFile))]
        public int? BankId { get; set; }

        [Display(Name = "Bank", ResourceType = typeof(ResourceFile))]
        public IEnumerable<SelectListItem>? Banks { get; set; }

        #endregion

        [Display(Name = "Province", ResourceType = typeof(ResourceFile))]
        public IEnumerable<SelectListItem> Provinces { get; set; }

        //[Display(Name = "ProvinceOfBirth", ResourceType = typeof(ResourceFile))]
        //public IEnumerable<SelectListItem> BirthProvinces { get; set; }

        //[Display(Name = "IssueProvinces", ResourceType = typeof(ResourceFile))]
        //public IEnumerable<SelectListItem> IssueProvinces { get; set; }

        [Display(Name = "City", ResourceType = typeof(ResourceFile))]
        public IEnumerable<SelectListItem> Cities { get; set; }

        //[Display(Name = "CityOfBirth", ResourceType = typeof(ResourceFile))]
        //public IEnumerable<SelectListItem> BirthCities { get; set; }

        //[Display(Name = "IssueCities", ResourceType = typeof(ResourceFile))]
        //public IEnumerable<SelectListItem> IssueCities { get; set; }

        //[Display(Name = "میزان درآمد")]
        //public IEnumerable<SelectListItem> SalaryRanges { get; set; }

        //[Display(Name = "شغل")]
        //public IEnumerable<SelectListItem> JobTitles { get; set; }

        public string? ReturnUrl { get; set; }

        public bool IsEditMode { get; set; } = false;

        public string UserId { get; set; }
    }

    [Serializable]
    public class UserInfoEditModel
    {
        public Guid Id { get; set; }
        public int? RequestFacilityId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(100)]
        [Display(Name = "UserName", ResourceType = typeof(ResourceFile))]
        public required string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.PersianCharacter, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(200)]
        [Display(Name = "FName", ResourceType = typeof(ResourceFile))]
        public required string FName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.PersianCharacter, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(200)]
        [Display(Name = "LName", ResourceType = typeof(ResourceFile))]
        public required string LName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.PersianCharacter, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(200)]
        [Display(Name = "FatherName", ResourceType = typeof(ResourceFile))]
        public required string FatherName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(10)]
        [Display(Name = "SSID", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.SSID, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public required string SSID { get; set; }

        //[StringLength(20)]
        //[Display(Name = "شماره ثنا")]
        //public string? SanaNumber { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[StringLength(6)]
        //[Display(Name = "BirthCertificateSerial")]
        //[RegularExpression(RegularExpression.BirthCertificateSerial,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        //public required string BirthCertificateSerial { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "Gender", ResourceType = typeof(ResourceFile))]
        public GenderEnum Gender { get; set; }

        [Display(Name = "BirthDate", ResourceType = typeof(ResourceFile))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public DateTime BirthDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(10)]
        [Display(Name = "NationalCode", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.NationalCode, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public required string NationalCode { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[Display(Name = "ProvinceOfBirth")]
        //public int? ProvinceOfBirthId { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[Display(Name = "CityOfBirth")]
        //public int? CityOfBirthId { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[Display(Name = "ProvinceOfIssue")]
        //public int? ProvinceOfIssueId { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[Display(Name = "CityOfIssue")]
        //public int? CityOfIssueId { get; set; }

        //[Display(Name = "ProvinceOfBirth")]
        //public IEnumerable<SelectListItem>? BirthProvinces { get; set; }

        //[Display(Name = "CityOfBirth")]
        //public IEnumerable<SelectListItem>? BirthCities { get; set; }

        public bool IsEditMode { get; set; } = false;
    }

    [Serializable]
    public class UserLocationEditModel
    {
        public Guid Id { get; set; }
        public int? RequestFacilityId { get; set; }

        [StringLength(500)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "Address", ResourceType = typeof(ResourceFile))]
        public required string Address { get; set; }

        public string? VerifiedAddress { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(15)]
        [Display(Name = "Mobile", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.Mobile, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public required string Mobile { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(50)]
        [Display(Name = "PhoneNumber", ResourceType = typeof(ResourceFile))]
        public required string? PhoneNumber { get; set; }

        [Display(Name = "Email", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.Email, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string? Email { get; set; }

        [Display(Name = "PostalCode", ResourceType = typeof(ResourceFile))]
        [StringLength(10)]
        [RegularExpression(RegularExpression.PostalCode, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public required string PostalCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "Province", ResourceType = typeof(ResourceFile))]
        public int ProvinceId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "City", ResourceType = typeof(ResourceFile))]
        public int CityId { get; set; }

        [Display(Name = "Province", ResourceType = typeof(ResourceFile))]
        public IEnumerable<SelectListItem>? Provinces { get; set; }

        [Display(Name = "City", ResourceType = typeof(ResourceFile))]
        public IEnumerable<SelectListItem>? Cities { get; set; }

        public bool IsEditMode { get; set; } = false;
    }

    //[Serializable]
    //public class UserBankAccountEditModel
    //{
    //    public Guid Id { get; set; }
    //    public int? RequestFacilityId { get; set; }

    //    [Display(Name = "CustomerNumber")]
    //    [StringLength(15)]
    //    [RegularExpression(RegularExpression.AccountNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
    //    [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
    //    public string CustomerNumber { get; set; }

    //    [Display(Name = "AccountNumber")]
    //    [StringLength(20)]
    //    [RegularExpression(RegularExpression.AccountNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
    //    [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
    //    public string AccountNumber { get; set; }

    //    [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
    //    [StringLength(20)]
    //    [Display(Name = "CardNumber")]
    //    [RegularExpression(RegularExpression.AyandehBankCardNumberWithDash,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
    //    public string CardNumber { get; set; }

    //    [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
    //    [StringLength(26)]
    //    [Display(Name = "IBAN")]
    //    [RegularExpression(RegularExpression.IBAN,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
    //    public string IBAN { get; set; }

    //    [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
    //    [Display(Name = "DepositStatus")]
    //    public string DepositStatus { get; set; }

    //    [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
    //    [Display(Name = "DepositOwners")]
    //    public string DepositOwners { get; set; }

    //    [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
    //    [StringLength(200,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.StringLengthMessage) )]
    //    [Display(Name = "BankName")]
    //    public string BankName { get; set; }

    //    //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
    //    [Display(Name = "Bank")]
    //    public int? BankId { get; set; }

    //    [Display(Name = "Bank")]
    //    public IEnumerable<SelectListItem> Banks { get; set; }

    //    public string ReturnUrl { get; set; }

    //    public bool IsEditMode { get; set; } = false;
    //}

    [Serializable]
    public class PersonJobEditModel
    {
        public Guid UserId { get; set; }

        [Display(Name = "Address", ResourceType = typeof(ResourceFile))]
        [StringLength(500)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public required string Address { get; set; }

        [StringLength(50)]
        [Display(Name = "PhoneNumber", ResourceType = typeof(ResourceFile))]
        public required string PhoneNumber { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "شغل")]
        public required int JobTitleId { get; set; }
        public IEnumerable<SelectListItem>? JobTitles { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "میزان درآمد")]
        public required int SalaryRangeId { get; set; }
        public IEnumerable<SelectListItem>? SalaryRanges { get; set; }

        public bool IsEditMode { get; set; } = false;
    }

    [Serializable]
    public class PersonJobViewModel
    {

        [Display(Name = "Address", ResourceType = typeof(ResourceFile))]
        public string Address { get; set; }

        [Display(Name = "PhoneNumber", ResourceType = typeof(ResourceFile))]
        public string PhoneNumber { get; set; }

        [Display(Name = "شغل")]
        public string JobTitle { get; set; }

        [Display(Name = "میزان درآمد")]
        public string SalaryRange { get; set; }
    }
}
