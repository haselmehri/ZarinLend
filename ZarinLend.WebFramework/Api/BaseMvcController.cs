using Microsoft.AspNetCore.Mvc;
using System;
using WebFramework.Configuration;
namespace WebFramework.Api
{
    public class BaseMvcController : Controller
    {
        public bool IsAuthenticated => User.Identity.IsAuthenticated && CookieManager.Get(HttpContext, CookieManager.CookieKeys.JwtToken) != null;
        public string GetBaseUrl => $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

        public string? BaseUrl()
        {
            if (Request == null) return null;
            var uriBuilder = new UriBuilder(Request.Scheme, Request.Host.Host, Request.Host.Port ?? -1);
            if (uriBuilder.Uri.IsDefaultPort)
            {
                uriBuilder.Port = -1;
            }

            return uriBuilder.Uri.AbsoluteUri;
        }
    }
}
