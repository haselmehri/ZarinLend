using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace WebFramework.Middlewares
{
    public static class NotFound404MiddlewareExtesions
    {
        public static IApplicationBuilder UseNotFound404Unauthorized401Middleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NotFound404Unauthorized401Middleware>();
        }
    }
    public class NotFound404Unauthorized401Middleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<NotFound404Unauthorized401Middleware> logger;

        public NotFound404Unauthorized401Middleware(RequestDelegate next,
            ILogger<NotFound404Unauthorized401Middleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            await next(context);

            if ((context.Response.StatusCode == 404 || context.Response.StatusCode == 401 || context.Response.StatusCode == 500) && !context.Response.HasStarted)
            {
                context.Response.StatusCode = context.Response.StatusCode;
                string originalPath = context.Request.Path.Value;
                if (context.Response.StatusCode == 404)
                {
                    logger.LogWarning($"404:Page Not Found, Path {originalPath}");                
                    context.Items["OriginalPath"] = originalPath;
                    context.Request.Path = "/404";
                    await next(context);
                }
                else if (context.Response.StatusCode == 401)
                {
                    logger.LogWarning($"401:Unauthorized, Path {originalPath}");                    
                    context.Request.Path = "/401";
                    await next(context);
                }
                else if (context.Response.StatusCode == 500)
                {
                    logger.LogWarning($"500:InternalServerError, Path {originalPath}");
                    context.Request.Path = "/500";
                    await next(context);
                }
            }
        }
    }
}
