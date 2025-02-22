using Common.Utilities;
using Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class PersonCompleteInfoModel
    {
        public Guid Id { get; set; }
        public int RequestFacilityId { get; set; }

        [Display(Name = "UserName", ResourceType = typeof(ResourceFile))]
        public string UserName { get; set; } 

        [Display(Name = "FName", ResourceType = typeof(ResourceFile))]
        public string FName { get; set; }

        [Display(Name = "LName", ResourceType = typeof(ResourceFile))]
        public string LName { get; set; }

        [Display(Name = "FatherName", ResourceType = typeof(ResourceFile))]
        public string FatherName { get; set; }

        [Display(Name = "SSID", ResourceType = typeof(ResourceFile))]
        public string SSID { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "Gender", ResourceType = typeof(ResourceFile))]
        public GenderEnum? Gender { get; set; }

        [Display(Name = "Address", ResourceType = typeof(ResourceFile))]
        public string? Address { get; set; }

        [Display(Name = "Mobile", ResourceType = typeof(ResourceFile))]
        public string Mobile { get; set; }

        [Display(Name = "PhoneNumber", ResourceType = typeof(ResourceFile))]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Email", ResourceType = typeof(ResourceFile))]
        public string? Email { get; set; }

        //[Display(Name = "PhoneNumber", ResourceType = typeof(ResourceFile))]
        //public string PhoneNumber { get; set; }

        [Display(Name = "BirthDate", ResourceType = typeof(ResourceFile))]
        public DateTime BirthDate { get; set; }

        [Display(Name = "BirthDate", ResourceType = typeof(ResourceFile))]
        public string ShamsiBirthDate { get
            {
                return BirthDate.GregorianToShamsi();
            }
        }

        public string FacilityNumber { get; set; }

        [Display(Name = "NationalCode", ResourceType = typeof(ResourceFile))]
        public string NationalCode { get; set; }

        [Display(Name = "PostalCode", ResourceType = typeof(ResourceFile))]
        public string? PostalCode { get; set; }

        [Display(Name = "CustomerNumber", ResourceType = typeof(ResourceFile))]
        public string? CustomerNumber { get; set; }

        //[Display(Name = "AccountNumber", ResourceType = typeof(ResourceFile))]
        //public string AccountNumber { get; set; }

        //[Display(Name = "ProvinceOfBirth", ResourceType = typeof(ResourceFile))]
        //public string ProvinceOfBirth { get; set; }

        [Display(Name = "Province", ResourceType = typeof(ResourceFile))]
        public string AddressProvince { get; set; }

        //[Display(Name = "CityOfBirth", ResourceType = typeof(ResourceFile))]
        //public string CityOfBirth { get; set; }

        [Display(Name = "City", ResourceType = typeof(ResourceFile))]
        public string AddressCity { get; set; }

        [Display(Name = "Nationality", ResourceType = typeof(ResourceFile))]
        public string Nationality { get; set; }

        [Display(Name = "UserType", ResourceType = typeof(ResourceFile))]
        public string UserType { get; set; }
    }
}
