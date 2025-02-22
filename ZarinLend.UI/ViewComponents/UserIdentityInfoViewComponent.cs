using Common;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class UserIdentityInfoViewComponent : ViewComponent
    {
        private readonly IUserService userService;

        public UserIdentityInfoViewComponent(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(UserIdentityInfoModel userIdentityInfo,bool showAccountInfo = true, CancellationToken cancellationToken=default)
        {
            ViewBag.ShowAccountInfo = showAccountInfo;
            return await Task.FromResult((IViewComponentResult)View("UserIdentityInfo", userIdentityInfo));
        }
    }
}
