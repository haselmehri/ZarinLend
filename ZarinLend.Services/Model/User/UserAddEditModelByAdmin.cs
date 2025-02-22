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
    public class UserAddEditModelByAdmin : IValidatableObject
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(100)]
        [Display(Name = "UserName", ResourceType = typeof(ResourceFile))]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.PersianCharacter,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(200)]
        [Display(Name = "FName", ResourceType = typeof(ResourceFile))]
        public string FName { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.PersianCharacter,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(200)]
        [Display(Name = "LName", ResourceType = typeof(ResourceFile))]
        public string LName { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [RegularExpression(RegularExpression.PersianCharacter,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(200)]
        [Display(Name = "FatherName", ResourceType = typeof(ResourceFile))]
        public string FatherName { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(10)]
        [Display(Name = "SSID", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.SSID,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string SSID { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "Gender", ResourceType = typeof(ResourceFile))]
        public GenderEnum Gender { get; set; }

        [StringLength(500)]
        [Display(Name = "Address", ResourceType = typeof(ResourceFile))]
        public string? Address { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(15)]
        [Display(Name = "Mobile", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.Mobile,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string Mobile { get; set; }

        [StringLength(50)]
        [Display(Name = "PhoneNumber", ResourceType = typeof(ResourceFile))]
        public string? PhoneNumber { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(10)]
        [Display(Name = "NationalCode", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.NationalCode,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string NationalCode { get; set; }

        [Display(Name = "Email", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.Email,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string? Email { get; set; }

        [Display(Name = "PostalCode", ResourceType = typeof(ResourceFile))]
        [StringLength(10)]
        [RegularExpression(RegularExpression.PostalCode,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string? PostalCode { get; set; }

        [Display(Name = "BirthDate", ResourceType = typeof(ResourceFile))]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public DateTime BirthDate { get; set; }

        [Display(Name = "BirthDate", ResourceType = typeof(ResourceFile))]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string ShamsiBirthDate { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "Province", ResourceType = typeof(ResourceFile))]
        public int? ProvinceId { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "City", ResourceType = typeof(ResourceFile))]
        public int? CityId { get; set; }

        #region List

        [Display(Name = "Province", ResourceType = typeof(ResourceFile))]
        public IEnumerable<SelectListItem>? Provinces { get; set; }

        [Display(Name = "City", ResourceType = typeof(ResourceFile))]
        public IEnumerable<SelectListItem>? Cities { get; set; }

        #endregion

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "نوع سازمان")]
        public int? OrganizationTypeId { get; set; }

        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "سازمان")]
        public int? OrganizationId { get; set; }

        [Display(Name = "سازمان")]
        public IEnumerable<SelectListItem>? Organizations { get; set; }

        [Display(Name = "نوع سازمان")]
        public IEnumerable<SelectListItem>? OrganizationTypes { get; set; }

        [Display(Name = "نقش کاربر")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public Guid RoleId { get; set; }

        [Display(Name = "نقش کاربر")]
        public IEnumerable<SelectListItem>? Roles { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[StringLength(100, ErrorMessage = "{0} باید حداقل  {2} کاراکتر و حداکثر {1} کاراکتر باشد.", MinimumLength = 6)]
        [StringLength(100, ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.PasswordMinLength), MinimumLength = 6)]
        [Display(Name = "Password", ResourceType = typeof(ResourceFile))]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [StringLength(100)]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Compare(nameof(Password),ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.PasswordCompareError))]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(ResourceFile))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name ="وضعیت")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public bool IsActive { get; set; } = true;

        public string? ReturnUrl { get; set; }

        public bool IsEditMode { get; set; } = false;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!NationalCodeHelper.IsValidIranianNationalCode(NationalCode))
                yield return new ValidationResult("فرمت کد ملی صحیح نمی باشد!");
            yield return ValidationResult.Success!;
        }
    }
}
