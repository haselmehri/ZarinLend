using Common.Utilities;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class UserRegisterByZarinpalModel : IValidatableObject
    {
        public bool LogiFromZarinpal { get; set; }
        public long ZP_Id { get; set; }
        public bool? VerifiedPostCode { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(100)]
        [Display(Name = "UserName", ResourceType = typeof(ResourceFile))]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(200, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.StringLengthMessage) )]
        [RegularExpression(RegularExpression.PersianCharacter,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "FName", ResourceType = typeof(ResourceFile))]
        public string FName { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.PersianCharacter,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(200)]
        [Display(Name = "LName", ResourceType = typeof(ResourceFile))]
        public string LName { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[RegularExpression(RegularExpression.PersianCharacter,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[StringLength(200)]
        //[Display(Name = "FatherName")]
        //public string FatherName { get; set; }

        [StringLength(500)]
        [Display(Name = "Address", ResourceType = typeof(ResourceFile))]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string Address { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[StringLength(10)]
        //[Display(Name = "SSID")]
        //[RegularExpression(RegularExpression.SSID,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        //public string SSID { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[StringLength(6)]
        //[Display(Name = "BirthCertificateSerial")]
        //[RegularExpression(RegularExpression.BirthCertificateSerial,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        //public string BirthCertificateSerial { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[Display(Name = "Gender")]
        //public GenderEnum Gender { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(15)]
        [Display(Name = "Mobile", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.Mobile,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string Mobile { get; set; }

        [StringLength(50)]
        [Display(Name = "PhoneNumber", ResourceType = typeof(ResourceFile))]
        public string PhoneNumber { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(10)]
        [Display(Name = "NationalCode", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.NationalCode,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string NationalCode { get; set; }

        [Display(Name = "Email", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.Email,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string Email { get; set; }

        [Display(Name = "PostalCode", ResourceType = typeof(ResourceFile))]
        [StringLength(10)]
        [RegularExpression(RegularExpression.PostalCode,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string PostalCode { get; set; }

        [Display(Name = "BirthDate", ResourceType = typeof(ResourceFile))]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public DateTime BirthDate { get; set; }

        //[Display(Name = "BirthDate")]
        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //public string ShamsiBirthDate { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[Display(Name = "ProvinceOfBirth")]
        //public int ProvinceOfBirthId { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[Display(Name = "ProvinceOfIssue")]
        //public int ProvinceOfIssueId { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[Display(Name = "Province")]
        //public int ProvinceId { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[Display(Name = "City")]
        //public int CityId { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[Display(Name = "CityOfBirth")]
        //public int CityOfBirthId { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[Display(Name = "CityOfIssue")]
        //public int CityOfIssueId { get; set; }

        //[Display(Name = "Province")]
        //public IEnumerable<SelectListItem> Provinces { get; set; }

        #region Bank Info

        //[Display(Name = "CustomerNumber")]
        //[StringLength(15)]
        //[RegularExpression(RegularExpression.CustomerNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //public string CustomerNumber { get; set; }


        ////[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[StringLength(20)]
        //[Display(Name = "شماره ثنا")]
        ////[RegularExpression(RegularExpression.AyandehBankCardNumberWithDash, ErrorMessage = "AyandehBankCardNumberWithDashMessage")]
        //public string SanaNumber { get; set; }

        //[Display(Name = "AccountNumber")]
        //[StringLength(20)]
        //[RegularExpression(RegularExpression.AccountNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //public string AccountNumber { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[StringLength(20)]
        //[Display(Name = "CardNumber")]
        ////[RegularExpression(RegularExpression.BankCardNumberWithDash,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        //[RegularExpression(RegularExpression.AyandehBankCardNumberWithDash, ErrorMessage = "AyandehBankCardNumberWithDashMessage")]
        //public string CardNumber { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[StringLength(26)]
        //[Display(Name = "IBAN")]
        //[RegularExpression(RegularExpression.IBAN,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        //public string IBAN { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[Display(Name = "DepositStatus")]
        //public string DepositStatus { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[Display(Name = "DepositOwners")]
        //public string DepositOwners { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[StringLength(200,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.StringLengthMessage) )]
        //[Display(Name = "BankName")]
        //public string BankName { get; set; }

        ////[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[Display(Name = "Bank")]
        //public int? BankId { get; set; }

        //[Display(Name = "Bank")]
        //public IEnumerable<SelectListItem> Banks { get; set; }


        #endregion

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[StringLength(100, ErrorMessage = "{0} باید حداقل  {2} کاراکتر و حداکثر {1} کاراکتر باشد.", MinimumLength = 6)]
        [StringLength(100, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.PasswordMinLength), MinimumLength = 6)]
        [Display(Name = "Password", ResourceType = typeof(ResourceFile))]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [StringLength(100)]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Compare(nameof(Password), ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.PasswordCompareError))]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(ResourceFile))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        //public string ReturnUrl { get; set; }

        //public bool IsEditMode { get; set; } = false;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!NationalCodeHelper.IsValidIranianNationalCode(NationalCode))
                yield return new ValidationResult("فرمت کد ملی صحیح نمی باشد!");
            yield return ValidationResult.Success!;
        }

        //public string ShamsiCreateDate
        //{
        //    get
        //    {
        //        return CreateDate.GregorianToShamsi(showTime: true);
        //    }
        //}
        //public DateTime? UpdateDate { get; set; }
        //public string ShamsiUpdateDate
        //{
        //    get
        //    {
        //        return UpdateDate.HasValue ? UpdateDate.Value.GregorianToShamsi(showTime: true) : string.Empty;
        //    }
        //}
    }

    [Serializable]
    public class UserRegisterByZarinpalResultModel
    {
        public User User { get; set; }
        public long? ZP_Id  { get; set; }
        public bool ExistUser { get; set; }
    }
}
