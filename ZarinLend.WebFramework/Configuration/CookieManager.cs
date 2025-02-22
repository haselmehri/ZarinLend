using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace WebFramework.Configuration
{
    public static class CookieManager
    {

        public enum CookieKeys
        {
            JwtToken,
            GlobalExpireTime,
            AspNetCoreCulture,
            CodeVerifier,
            StatementOrderId,
            QuickRegisterData,
            AuthZL
        }

        public enum ExpireTimeMode
        {
            Minute,
            Hour,
            Day
        }

        private static readonly Dictionary<CookieKeys, string> Keys = new Dictionary<CookieKeys, string>()
        {
            { CookieKeys.JwtToken,"JWT_TOKEN"},
            { CookieKeys.GlobalExpireTime,"GlobalExpireTime"},
            { CookieKeys.AspNetCoreCulture,".AspNetCore.Culture" },
            { CookieKeys.CodeVerifier,"CodeVerifier" },
            { CookieKeys.StatementOrderId,"StatementOrderId" },
            { CookieKeys.QuickRegisterData,"QuickRegisterData" },
            { CookieKeys.AuthZL,"AuthZL" }
        };

        public static string Get(HttpContext context, CookieKeys key)
        {
            var cookieName = Keys[key];

            return context.Request.Cookies[cookieName]!;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTimeMode"></param>
        /// <param name="expireTime">expireTime of minutes</param>
        public static void Set(HttpContext context, CookieKeys key, string value, SameSiteMode sameSite, bool httpOnly, bool isEssential, bool secure, ExpireTimeMode? expireTimeMode = null, int? expireTime = null)
        {
            var cookieName = Keys[key];

            var option = new CookieOptions
            {
                HttpOnly = httpOnly,
                IsEssential = isEssential,
                SameSite = sameSite,
                Secure = secure,
                //HttpOnly = true,
                //IsEssential = true,
                //SameSite = SameSiteMode.Strict,
                //Secure = false,//true,
            };

            if (expireTime.HasValue && expireTimeMode.HasValue)
            {
                option.Expires = expireTimeMode switch
                {
                    ExpireTimeMode.Day => DateTime.Now.AddDays(expireTime.Value),
                    ExpireTimeMode.Hour => DateTime.Now.AddHours(expireTime.Value),
                    ExpireTimeMode.Minute => DateTime.Now.AddMinutes(expireTime.Value),
                    _ => option.Expires
                };
            }

            if (key == CookieKeys.JwtToken)
            {
                context.Response.Cookies.Append(Keys[CookieKeys.GlobalExpireTime], "true", new CookieOptions
                {
                    HttpOnly = httpOnly,
                    IsEssential = isEssential,
                    SameSite = SameSiteMode.Lax,
                    Secure = secure,
                });
            }

            context.Response.Cookies.Append(cookieName, value, option);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTimeMode"></param>
        /// <param name="expireTime">expireTime of minutes</param>
        public static void Set(HttpContext context, CookieKeys key, string value, SameSiteMode sameSite, bool httpOnly, bool isEssential, bool secure, DateTime expireTime)
        {
            var cookieName = Keys[key];

            var option = new CookieOptions
            {
                HttpOnly = httpOnly,
                IsEssential = isEssential,
                SameSite = sameSite,
                Secure = secure,
                Expires = expireTime,
            };

            if (key == CookieKeys.JwtToken)
            {
                context.Response.Cookies.Append(Keys[CookieKeys.GlobalExpireTime], "true", new CookieOptions
                {
                    HttpOnly = httpOnly,
                    IsEssential = isEssential,
                    SameSite = SameSiteMode.Lax,
                    Secure = secure,
                    Expires = expireTime
                });
            }

            context.Response.Cookies.Append(cookieName, value, option);
        }

        public static void Remove(HttpContext context, CookieKeys key)
        {
            var cookieName = Keys[key];

            context.Response.Cookies.Delete(cookieName);
            if (key == CookieKeys.JwtToken)
                context.Response.Cookies.Delete(Keys[CookieKeys.GlobalExpireTime]);
        }

        public static void RemoveAllCookie(HttpContext context)
        {
            foreach (var key in Keys.Keys)
            {
                if (key == CookieKeys.AspNetCoreCulture) continue;
                Remove(context, key);
            }
        }
    }
}
