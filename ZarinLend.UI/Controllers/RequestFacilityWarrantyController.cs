using Common;
using Core.Entities;
using Core.Entities.Business.RequestFacility;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.UI.Controllers
{
    public class RequestFacilityWarrantyController : BaseMvcController
    {
        private readonly IRequestFacilityService requestFacilityService;
        private readonly IRequestFacilityWarrantyService requestFacilityWarrantyService;

        public RequestFacilityWarrantyController(IRequestFacilityService requestFacilityService,
            IRequestFacilityWarrantyService requestFacilityWarrantyService)
        {
            this.requestFacilityService = requestFacilityService;
            this.requestFacilityWarrantyService = requestFacilityWarrantyService;
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> RegisterGuarantees(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "فرم بارگذاری و ثبت تضامین";
            var userId = new Guid(User.Identity.GetUserId());
            if (await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer },
                WorkFlowFormEnum.RegisterGuarantees, cancellationToken))
            {
                return View(model: await requestFacilityWarrantyService.GetRequestFacilityWaranty(userId, requestFacilityId, cancellationToken));
            }

            return RedirectToAction(nameof(RequestFacilityController.List), nameof(RequestFacility));
        }
    }
}