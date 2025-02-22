using Common;
using Core.Entities;
using Core.Entities.Business.RequestFacility;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Model;
using Services.Model.IranCreditScoring;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.UI.Controllers
{
    public class RequestFacilityGuarantorWarrantyController : BaseMvcController
    {
        private readonly IRequestFacilityGuarantorService requestFacilityGuarantorService;
        private readonly IRequestFacilityGuarantorWarrantyService requestFacilityGuarantorWarrantyService;
        private readonly IGlobalSettingService globalSettingService;
        private readonly ITransactionService transactionService;
        private readonly IIranCreditScoringService iranCreditScoringService;

        public RequestFacilityGuarantorWarrantyController(IRequestFacilityGuarantorService requestFacilityGuarantorService,
                                                          IRequestFacilityGuarantorWarrantyService requestFacilityGuarantorWarrantyService)
        {
            this.requestFacilityGuarantorService = requestFacilityGuarantorService;
            this.requestFacilityGuarantorWarrantyService = requestFacilityGuarantorWarrantyService;
        }


        [CustomAuthorize(RoleEnum.Buyer)]
        [HttpGet("[controller]/[action]/{requestFacilityGuarantorId:int}")]
        public async Task<ActionResult> RegisterGuaranteesByGuarantor(int requestFacilityGuarantorId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "ثبت اطلاعات و بارگذاری تصویر چک";
            var userId = new Guid(User.Identity.GetUserId());
            if (await requestFacilityGuarantorService.CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityGuarantorId, new List<RoleEnum> { RoleEnum.Buyer },
                WorkFlowFormEnum.RegisterGuaranteesByGuarantor, cancellationToken))
            {
                return View("RegisterGuarantees", model: await requestFacilityGuarantorWarrantyService.GetRequestFacilityGuarantorWarranty(userId, requestFacilityGuarantorId, cancellationToken));
            }

            return RedirectToAction(nameof(RequestFacilityGuarantorController.List), nameof(RequestFacilityGuarantor));
        }
    }
}