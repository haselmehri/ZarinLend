using Asp.Versioning;
using Common;
using Common.Exceptions;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services;
using Services.Model.IranCreditScoring;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.ApiControllers.Controllers.v1
{

    [ApiVersion("1")]
    public class IranCreditScoringController : BaseApiController
    {
        private readonly IIranCreditScoringService iranCreditScoringService;
        private readonly IRequestFacilityService requestFacilityService;
        private readonly ITransactionService transactionService;
        private readonly IGlobalSettingService globalSettingService;
        private readonly IRequestFacilityGuarantorService requestFacilityGuarantorService;
        private readonly ISamanInternetPaymentService samanInternetPaymentService;
        private readonly IZarinPalInternetPaymentService zarinPalInternetPaymentService;
        private readonly IUserService userService;

        public IranCreditScoringController(IIranCreditScoringService iranCreditScoringService,
                                           IRequestFacilityService requestFacilityService,
                                           ITransactionService transactionService,
                                           IGlobalSettingService globalSettingService,
                                           IRequestFacilityGuarantorService requestFacilityGuarantorService,
                                           ISamanInternetPaymentService samanInternetPaymentService,
                                           IZarinPalInternetPaymentService zarinPalInternetPaymentService,
                                           IUserService userService)
        {
            this.iranCreditScoringService = iranCreditScoringService;
            this.requestFacilityService = requestFacilityService;
            this.transactionService = transactionService;
            this.globalSettingService = globalSettingService;
            this.requestFacilityGuarantorService = requestFacilityGuarantorService;
            this.samanInternetPaymentService = samanInternetPaymentService;
            this.zarinPalInternetPaymentService = zarinPalInternetPaymentService;
            this.userService = userService;
        }


        [HttpPost("[action]/{requestFacilityId:int}")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> GoToNextStepFromVerifyPaymentToZarinLend(int requestFacilityId, CancellationToken cancellationToken)
        {
            if (/*(await samanInternetPaymentService.ExistSuccessfulPayment(requestFacilityId, PaymentType.PayValidationFee, cancellationToken) ||
                 await zarinPalInternetPaymentService.ExistSuccessfulPayment(requestFacilityId, PaymentType.PayValidationFee, cancellationToken)) &&*/
                 await iranCreditScoringService.ExistVerifyResult(requestFacilityId, 30, cancellationToken) &&
                 await requestFacilityService.GetValidationMustBeRepeated(requestFacilityId, cancellationToken) == false)
            {
                var waitingRequestFacilityId = await requestFacilityService
                .GetRequestFacilityIdWaitingSpecifiedStepAndRole(new Guid(User.Identity!.GetUserId()), new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.PaymentVerifyShahkarAndSamatService, cancellationToken);
                if (waitingRequestFacilityId.HasValue)
                    return await iranCreditScoringService.GoToNextStepFromVerifyPaymentToZarinLend(new Guid(User.Identity.GetUserId()),
                                                                                                   waitingRequestFacilityId.Value,
                                                                                                   cancellationToken);
                else
                    throw new AppException("درخواست فعالی در این مرحله وجود ندارد");
            }
            else
                throw new AppException("کاربر گرامی هزینه اعتبارسنجی را پرداخت نکرده اید یا اعتبارسنجی انجام نشده است");
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer)]
        public virtual async Task<ApiResult<dynamic>> SendRequest(IranCreditScoringInputModel model, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity!.GetUserId());
            var setting = await globalSettingService.GetActiveGlobalSetting(cancellationToken);
            if (await transactionService.GetBalance(userId, cancellationToken) < setting.ValidityPeriodOfValidation)
                return new ApiResult<dynamic>(false,ApiResultStatusCode.ServerError,new { ErrorType ="AccountBalanceInsufficient"},
                    message:$"موجودی حساب شما برای انجام اعتبارسنجی کافی نیست،لطفا حساب خود را شارژ کنید{Environment.NewLine}هزینه اعتبارسنجی : {setting.ValidityPeriodOfValidation:N0} ريال");

            if (model.IranCreditScoringRequestType == IranCreditScoringRequestType.BeforeFacilityRequest)
            {
                var hashCode = await iranCreditScoringService.Request(new RequestModel()
                {
                    RealPersonNationalCode = User.Identity!.GetUserName(),
                    MobileNumber = await userService.GetMobile(userId)
                }, userId, cancellationToken);

                return new
                {
                    HashCode = hashCode
                };
            }
            else if (model.IranCreditScoringRequestType == IranCreditScoringRequestType.ForFacilityRequest)
            {
                var status = await requestFacilityService
                   .CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, model.RequestId, new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.VerifyIranCreditScoring, cancellationToken);
                if (status)
                {
                    var hashCode = await iranCreditScoringService.Request(new RequestModel()
                    {
                        RealPersonNationalCode = User.Identity!.GetUserName(),
                        MobileNumber = await userService.GetMobile(userId)
                    }, userId, cancellationToken);

                    return new
                    {
                        HashCode = hashCode
                    };
                }
                else
                {
                    throw new AppException("درخواست فعالی در این مرحله اعتبارسنجی وجود ندارد");
                }
            }
            else
            {
                return null;
                //var status = await requestFacilityGuarantorService
                //   .CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, model.RequestId, new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.VerifyIranCreditScoringByGuarantor, cancellationToken);
                //if (status)
                //{
                //    var hashCode = await iranCreditScoringService.Request(new RequestModel()
                //    {
                //        RealPersonNationalCode = User.Identity.GetUserName(),
                //        MobileNumber = await userService.GetMobile(userId)
                //    }, userId, cancellationToken);

                //    return new
                //    {
                //        HashCode = hashCode
                //    };
                //}
                //else
                //{
                //    throw new AppException("درخواست فعالی در این مرحله اعتبارسنجی وجود ندارد");
                //}
            }
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer)]
        public virtual async Task<ApiResult<dynamic>> Download(ValidateInput model, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity!.GetUserId());

            var validateResult = await iranCreditScoringService.Validate(model.HashCode, model.Otp, userId, cancellationToken);
            if (!string.IsNullOrEmpty(validateResult))
            {
                var statusResult = await iranCreditScoringService.Status(model.HashCode, userId, cancellationToken);
                if (statusResult != null && statusResult.Status == "NotMatchingRealPersonNationalCodeAndMobileNumber")
                    throw new AppException(statusResult.StatusTitle);
                while (statusResult == null || statusResult.ReportLink == null)
                {
                    Thread.Sleep(1000);
                    statusResult = await iranCreditScoringService.Status(model.HashCode, userId, cancellationToken);
                    if (statusResult != null && statusResult.Status == "NotMatchingRealPersonNationalCodeAndMobileNumber")
                        throw new AppException(statusResult.StatusTitle);
                }
                if (statusResult != null /*&& statusResult.StatusTitle == "ساخت گزارش"*/)
                {
                    var reportCode = statusResult.ReportLink.Split('/')[statusResult.ReportLink.Split('/').Length - 1];
                    if (!string.IsNullOrEmpty(reportCode))
                    {
                        var pdfBase64 = await iranCreditScoringService.PdfReport(reportCode, userId, cancellationToken);
                        var xml = string.Empty;// await iranCreditScoringService.Xml(reportCode, cancellationToken);
                        var json = await iranCreditScoringService.Json(reportCode, userId, cancellationToken);

                        var result = new IranCreditScoringResult();
                        if (!string.IsNullOrEmpty(json))
                            result = JsonConvert.DeserializeObject<IranCreditScoringResult>(json);

                        result.Score.Xml = xml;
                        result.Score.Json = json;
                        result.Score.PdfBase64String = pdfBase64;
                        result.Score.RequestId = model.RequestId;
                        result.Score.IranCreditScoringRequestType = model.IranCreditScoringRequestType;

                        await iranCreditScoringService.SaveVerify(userId, result.Score, cancellationToken);
                        var setting = await globalSettingService.GetActiveGlobalSetting(cancellationToken);
                        await transactionService.Withdrawal(-setting.ValidationFee, userId, userId, null);
                        return result;
                    }
                }
            }

            return new ApiResult<dynamic>(false, ApiResultStatusCode.ServerError, null, message: "ارتباط با سرور جهت دریافت نتیحه اعتبار سنجی برقرار نشد!");
        }

        //[HttpPost("[action]")]
        //[CustomAuthorize(RoleEnum.Buyer)]
        //public virtual async Task<object> Download(ValidateInput model, CancellationToken cancellationToken)
        //{
        //    var userId = new Guid(User.Identity.GetUserId());
        //    var tokenModel = new TokenModel()
        //    {
        //        AccessToken = model.AccessToken,
        //        ApiKey = model.ApiKey
        //    };
        //    var validateResult = await iranCreditScoringService.Validate(model.HashCode, model.Otp, tokenModel, cancellationToken);
        //    if (!string.IsNullOrEmpty(validateResult))
        //    {
        //        var statusResult = await iranCreditScoringService.Status(model.HashCode, tokenModel, cancellationToken);
        //        while(statusResult == null || statusResult.ReportLink == null)
        //        {
        //            Thread.Sleep(1000);
        //            statusResult = await iranCreditScoringService.Status(model.HashCode, tokenModel, cancellationToken);
        //        }
        //        if (statusResult != null /*&& statusResult.StatusTitle == "ساخت گزارش"*/)
        //        {
        //            var reportCode = statusResult.ReportLink.Split('/')[statusResult.ReportLink.Split('/').Length - 1];
        //            if (!string.IsNullOrEmpty(reportCode))
        //            {
        //                var pdfBase64 = await iranCreditScoringService.PdfReport(reportCode, tokenModel, cancellationToken);
        //                var xml = await iranCreditScoringService.Xml(reportCode, tokenModel, cancellationToken);
        //                if (!string.IsNullOrEmpty(xml))
        //                {
        //                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(xml);
        //                    xml = Convert.ToBase64String(plainTextBytes);
        //                }
        //                var json = await iranCreditScoringService.Json(reportCode, tokenModel, cancellationToken);
        //                var result = new IranCreditScoringResult();
        //                if (!string.IsNullOrEmpty(json))
        //                {
        //                    result = JsonConvert.DeserializeObject<IranCreditScoringResult>(json);
        //                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(json);
        //                    json = Convert.ToBase64String(plainTextBytes);
        //                }

        //                //TODO IranCreditScoring
        //                await iranCreditScoringService.ApprovedVerifyIranCreditScoreStep(userId, model.RequestFacilityId, cancellationToken);
        //                return new
        //                {
        //                    Result = result,
        //                    PdfBase64 = pdfBase64,
        //                    Xml = xml,
        //                    Json = json
        //                };
        //            }
        //        }
        //    }
        //    return null;
        //}
    }
}