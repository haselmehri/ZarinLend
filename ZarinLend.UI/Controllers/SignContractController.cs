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
    public class SignContractController : BaseMvcController
    {
        private readonly IRequestFacilityService requestFacilityService;
        private readonly IRequestFacilityWarrantyService requestFacilityWarrantyService;

        public SignContractController(IRequestFacilityService requestFacilityService,
            IRequestFacilityWarrantyService requestFacilityWarrantyService)
        {
            this.requestFacilityService = requestFacilityService;
            this.requestFacilityWarrantyService = requestFacilityWarrantyService;
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> SignContractByUser(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "امضاء قرارداد تسهیلات";
            var userId = new Guid(User.Identity!.GetUserId());
            if (await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer },
                WorkFlowFormEnum.SignContractByUser, cancellationToken))
            {
                ViewBag.SendSignContractNotification = false;
                return View(model: requestFacilityId);
            }

            return RedirectToAction(nameof(RequestFacilityController.List), nameof(RequestFacility));
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> SendSignContractNotification(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "امضاء قرارداد تسهیلات";
            var userId = new Guid(User.Identity!.GetUserId());
            if (await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer },
                WorkFlowFormEnum.WaitingToSignContractByUser, cancellationToken))
            {
                ViewBag.SendSignContractNotification = true;
                return View(nameof(SignContractByUser), model: requestFacilityId);
            }

            return RedirectToAction(nameof(RequestFacilityController.List), nameof(RequestFacility));
        }
    }
}