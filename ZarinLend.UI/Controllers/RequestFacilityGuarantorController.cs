using Common;
using Core.Entities;
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
    public class RequestFacilityGuarantorController : BaseMvcController
    {
        private readonly IRequestFacilityGuarantorService requestFacilityGuarantorService;
        private readonly IGlobalSettingService globalSettingService;
        private readonly ITransactionService transactionService;
        private readonly IIranCreditScoringService iranCreditScoringService;

        public RequestFacilityGuarantorController(IRequestFacilityGuarantorService requestFacilityGuarantorService,
                                                  IGlobalSettingService globalSettingService,
                                                  ITransactionService transactionService,
                                                  IIranCreditScoringService iranCreditScoringService)
        {
            this.requestFacilityGuarantorService = requestFacilityGuarantorService;
            this.globalSettingService = globalSettingService;
            this.transactionService = transactionService;
            this.iranCreditScoringService = iranCreditScoringService;
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult> RegisterGuarantor(CancellationToken cancellationToken)
        {
            var setting = await globalSettingService.GetActiveGlobalSetting(cancellationToken);
            IranCreditScoringModel iranCreditScoringResult;
            var userId = new Guid(User.Identity.GetUserId());
            if (//await transactionService.GetBalance(userId, cancellationToken) >= setting.ValidationFee ||
                 await iranCreditScoringService.ExistVerifyResult(userId, expireAfterFewDays: setting.ValidityPeriodOfValidation, cancellationToken))
            {
                iranCreditScoringResult = await iranCreditScoringService.GetVerifyResult(userId, cancellationToken);
                ViewBag.Title = "ثبت ضامن";
                ViewBag.UserRisk = iranCreditScoringResult.Risk;
                return View(model: await requestFacilityGuarantorService.PrepareModelForAdd(iranCreditScoringResult.Risk, cancellationToken));
            }
            else
                return RedirectToAction("InternetPayment", "Payment");
        }

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert)]
        [HttpGet("[controller]/[action]/{requestFacilityGuarantorId:int}")]
        public async Task<ActionResult> VerifyGuarantorByZarinLend(int requestFacilityGuarantorId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "بررسی ضامن توسط زرین لند";
            return View(await requestFacilityGuarantorService.GetRequestFacilityCompleteInfo(requestFacilityGuarantorId, WorkFlowFormEnum.VerifyGuarantorByZarinLend, cancellationToken));
        }

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert)]
        [HttpGet("[controller]/[action]/{requestFacilityGuarantorId:int}")]
        public async Task<ActionResult> VerifyGuaranteesByZarinLend(int requestFacilityGuarantorId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "بررسی چک توسط توسط زرین لند";
            return View("VerifyGuarantorByZarinLend", await requestFacilityGuarantorService.GetRequestFacilityCompleteInfo(requestFacilityGuarantorId, WorkFlowFormEnum.VerifyGuaranteesByZarinLend, cancellationToken));
        }

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert, RoleEnum.Buyer)]
        [HttpGet("[controller]/[action]/{requestFacilityGuarantorId:int}")]
        public async Task<ActionResult> Detail(int requestFacilityGuarantorId, CancellationToken cancellationToken)
        {
            ViewBag.Title = "جزئیات درخواست";
            if (User.IsInRole(RoleEnum.Admin.ToString()) || User.IsInRole(RoleEnum.SuperAdmin.ToString()) || User.IsInRole(RoleEnum.ZarinLendExpert.ToString()))
                return View("VerifyGuarantorByZarinLend", await requestFacilityGuarantorService.GetRequestFacilityCompleteInfo(requestFacilityGuarantorId, cancellationToken: cancellationToken));
            else
                return View("VerifyGuarantorByZarinLend", await requestFacilityGuarantorService
                    .GetRequestFacilityCompleteInfo(requestFacilityGuarantorId, guarantorUserId: new Guid(User.Identity.GetUserId()), cancellationToken: cancellationToken));
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        [HttpGet("[controller]/[action]")]
        public ActionResult List()
        {
            ViewBag.Title = "لیست درخواست ها";
            return View();
        }

        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.ZarinLendExpert)]
        [HttpGet("[controller]/[action]")]
        public ActionResult AllRequests()
        {
            ViewBag.Title = "لیست درخواست ها";
            return View();
        }
    }
}