using Microsoft.AspNetCore.Mvc;
using System;
using WebFramework.Configuration;
using WebFramework.Filters;

namespace WebFramework.Api
{
    [ApiController]
    [ApiResultFilter]
    [Route("api/v{version:apiVersion}/[controller]")] // => /api/v1/...
    //[AllowAnonymous] //to development test
    public class BaseApiController : ControllerBase
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
