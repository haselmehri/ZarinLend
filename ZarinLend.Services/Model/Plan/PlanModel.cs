using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class PlanModel
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name ="نام")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "مبلغ چک/ضمانت")]
        public long AmountWaranty { get; set; }

        [Display(Name = "مبلغ تسهیلات")]
        public long? FacilityAmount { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Display(Name = "وضعیت")]
        public bool IsActive { get; set; }

        [Display(Name = "شرکت")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public short OrganizationId { get; set; }

        [Display(Name = "شرکت")]
        public IEnumerable<SelectListItem> Organizations { get; set; }

        [Display(Name = "نوع تسهیلات")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public short FacilityTypeId { get; set; }

        [Display(Name = "نوع تسهیلات")]
        public IEnumerable<SelectListItem> FacilityTypes { get; set; }

        [Display(Name="لیست اعضاء")]
        public List<PlanMemberModel> Members { get; set; }

        public bool IsEditMode { get; set; }
    }
}
