using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace WebFramework.Configuration
{
    public static class SessionManager
    {
        public enum SessionKeys
        {
            JwtToken,
            SelectedProject
        }

        private static readonly Dictionary<SessionKeys, string> Keys = new Dictionary<SessionKeys, string>
        {
            { SessionKeys.JwtToken,"JWT_TOKEN"},
            { SessionKeys.SelectedProject,"Selected_Project_Id"},
        };

        public static string Get(HttpContext context, SessionKeys key)
        {
            var sessionName = Keys[key];

            return context.Session?.GetString(sessionName);
        }
        public static void Set(HttpContext context, SessionKeys key, string value)
        {
            var sessionName = Keys[key];

            context.Session.SetString(sessionName, value);
        }

        public static void Remove(HttpContext context, SessionKeys key)
        {
            var sessionName = Keys[key];

            context.Session.Remove(sessionName);
        }
    }
}
