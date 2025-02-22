using Common.Utilities;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model.NeginHub
{
    public class GetCivilRegistryDataInputModel
    {
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(10)]
        [Display(Name = "NationalCode", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.NationalCode,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string NationalCode { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "BirthDate", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.PersianDate,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string ShamsiBirthDate { get; set; }
    }
}
