using Common.CustomAttribute;
using Common.CustomFileAttribute;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model.Invoice
{
    public class InvoiceImageUploadModel
    {
        public int Id { get; set; }

        /// <summary>
        /// OrganizationId
        /// </summary>
        public int ShopOrganizationId { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        [Display(Name = "شماره فاکتور")]
        public string Number { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "تصویر فاکتور را جهت بارگذاری انتخاب کنید")]
        [MaxFileSize(1 * 1024 * 1024/*Max File Size : 1MB*/), AllowedExtensions(new string[] { ".jpg", ".jpeg", ".gif", ".png" }, errorMessage: "فقط فایل هایی از نوع تصویر/عکس با پسوند (jpg,jpeg,gif,png) می توانید بارگذاری کنید!")]
        public IFormFile InvoiceFile { get; set; }

    }
}
