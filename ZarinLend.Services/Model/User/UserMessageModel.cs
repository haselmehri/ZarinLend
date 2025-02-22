using Common.CustomAttribute;
using Common.CustomFileAttribute;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Services.Model
{
    [Serializable]
    public class UserMessageModel
    {
        public bool SendToInbox { get; set; }
        public bool SendSms { get; set; }
        public bool SendNotification { get; set; }
        public Guid SenderId { get; set; }
        public List<Guid> UserIds { get; set; }
        public string InboxMessageContent { get; set; }
        public string SmsContent { get; set; }
        public UserNotificationModel UserNotificationModel { get; set; }
    }

    public class UserNotificationModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string IconUrl { get; set; }

        [MaxFileSize(200 * 1024/*Max File Size : 100kb*/),AllowedExtensions([".png", ".jpg" ,".jpeg",".gif", ".bmp"], errorMessage: "فقط فایل هایی از نوع عکس/تصویر با پسوند (png,jpg,jpeg,gif,bmp) می توانید بارگذاری کنید!")]
        public IFormFile ImageFile { get; set; }
        public string Url { get; set; }
    }
}
