using System;
using System.ComponentModel.DataAnnotations;
using ZarinLend.Common.LocalizationResource;

namespace Services.Model
{
    public class UserVAPIDModel
    {
        public Guid UserId { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string Endpoint { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string P256dh { get; set; }

        [Required(AllowEmptyStrings = false,  ErrorMessageResourceType = typeof(ResourceFile), ErrorMessageResourceName = nameof(ResourceFile.RequiredValidationMessage))]
        public string Auth { get; set; }
        public string Platform { get; set; }
        public string AppVersion { get; set; }
        public string AppCodeName { get; set; }
        public string AppName { get; set; }
        public string UserAgent { get; set; }
        public bool? IsMobile { get; set; }
        public string OsName { get; set; }

    }
}
