using Common.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    [Serializable]
    public class UserSearchModel
    {
        //[Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[StringLength(100)]
        //[Display(Name = "UserName")]
        //public string UserName { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [StringLength(10)]
        [Display(Name = "NationalCode", ResourceType = typeof(ResourceFile))]
        [RegularExpression(RegularExpression.NationalCode,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string NationalCode { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} را جهت انتقال وارد کنید")]
        [Display(Name = "میزان اعتبار(ريال)")]
        [RegularExpression(RegularExpression.NumberThousandSeparator,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public string Amount { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} را جهت انتقال وارد کنید")]
        [Display(Name = "کد تایید")]
        [RegularExpression(RegularExpression.IntegerNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        [MaxLength(6)]
        [StringLength(6, MinimumLength = 6, ErrorMessage ="{0} باید عددی شش(6) رقمی باشد")]
        public string VerificationCode { get; set; }
    }
}
