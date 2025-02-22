using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace WebFramework.Extensions
{
    [HtmlTargetElement("script", Attributes = "on-content-loaded")]
    public class ScriptTagHelper : TagHelper
    {
        /// <summary>
        /// execute script once document is loaded.
        /// </summary>
        public bool OnContentLoaded { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!OnContentLoaded)
            {
                base.Process(context, output);
            }
            else
            {
                var content = output.GetChildContentAsync().Result;
                var javascript = content.GetContent();

                var sb = new StringBuilder();
                sb.Append("document.addEventListener('DOMContentLoaded',");
                sb.Append("function(){");
                sb.Append("let date= new Date();");
                sb.Append("console.log('ScriptTagHelper : ' + date.getHours()+ ':' + date.getMinutes() + ':' + date.getSeconds() + '.' + date.getMilliseconds());");
                sb.Append(javascript);
                sb.Append("});");

                output.Content.SetHtmlContent(sb.ToString());
            }
        }
    }
}
