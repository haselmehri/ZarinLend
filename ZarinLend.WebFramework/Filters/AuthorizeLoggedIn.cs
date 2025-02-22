using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace WebFramework.Filters
{
    public class AuthorizeLoggedIn : IAuthorizationRequirement
    {
        public AuthorizeLoggedIn()
        {

        }
    }

    public class LoggedIn : AuthorizationHandler<AuthorizeLoggedIn>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public LoggedIn(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       AuthorizeLoggedIn requirement)
        {
            if (context.Resource is AuthorizationFilterContext redirectContext)
                redirectContext.Result = new RedirectResult("/login");
            else
            //if (SessionManager.Get(_httpContextAccessor.HttpContext, SessionManager.SessionKeys.PersonId) != null)
            //{
                context.Succeed(requirement);
            //}
            return Task.CompletedTask;
        }
    }
}
