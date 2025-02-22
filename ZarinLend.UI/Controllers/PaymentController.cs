using Common;
using Common.Exceptions;
using Common.Utilities;
using Core.Entities;
using Core.Entities.Business.Payment;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services;
using Services.Model;
using Services.Model.GlobalSetting;
using Services.Model.Payment;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Web.ApiControllers.Controllers.v1;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.UI.Controllers
{
    public class PaymentController : BaseMvcController
    {
        private readonly IRequestFacilityService requestFacilityService;
        private readonly ITransactionService transactionService;
        private readonly IZarinPalInternetPaymentService zarinPalInternetPaymentService;
        private readonly IRequestFacilityInstallmentService requestFacilityInstallmentService;
        private readonly ISamanInternetPaymentService samanInternetPaymentService;
        private readonly IRequestFacilityGuarantorService requestFacilityGuarantorService;
        private readonly IIranCreditScoringService iranCreditScoringService;
        private readonly IGlobalSettingService globalSettingService;
        private readonly IPaymentInfoService paymentInfoService;

        private GlobalSettingViewModel GlobalSettingModel { get; set; }

        public PaymentController(IRequestFacilityService requestFacilityService,
                                 ITransactionService transactionService,
                                 IZarinPalInternetPaymentService zarinPalInternetPaymentService,
                                 IRequestFacilityInstallmentService requestFacilityInstallmentService,
                                 ISamanInternetPaymentService samanInternetPaymentService,
                                 IRequestFacilityGuarantorService requestFacilityGuarantorService,
                                 IIranCreditScoringService iranCreditScoringService,
                                 IGlobalSettingService globalSettingService,
                                 IPaymentInfoService paymentInfoService)
        {
            this.requestFacilityService = requestFacilityService;
            this.transactionService = transactionService;
            this.zarinPalInternetPaymentService = zarinPalInternetPaymentService;
            this.requestFacilityInstallmentService = requestFacilityInstallmentService;
            this.samanInternetPaymentService = samanInternetPaymentService;
            this.requestFacilityGuarantorService = requestFacilityGuarantorService;
            this.iranCreditScoringService = iranCreditScoringService;
            this.globalSettingService = globalSettingService;
            this.paymentInfoService = paymentInfoService;
            GlobalSettingModel = globalSettingService.GetActiveGlobalSetting(default).GetAwaiter().GetResult();
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        public ActionResult PaymentList()
        {
            ViewBag.Title = "لیست پرداخت های اینترنی";
            return View();
        }

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert)]
        public ActionResult Search()
        {
            ViewBag.Title = "جستوی پرداخت های اینترنی";
            ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            ViewBag.PersianStartDate = DateTime.Now.AddMonths(-1).GregorianToShamsi();

            ViewBag.EndDate = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.PersianEndDate = DateTime.Now.GregorianToShamsi();

            return View("SearchPayment", new SamanIpgFilterModel());
        }

        #region Internet Payment for Sales Commission

        [CustomAuthorize(RoleEnum.Buyer)]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}")]
        public async Task<ActionResult<InternetPaymentModel>> PaySalesCommission(int requestFacilityId, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity!.GetUserId());
            await zarinPalInternetPaymentService.ValidationStatuslessPayments(userId, cancellationToken);
            var waitingRequestFacility = await requestFacilityService
               .GetRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, userId, new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.PaySalesCommission, cancellationToken);

            var model = new InternetPaymentModel();
            model.PaymentType = PaymentType.PaySalesCommission;
            if (waitingRequestFacility != null)
            {
                model.Error = TempData["Error"]?.ToString()!;
                model.RequestFacilityId = requestFacilityId;
                model.Amount = waitingRequestFacility.PrePaymentOrFee;
                model.Description = "پرداخت کمیسیون فروش-پیش پرداخت";
                model.PostAction = Url.Action(nameof(PostPaySalesCommission), "Payment")!;
                return View("InternetPayment", model);
            }
            else
            {
                model.Error = "در حال حاضر هیچ درخواست تسهیلاتی در مرحله 'پرداخت کارمزد کمیسیون فروش' وجود ندارد!";
                return View("InternetPayment", model);
            }
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        [HttpPost("[controller]/[action]/{requestFacilityId:int}/{ipgType}")]
        public async Task<ActionResult> PostPaySalesCommission(int requestFacilityId, IpgType ipgType, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity!.GetUserId());
            await zarinPalInternetPaymentService.ValidationStatuslessPayments(userId, cancellationToken);
            var waitingRequestFacility = await requestFacilityService
              .GetRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, userId, new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.PaySalesCommission, cancellationToken);

            if (waitingRequestFacility != null)
            {
                if (ipgType == IpgType.ZarinpalIPG)
                {
                    var callBackUrl = $"{GetBaseUrl}/Payment/{nameof(PostPaySalesCommissionCallBack)}";
                    var authority = await zarinPalInternetPaymentService.InitilizeInternetPayment(
                        userId,
                        waitingRequestFacility.Id,
                        amount: waitingRequestFacility.PrePaymentOrFee,//Convert.ToInt64(Math.Round(InstallmentCalculator.CalculatePrePaymentOrFee(waitingRequestFacility.Amount, waitingRequestFacility.MonthCount))),
                        PaymentType.PaySalesCommission,
                        callBackUrl,
                        User.Identity!.GetMobile(),
                        "پرداخت کمیسیون فروش-پیش پرداخت",
                        cancellationToken);
                    if (!string.IsNullOrEmpty(authority))
                    {
                        string gatewayUrl = ZarinPalInternetPaymentService.GatewayUrl + authority;
                        return Redirect(gatewayUrl);
                    }
                    else
                    {
                        TempData["Error"] = "خطا در براقراری ارتباط با درگاه بانک!";
                        return RedirectToAction(actionName: nameof(PaySalesCommission), new { requestFacilityId });
                    }
                }
                else
                {
                    var callBackUrl = $"{GetBaseUrl}/Payment/{nameof(SamanPostPaySalesCommissionCallBack)}";
                    var resNum = await samanInternetPaymentService.InitilizeInternetPayment(userId, 
                                                                                            requestFacilityId,
                                                                                            amount: waitingRequestFacility.PrePaymentOrFee,//Convert.ToInt64(Math.Round(InstallmentCalculator.CalculatePrePaymentOrFee(waitingRequestFacility.Amount, waitingRequestFacility.MonthCount))),
                                                                                            PaymentType.PaySalesCommission, 
                                                                                            "پرداخت کمیسیون فروش-پیش پرداخت", 
                                                                                            cancellationToken);

                    var token = await samanInternetPaymentService.GetToken(resNum,
                                                                           amount: waitingRequestFacility.PrePaymentOrFee,
                                                                           User.Identity!.GetMobile(),
                                                                           callBackUrl,
                                                                           cancellationToken);

                    var error = "خطا در براقراری ارتباط با درگاه بانک!";
                    if (token != null)
                    {
                        if (token.Status == 1 && !string.IsNullOrEmpty(token.Token))
                        {
                            //eturn Redirect($"{SamanInternetPaymentService.GET_TOKEN_URL}?token={token.Token}");
                            return View("PostToSamanIPG", token.Token);
                        }
                        else if (!string.IsNullOrEmpty(token.ErrorDesc))
                        {
                            error += $"<br />{token.ErrorDesc}";
                        }

                    }
                    TempData["Error"] = error;
                    return RedirectToAction(actionName: nameof(PaySalesCommission), new { requestFacilityId });
                }
            }

            return View(viewName: nameof(PaySalesCommission), model: new InternetPaymentModel()
            {
                Error = "در حال حاضر هیچ درخواست تسهیلاتی در مرحله 'پرداخت کارمزد کمیسیون فروش' وجود ندارد!"
            });
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        public async Task<ActionResult<InternetPaymentResponseModel>> PostPaySalesCommissionCallBack(CancellationToken cancellationToken)
        {
            var model = await zarinPalInternetPaymentService.ValidatePaymentResponse(HttpContext, cancellationToken);
            model.PaymentType = PaymentType.PaySalesCommission;
            if (model.IsSuccess)
            {
                var userId = new Guid(User.Identity!.GetUserId());
                await zarinPalInternetPaymentService.ApprovedPaymentSalesCommissioStep(userId, model.RequestFacilityId!.Value, cancellationToken);
            }

            return View(nameof(InternetPaymentCallBack), model: model);
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        public async Task<ActionResult<SamanInternetPaymentCallBackModel>> SamanPostPaySalesCommissionCallBack(CancellationToken cancellationToken)
        {
            var model = await samanInternetPaymentService.ValidatePaymentResponse(HttpContext, cancellationToken);
            model.PaymentType = PaymentType.PaySalesCommission;
            if (model.IsSuccess)
            {
                var userId = new Guid(User.Identity!.GetUserId());
                await samanInternetPaymentService.ApprovedPaymentSalesCommissioStep(userId, model.RequestFacilityId!.Value, cancellationToken);
            }

            return View(nameof(SamanInternetPaymentCallBack), model: model);
        }

        #endregion

        #region Internet Payment for Verify

        [CustomAuthorize(RoleEnum.Buyer)]
        [HttpGet("[controller]/[action]")]
        public async Task<ActionResult<InternetPaymentModel>> InternetPayment(CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity.GetUserId());
            var setting = await globalSettingService.GetActiveGlobalSetting(cancellationToken);
            if (await transactionService.GetBalance(userId, cancellationToken) >= setting.ValidityPeriodOfValidation)
                return RedirectToAction("Verify", "IranCreditScoring");

            var model = new InternetPaymentModel();

            await zarinPalInternetPaymentService.ValidationStatuslessPayments(userId, cancellationToken);

            if (await iranCreditScoringService.ExistVerifyResult(userId, setting.ValidityPeriodOfValidation, cancellationToken))
                model.ExistValidation = true;
            else
                model.ExistValidation = false;

            model.PaymentType = PaymentType.PayValidationFee;

            model.Error = TempData["Error"]?.ToString();
            model.Amount = GlobalSettingModel.ValidationFee;
            model.Description = "هزینه اعتبار سنجی";
            model.PostAction = Url.Action(nameof(PostInternetPaymentForVerify), "Payment");
            return View(model);
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        [HttpPost("[controller]/[action]/{ipgType}")]
        public async Task<ActionResult> PostInternetPaymentForVerify(IpgType ipgType, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity.GetUserId());
            await zarinPalInternetPaymentService.ValidationStatuslessPayments(userId, cancellationToken);

            if (ipgType == IpgType.ZarinpalIPG)
            {
                var callBackUrl = $"{GetBaseUrl}/Payment/{nameof(InternetPaymentCallBack)}";
                var authority = await zarinPalInternetPaymentService.InitilizeInternetPayment(userId, requestFacilityId: null,
                    GlobalSettingModel.ValidationFee, PaymentType.PayValidationFee,
                    callBackUrl, User.Identity.GetMobile(), "پرداخت هزینه اعتبار سنجی", cancellationToken);
                if (!string.IsNullOrEmpty(authority))
                {
                    string gatewayUrl = ZarinPalInternetPaymentService.GatewayUrl + authority;
                    return Redirect(gatewayUrl);
                }
                else
                {
                    TempData["Error"] = "خطا در براقراری ارتباط با درگاه بانک!";
                    return RedirectToAction(actionName: nameof(InternetPayment));
                }
            }
            else
            {
                var callBackUrl = $"{GetBaseUrl}/Payment/{nameof(SamanInternetPaymentCallBack)}";
                var resNum = await samanInternetPaymentService.InitilizeInternetPayment(userId, requestFacilityId: null,
                    amount: GlobalSettingModel.ValidationFee,
                    PaymentType.PayValidationFee, "پرداخت هزینه اعتبار سنجی", cancellationToken);

                var token = await samanInternetPaymentService.GetToken(resNum,
                    amount: GlobalSettingModel.ValidationFee,
                    User.Identity.GetMobile(),
                    callBackUrl,
                    cancellationToken);

                var error = "خطا در براقراری ارتباط با درگاه بانک!";
                if (token != null)
                {
                    if (token.Status == 1 && !string.IsNullOrEmpty(token.Token))
                    {
                        //eturn Redirect($"{SamanInternetPaymentService.GET_TOKEN_URL}?token={token.Token}");
                        return View("PostToSamanIPG", token.Token);
                    }
                    else if (!string.IsNullOrEmpty(token.ErrorDesc))
                    {
                        error += $"<br />{token.ErrorDesc}";
                    }

                }
                TempData["Error"] = error;
                return RedirectToAction(actionName: nameof(InternetPayment));
            }
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        public async Task<ActionResult<InternetPaymentResponseModel>> InternetPaymentCallBack(CancellationToken cancellationToken)
        {
            var model = await zarinPalInternetPaymentService.ValidatePaymentResponse(HttpContext, cancellationToken);
            model.PaymentType = PaymentType.PayValidationFee;
            if (model.IsSuccess)
            {
                var userId = new Guid(User.Identity.GetUserId());
                //await transactionService.Withdrawal(-model.Amount, userId, userId, null);

                TempData["IpgType"] = IpgType.ZarinpalIPG;
                TempData["InternetBankResponse"] = JsonConvert.SerializeObject(model);
                return RedirectToAction("Verify", "IranCreditScoring");
            }

            return View(model: model);
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        //[HttpPost]
        public async Task<ActionResult<SamanInternetPaymentCallBackModel>> SamanInternetPaymentCallBack(CancellationToken cancellationToken)
        {
            var model = await samanInternetPaymentService.ValidatePaymentResponse(HttpContext, cancellationToken);
            model.PaymentType = PaymentType.PayValidationFee;
            if (model.IsSuccess)
            {
                var userId = new Guid(User.Identity.GetUserId());
                //await transactionService.Withdrawal(-model.Amount, userId, userId, null);

                TempData["IpgType"] = IpgType.SamanIPG;
                TempData["InternetBankResponse"] = JsonConvert.SerializeObject(model);
                return RedirectToAction("Verify", "IranCreditScoring");
            }

            return View(model: model);
        }

        #region callback after payment with samat verify
        //[CustomAuthorize(RoleEnum.Buyer)]
        //public async Task<ActionResult<InternetPaymentResponseModel>> InternetPaymentCallBack(CancellationToken cancellationToken)
        //{
        //    var model = await zarinPalInternetPaymentService.ValidatePaymentResponse(HttpContext, cancellationToken);
        //    model.PaymentType = PaymentType.PayValidationFee;
        //    if (model.IsSuccess)
        //    {
        //        var userId = new Guid(User.Identity.GetUserId());
        //        if (await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(model.RequestFacilityId, new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.PaymentVerifyShahkarAndSamatService, cancellationToken) == false)
        //        {
        //            model.InquiryShahkarAndSamatServiceDone = true;
        //            return View(model: model);
        //        }
        //        await zarinPalInternetPaymentService.ApprovedPaymentVerifyShahkarAndSamatServiceStep(userId, model.RequestFacilityId, cancellationToken);

        //        if (await samatService.GetUserFacilitiesFromCentralBank(model.RequestFacilityId, userId, userId, cancellationToken) &&
        //            await samatService.GetUserBackChequesFromCentralBank(model.RequestFacilityId, userId, userId, cancellationToken))
        //        {
        //            await transactionService.WithdrawalAccreditationFee(-(GlobalSettingModel != null ? GlobalSettingModel.ValidationFee : VERIFY_PAYMENT_FEE_AMOUNT), userId, userId, model.RequestFacilityId, cancellationToken);
        //            await zarinPalInternetPaymentService.ApprovedVerifyShahkarAndSamatServiceStep(userId, userId, model.RequestFacilityId, cancellationToken);
        //            //model.InquiryShahkarAndSamatServiceDone = true;
        //            return RedirectToAction("Verify", "IranCreditScoring", new { requestFacilityId = model.RequestFacilityId });
        //        }
        //    }

        //    return View(model: model);
        //}
        #endregion callback after payment with samat verify

        #endregion

        #region Internet Payment for Installment

        [CustomAuthorize(RoleEnum.Buyer)]
        [HttpPost("[controller]/[action]/{requestFacilityInstallmentId:int}/{ipgType}")]
        public async Task<ActionResult> PostInternetPaymentForInstallment(int requestFacilityInstallmentId, IpgType ipgType, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity!.GetUserId());
            await zarinPalInternetPaymentService.ValidationStatuslessPayments(userId, cancellationToken);
            var requestFacilityInstallment = await requestFacilityInstallmentService.PrepareForPayment(requestFacilityInstallmentId, cancellationToken);

            if (requestFacilityInstallment != null)
            {
                if (requestFacilityInstallment.Paid)
                    throw new AppException("مرحله تسهیلات فوق قبلا پرداخت(تسویه) شده است!");

                if (ipgType == IpgType.ZarinpalIPG)
                {
                    var callBackUrl = $"{GetBaseUrl}/Payment/{nameof(InternetPaymentForInstallmentCallBack)}";
                    var authority = await zarinPalInternetPaymentService.InitilizeInternetPayment(userId, requestFacilityInstallment.RequestFacilityId, requestFacilityInstallmentId,
                        requestFacilityInstallment.RealPayAmount!.Value, callBackUrl, User.Identity!.GetMobile(), "پرداخت اقساط", cancellationToken);
                    if (!string.IsNullOrEmpty(authority))
                    {
                        string gatewayUrl = ZarinPalInternetPaymentService.GatewayUrl + authority;
                        return Redirect(gatewayUrl);
                    }
                    else
                    {
                        return View(viewName: nameof(InternetPayment), model: new InternetPaymentModel()
                        {
                            Error = "خطا در براقراری ارتباط با درگاه بانک!"
                        });
                    }
                }
                else
                {
                    var callBackUrl = $"{GetBaseUrl}/Payment/{nameof(SamanInternetPaymentForInstallmentCallBack)}";
                    var resNum = await samanInternetPaymentService
                                    .InitilizeInternetPayment(userId,
                                                             requestFacilityInstallment.RequestFacilityId,
                                                             requestFacilityInstallmentId,
                                                             amount:requestFacilityInstallment.RealPayAmount!.Value,
                                                             "پرداخت اقساط",
                                                             cancellationToken);

                    var token = await samanInternetPaymentService.GetToken(resNum,
                        amount:requestFacilityInstallment.RealPayAmount!.Value,
                        User.Identity!.GetMobile(),
                        callBackUrl,
                        cancellationToken);

                    var error = "خطا در براقراری ارتباط با درگاه بانک!";
                    if (token != null)
                    {
                        if (token.Status == 1 && !string.IsNullOrEmpty(token.Token))
                        {
                            //eturn Redirect($"{SamanInternetPaymentService.GET_TOKEN_URL}?token={token.Token}");
                            return View("PostToSamanIPG", token.Token);
                        }
                        else if (!string.IsNullOrEmpty(token.ErrorDesc))
                        {
                            error += $"<br />{token.ErrorDesc}";
                        }

                    }
                    throw new Exception(error);
                    //TempData["Error"] = error;
                    //return RedirectToAction(actionName: nameof(InternetPayment));
                }
            }

            return View(viewName: nameof(InternetPayment), model: new InternetPaymentModel()
            {
                PaymentType = PaymentType.PayInstallment,
                Error = "مرحله تسهیلات فوق جهت تسویه یافت نشد!"
            });
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        public async Task<ActionResult<InternetPaymentResponseModel>> InternetPaymentForInstallmentCallBack(CancellationToken cancellationToken)
        {
            var model = await zarinPalInternetPaymentService.ValidatePaymentResponse(HttpContext, cancellationToken);
            model.PaymentType = PaymentType.PayInstallment;
            if (model.IsSuccess)
            {
                var userId = new Guid(User.Identity.GetUserId());
                return View(nameof(InternetPaymentCallBack), model: model);
            }

            return View(nameof(InternetPaymentCallBack), model: model);
        }

        [CustomAuthorize(RoleEnum.Buyer)]
        public async Task<ActionResult<InternetPaymentResponseModel>> SamanInternetPaymentForInstallmentCallBack(CancellationToken cancellationToken)
        {
            var model = await samanInternetPaymentService.ValidatePaymentResponse(HttpContext, cancellationToken);
            model.PaymentType = PaymentType.PayInstallment;
            if (model.IsSuccess)
            {
                var userId = new Guid(User.Identity.GetUserId());
                return View(nameof(SamanInternetPaymentCallBack), model: model);
            }

            return View(nameof(SamanInternetPaymentCallBack), model: model);
        }

        #endregion Internet Payment for Installment

        #region Internet Payment by Buyer in Shop

        [CustomAuthorize(RoleEnum.Seller)]
        //[HttpGet("pay/{paymentInfoEncryptId}")]
        //[HttpPost("[controller]/[action]/{paymentInfoEncryptId}")]
        [HttpPost("[controller]/[action]")]
        public async Task<ActionResult> BuyerPayementInShop(string paymentInfoEncryptId, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity.GetUserId());
            var paymentId = new Guid(await SecurityHelper.DecryptAsync(WebUtility.UrlDecode(paymentInfoEncryptId), PaymentInfoController.KEY_FOR_ENCRYPT_DECRYPT));
            var paymentInfo = await paymentInfoService.Get(paymentId, userId, cancellationToken);
            if (paymentInfo == null)
            {
                return View(viewName: nameof(InternetPayment), model: new InternetPaymentModel()
                {
                    PaymentType = PaymentType.PayForBuyByBuyer,
                    Error = "لینک پرداخت نامعتبر است!"
                });
            }

            await zarinPalInternetPaymentService.ValidationStatuslessPayments(userId, cancellationToken);

            if (paymentInfo.IpgType == IpgType.ZarinpalIPG)
            {
                var callBackUrl = $"{GetBaseUrl}/Payment/{nameof(BuyerPayementInShopZarinpalCallBack)}";
                var authority = await zarinPalInternetPaymentService.InitilizeInternetPaymentBySeller(paymentInfo.BuyerId,
                                                                                                      paymentInfo.Id,
                                                                                                      amount: /*paymentInfo.Amount*/10000,
                                                                                                      paymentInfo.BuyerMobile,
                                                                                                      "پرداخت هزینه کالا",
                                                                                                      callBackUrl,
                                                                                                      cancellationToken);
                if (!string.IsNullOrEmpty(authority))
                {
                    string gatewayUrl = ZarinPalInternetPaymentService.GatewayUrl + authority;
                    return Redirect(gatewayUrl);
                }
                else
                {
                    TempData["Error"] = "خطا در براقراری ارتباط با درگاه بانک!";
                    return RedirectToAction("SearchUser", "User");
                }
            }
            else
            {
                var callBackUrl = $"{GetBaseUrl}/Payment/{nameof(BuyerPayementInShopSamanCallBack)}";
                var resNum = await samanInternetPaymentService
                                .InitilizeInternetPaymentBySeller(paymentInfo.BuyerId,
                                                                  paymentInfo.Id,
                                                                  10100,//paymentInfo.Amount,
                                                                  paymentInfo.CardNumber,
                                                                  "پرداخت هزینه کالا",
                                                                  cancellationToken);

                var token = await samanInternetPaymentService.GetToken(resNum,
                    amount:/*paymentInfo.Amount*/ 10100,
                    paymentInfo.BuyerMobile,
                    callBackUrl,
                    paymentInfo.CardNumber,
                    cancellationToken);

                var error = "خطا در براقراری ارتباط با درگاه بانک!";
                if (token != null)
                {
                    if (token.Status == 1 && !string.IsNullOrEmpty(token.Token))
                    {
                        return View("PostToSamanIPG", token.Token);
                    }
                    else if (!string.IsNullOrEmpty(token.ErrorDesc))
                    {
                        error += $"<br />{token.ErrorDesc}";
                    }

                }
                TempData["Error"] = error;
                return RedirectToAction("SearchUser", "User");
            }
        }

        [CustomAuthorize(RoleEnum.Seller)]
        public async Task<ActionResult<InternetPaymentResponseModel>> BuyerPayementInShopZarinpalCallBack(CancellationToken cancellationToken)
        {
            var model = await zarinPalInternetPaymentService.ValidatePaymentResponse(HttpContext, cancellationToken);
            model.PaymentType = PaymentType.PayForBuyByBuyer;
            if (model.IsSuccess)
            {
                var userId = new Guid(User.Identity.GetUserId());
                return View(nameof(InternetPaymentCallBack), model: model);
            }

            return View(nameof(InternetPaymentCallBack), model: model);
        }

        [CustomAuthorize(RoleEnum.Seller)]
        public async Task<ActionResult<InternetPaymentResponseModel>> BuyerPayementInShopSamanCallBack(CancellationToken cancellationToken)
        {
            var model = await samanInternetPaymentService.ValidatePaymentResponse(HttpContext, cancellationToken);
            model.PaymentType = PaymentType.PayForBuyByBuyer;
            if (model.IsSuccess)
            {
                var userId = new Guid(User.Identity.GetUserId());
                return View(nameof(SamanInternetPaymentCallBack), model: model);
            }

            return View(nameof(SamanInternetPaymentCallBack), model: model);
        }

        #endregion Internet Payment by Buyer in Shop
    }
}