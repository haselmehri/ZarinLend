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
    public class RequestFacilityPromissoryController(IRequestFacilityService requestFacilityService, IRequestFacilityPromissoryService requestFacilityPromissoryService) : BaseMvcController
    {
        [CustomAuthorize(RoleEnum.Buyer)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult> SignPromissoryByUser(int requestFacilityId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "صدور سفته";
            ViewBag.IsInWaitingSignPromissoryByUserStep = false;
            var userId = new Guid(User.Identity!.GetUserId());
            var isInSignPromissoryByUserStep = await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(userId,
                                                                                                                        requestFacilityId,
                                                                                                                        new List<RoleEnum> { RoleEnum.Buyer },
                                                                                                                        WorkFlowFormEnum.SignPromissoryByUser,
                                                                                                                        cancellationToken);
            if (isInSignPromissoryByUserStep)
                return View(model: await requestFacilityPromissoryService.GetRequestFacilityPromissory(userId, requestFacilityId, cancellationToken));
            ViewBag.IsInWaitingSignPromissoryByUserStep = await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.WaitingSignPromissoryByUser, cancellationToken);

            if (ViewBag.IsInWaitingSignPromissoryByUserStep)
                return View(model: await requestFacilityPromissoryService.GetRequestFacilityPromissory(userId, requestFacilityId, cancellationToken));
            
            return RedirectToAction(nameof(RequestFacilityController.List), nameof(RequestFacility));
        }
    }
}