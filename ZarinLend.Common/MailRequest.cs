using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace Common
{
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachments { get; set; }
        public List<string> FilePathAttachments { get; set; }
        public List<MemoryStream> StreamAttachments { get; set; }
    }

    public class PostalServerMailRequest
    {
        [JsonProperty("to")]
        public List<string> To { get; set; }
        [JsonProperty("from")]
        public string From { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("reply_to")]
        public string ReplyTo { get; set; }
        [JsonProperty("plain_body")]
        public string PlainBody { get; set; }
        [JsonProperty("html_body")]
        public string HtmlBody { get; set; }
    }
}
