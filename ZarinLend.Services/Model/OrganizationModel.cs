using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    public class OrganizationModel
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "شناسه ملی/شماره ثبت")]
        public long NationalId { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name ="نام")]
        public string Name { get; set; }
        /// <summary>
        /// تلفن های فروشگاه/لیزینگ
        /// </summary>

        [Display(Name = "تلفن")]
        public string Tel { get; set; }

        [Display(Name = "آدرس")]
        public string Address { get; set; }
        /// <summary>
        /// آدرس سایت فروشگاه
        /// </summary>
        [Display(Name = "آدرس سایت")]
        public string SiteUrl { get; set; }

        [Display(Name = "وضعیت")]
        public bool IsActive { get; set; }

        [Display(Name = "نوع سازمان")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public short OrganizationTypeId { get; set; }
        public string OrganizationTypeName { get; set; }

        [Display(Name = "نوع سازمان")]
        public IEnumerable<SelectListItem> OrganizationTypes { get; set; }

        public bool IsEditMode { get; set; }
    }
}
