using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Model.GlobalSetting;
using System;
using System.Globalization;
using System.Threading;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.UI.Controllers
{
    public class HomeController : BaseMvcController
    {
        private readonly IEmailSender emailSender;
        private readonly IGlobalSettingService globalSettingService;

        public HomeController(IEmailSender emailSender, IGlobalSettingService globalSettingService)
        {
            this.emailSender = emailSender;
            this.globalSettingService = globalSettingService;
        }

        [CustomAuthorize()]
        public IActionResult Index()
        {
            ViewBag.Title = "داشبورد";
            if (User.IsInRole(RoleEnum.Admin.ToString()) ||
                User.IsInRole(RoleEnum.SuperAdmin.ToString()) ||
                User.IsInRole(RoleEnum.ZarinLendExpert.ToString()))
                return View(viewName: "~/Views/Home/IndexAdmin.cshtml");
            else if (User.IsInRole(RoleEnum.AdminBankLeasing.ToString()) ||
                     User.IsInRole(RoleEnum.SupervisorLeasing.ToString()) ||
                     User.IsInRole(RoleEnum.BankLeasing.ToString()))
                return View(viewName: "~/Views/Home/IndexLeasing.cshtml");
            else if (User.IsInRole(RoleEnum.Buyer.ToString()))
                return View(viewName: "~/Views/Home/Index.cshtml");
            else if (User.IsInRole(RoleEnum.Seller.ToString()))
                return View(viewName: "~/Views/Home/IndexSeller.cshtml");

            return View("~/Views/Home/Index.cshtml");
        }

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin)]
        public ActionResult<GlobalSettingModel> GlobalSetting()
        {          
            return View(model: new GlobalSettingModel());
        }

        [AllowAnonymous]
        public IActionResult SetCultureCookie(string cltr, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cltr)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            //            return View();
            return LocalRedirect(returnUrl);
        }

        [Route("404")]
        [AllowAnonymous]
        public IActionResult PageNotFound()
        {
            string originalPath = "unknown";
            if (HttpContext.Items.ContainsKey("OriginalPath"))
            {
                originalPath = HttpContext.Items["OriginalPath"] as string;
            }
            return View("404");
        }

        [Route("401")]
        [AllowAnonymous]
        public new IActionResult Unauthorized()
        {
            string originalPath = "unknown";
            if (HttpContext.Items.ContainsKey("OriginalPath"))
            {
                originalPath = HttpContext.Items["OriginalPath"] as string;
            }
            return View("401");
        }

        [Route("500")]
        [AllowAnonymous]
        public IActionResult InternalServerError(string errorMessage)
        {
            //if (HttpContext.Items.ContainsKey("OriginalPath"))
            //    ViewBag.OrginalPath = HttpContext.Items["OriginalPath"] as string;

            if (HttpContext.Items.ContainsKey("ErrorMessage"))
                ViewBag.ErrorMessage = HttpContext.Items["ErrorMessage"] as string;

            ViewBag.ErrorMessage += errorMessage;

            return View("500");
        }
    }
}