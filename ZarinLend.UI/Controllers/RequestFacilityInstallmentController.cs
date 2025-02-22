using Common;
using Common.Utilities;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.UI.Controllers
{
    public class RequestFacilityInstallmentController : BaseMvcController
    {
        private readonly IRequestFacilityService requestFacilityService;
        private readonly IZarinPalInternetPaymentService zarinPalInternetPaymentService;

        public RequestFacilityInstallmentController(IRequestFacilityService requestFacilityService, IZarinPalInternetPaymentService zarinPalInternetPaymentService)
        {
            this.requestFacilityService = requestFacilityService;
            this.zarinPalInternetPaymentService = zarinPalInternetPaymentService;
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        public async Task<ActionResult> InstallmentList(CancellationToken cancellationToken)
        {
            ViewBag.Title = "اقساط من";
            await zarinPalInternetPaymentService.ValidationStatuslessPayments(new Guid(User.Identity.GetUserId()), cancellationToken);
            return View(model:await requestFacilityService.SelectApprovalFacility(new Guid(User.Identity.GetUserId()), cancellationToken));
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        public async Task<ActionResult> InstallmentList2(CancellationToken cancellationToken)
        {
            ViewBag.Title = "اقساط من";
            await zarinPalInternetPaymentService.ValidationStatuslessPayments(new Guid(User.Identity.GetUserId()), cancellationToken);
            return View(model: await requestFacilityService.SelectApprovalFacility(new Guid(User.Identity.GetUserId()), cancellationToken));
        }

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert)]
        public  ActionResult Search(CancellationToken cancellationToken)
        {
            ViewBag.Title = "جستجوی پیشرفته";
            //ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            //ViewBag.PersianStartDate = DateTime.Now.AddMonths(-1).GregorianToShamsi();

            //ViewBag.EndDate = DateTime.Now.ToString("yyyy/MM/dd");
            //ViewBag.PersianEndDate = DateTime.Now.GregorianToShamsi();

            RequestFacilityInstallmentFilterModel model = new RequestFacilityInstallmentFilterModel();

            return View(model);
        }
    }
}