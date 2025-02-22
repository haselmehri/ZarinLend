using Common.CustomAttribute;
using Common.CustomFileAttribute;
using Common.Utilities;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    public class RequestFacilityWarrantyAddModel
    {
        public int Id { get; set; }
        public int RequestFacilityId { get; set; }
        public Guid UserId { get; set; }

        [Display(Name = "تصویر چک")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [MaxFileSize(1 * 1024 * 1024/*Max File Size : 1MB*/), AllowedExtensions([".jpg", ".jpeg", ".gif", ".png"], errorMessage: "فقط فایل هایی از نوع تصویر/عکس با پسوند (jpg,jpeg,gif,png) می توانید بارگذاری کنید!")]
        public required IFormFile ChequeFile { get; set; }

        [Display(Name = "فایل سفته")]
        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [MaxFileSize(1 * 1024 * 1024/*Max File Size : 1MB*/), AllowedExtensions([".pdf"], errorMessage: "فقط فایل هایی با پسوند pdf می توانید بارگذاری کنید!")]
        public required IFormFile PromissoryNoteFile { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        //[StringLength(11, ErrorMessage = "{0} باید {1} رقمی باشد")]
        [MaxLength(11, ErrorMessage = "{0} باید {1} رقمی باشد")]
        [Display(Name = "شماره سفته")]
        [RegularExpression(RegularExpression.SaftehNumber,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RegularExpression))]
        public required string DocumentNumber { get; set; }
    }
}
