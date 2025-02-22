using Common;
using Core.Entities;
using Core.Entities.Business.Payment;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services;
using Services.Model;
using Services.Model.IranCreditScoring;
using Services.Model.Payment;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.UI.Controllers
{
    public class IranCreditScoringController : BaseMvcController
    {
        private readonly IRequestFacilityService requestFacilityService;
        private readonly IRequestFacilityGuarantorService requestFacilityGuarantorService;
        private readonly IGlobalSettingService globalSettingService;
        private readonly ITransactionService transactionService;
        private readonly IUserService userService;

        public IranCreditScoringController(IRequestFacilityService requestFacilityService,
                                           IRequestFacilityGuarantorService requestFacilityGuarantorService,
                                           IGlobalSettingService globalSettingService,
                                           ITransactionService transactionService,
                                           IUserService userService)
        {
            this.requestFacilityService = requestFacilityService;
            this.requestFacilityGuarantorService = requestFacilityGuarantorService;
            this.globalSettingService = globalSettingService;
            this.transactionService = transactionService;
            this.userService = userService;
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult<IranCreditScoringInputModel>> Verify(CancellationToken cancellationToken = default)
        {
            var userId = new Guid(User.Identity.GetUserId());
            var setting = await globalSettingService.GetActiveGlobalSetting(cancellationToken);
            if (await transactionService.GetBalance(userId, cancellationToken) < setting.ValidityPeriodOfValidation)
                return RedirectToAction("InternetPayment", "Payment");

            var model = new IranCreditScoringInputModel()
            {
                IranCreditScoringRequestType = IranCreditScoringRequestType.BeforeFacilityRequest,
                NationalCode = User.Identity.GetUserName(),
                Mobile = await userService.GetMobile(userId, cancellationToken),
            };
            if (TempData["InternetBankResponse"] != null && TempData["IpgType"] != null)
            {
                if ((IpgType)TempData["IpgType"] == IpgType.ZarinpalIPG)
                    model.ZarinPalPaymentResponse = JsonConvert.DeserializeObject<InternetPaymentResponseModel>(TempData["InternetBankResponse"].ToString());
                else if ((IpgType)TempData["IpgType"] == IpgType.SamanIPG)
                    model.SamanPaymentResponse = JsonConvert.DeserializeObject<SamanInternetPaymentCallBackModel>(TempData["InternetBankResponse"].ToString());
            }
            return View(model: model);
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult<IranCreditScoringInputModel>> Verify_Old(int requestFacilityId, CancellationToken cancellationToken = default)
        {
            var userId = new Guid(User.Identity.GetUserId());

            var waitingRequestFacilityId =
                await requestFacilityService.GetRequestFacilityIdWaitingSpecifiedStepAndRole(requestFacilityId,
                                                                                             userId,
                                                                                             new List<RoleEnum> { RoleEnum.Buyer },
                                                                                             WorkFlowFormEnum.VerifyIranCreditScoring,
                                                                                             cancellationToken);

            if (waitingRequestFacilityId.HasValue)
            {
                var model = new IranCreditScoringInputModel()
                {
                    IranCreditScoringRequestType = IranCreditScoringRequestType.ForFacilityRequest,
                    RequestId = requestFacilityId,
                    NationalCode = User.Identity.GetUserName(),
                    Mobile = await userService.GetMobile(userId),
                };
                if (TempData["InternetBankResponse"] != null && TempData["IpgType"] != null)
                {
                    if ((IpgType)TempData["IpgType"] == IpgType.ZarinpalIPG)
                        model.ZarinPalPaymentResponse = JsonConvert.DeserializeObject<InternetPaymentResponseModel>(TempData["InternetBankResponse"].ToString());
                    else if ((IpgType)TempData["IpgType"] == IpgType.SamanIPG)
                        model.SamanPaymentResponse = JsonConvert.DeserializeObject<SamanInternetPaymentCallBackModel>(TempData["InternetBankResponse"].ToString());
                }
                return View(model: model);
            }

            return RedirectToAction("List", "RequestFacility");
        }


        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin)]
        public ActionResult<IranCreditScoringResultRuleAddEditModel> IranCreditScoringResultRules()
        {
            var model = new IranCreditScoringResultRuleAddEditModel();
            return View(model: model);
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        [HttpGet("[controller]/[action]/{requestFacilityGuarantorId:int}")]
        public async Task<ActionResult<IranCreditScoringInputModel>> VerifyByGuarantor(int requestFacilityGuarantorId, CancellationToken cancellationToken = default)
        {
            var userId = new Guid(User.Identity.GetUserId());

            var waitingRequestFacilityGuarantorId =
                await requestFacilityGuarantorService.GetRequestFacilityGuarantorIdWaitingSpecifiedStepAndRole(requestFacilityGuarantorId,
                                                                                                               userId,
                                                                                                               new List<RoleEnum> { RoleEnum.Buyer },
                                                                                                               WorkFlowFormEnum.VerifyGuarantorByZarinLend,
                                                                                                               cancellationToken);

            if (waitingRequestFacilityGuarantorId.HasValue)
            {
                var model = new IranCreditScoringInputModel()
                {
                    IranCreditScoringRequestType = IranCreditScoringRequestType.ForFacilityRequestGuarantor,
                    RequestId = requestFacilityGuarantorId,
                    NationalCode = User.Identity.GetUserName(),
                    Mobile = await userService.GetMobile(userId),
                };
                if (TempData["InternetBankResponse"] != null && TempData["IpgType"] != null)
                {
                    if ((IpgType)TempData["IpgType"] == IpgType.ZarinpalIPG)
                        model.ZarinPalPaymentResponse = JsonConvert.DeserializeObject<InternetPaymentResponseModel>(TempData["InternetBankResponse"].ToString());
                    else if ((IpgType)TempData["IpgType"] == IpgType.SamanIPG)
                        model.SamanPaymentResponse = JsonConvert.DeserializeObject<SamanInternetPaymentCallBackModel>(TempData["InternetBankResponse"].ToString());
                }
                return View("Verify", model: model);
            }

            return RedirectToAction("List", "RequestFacilityGuarantor");
        }
    }
}