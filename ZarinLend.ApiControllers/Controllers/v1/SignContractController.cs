using Asp.Versioning;
using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;
using WebFramework.StimulsoftHelper;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class SignContractController(ISignContractService signContractService,
                                        IRequestFacilityService requestFacilityService,
                                        IWebHostEnvironment webHostEnvironment,
                                        IStimulsoftHelper stimulsoftHelper,
                                        ILogger<SignContractController> logger) : BaseApiController
    {
        [HttpPost("[action]/{requestFacilityId:int}")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> SignContractByNeoZarin(int requestFacilityId, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity!.GetUserId());
            if (await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer },
                WorkFlowFormEnum.SignContractByUser, cancellationToken) ||
                await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer },
                WorkFlowFormEnum.WaitingToSignContractByUser, cancellationToken))
            {
                var report = await stimulsoftHelper.GetContractReport(requestFacilityId, cancellationToken);
                var pdfResult = stimulsoftHelper.ExportToPdf(report);
                var result = await signContractService.SignContractByNeoZarin(userId, requestFacilityId, pdfResult.Data, GetBaseUrl, cancellationToken);
                return new ApiResult<string>(result.Item1, ApiResultStatusCode.Success, result.Item2);
            }

            throw new AppException(ApiResultStatusCode.LogicError, "درخواست فوق یافت نشد/یا در مرحله 'امضای قرارداد/یا در انتظار امضای قرارداد' نمی باشد");
        }

        [HttpPost("[action]/{requestFacilityId:int}")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> SignContractByAyandehSign(int requestFacilityId, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity!.GetUserId());
            if (await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer },
                WorkFlowFormEnum.SignContractByUser, cancellationToken) ||
                await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer },
                WorkFlowFormEnum.WaitingToSignContractByUser, cancellationToken))
            {
                var report = await stimulsoftHelper.GetContractReport(requestFacilityId, cancellationToken);
                var pdfResult = stimulsoftHelper.ExportToPdf(report);
                var signingToken = await signContractService.SignContractByAyandehSign(userId, requestFacilityId, pdfResult.Data, cancellationToken);

                return new ApiResult(signingToken, ApiResultStatusCode.Success);
            }

            throw new AppException(ApiResultStatusCode.LogicError, "درخواست فوق یافت نشد/یا در مرحله 'امضای قرارداد/یا در انتظار امضای قرارداد' نمی باشد");
        }

        [HttpPost("[action]/{requestFacilityId:int}")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> SignContractByManagerOnAyandehSign(int requestFacilityId, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity!.GetUserId());
            //if (await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId,
            //                                                                                 User.Identity!.GetUserLeasingId(),
            //                                                                                 new List<RoleEnum> { RoleEnum.AdminBankLeasing },
            //                                                                                 WorkFlowFormEnum.AdminBankLeasingSignature,
            //                                                                                 cancellationToken))
            //{
            var result = await signContractService.SignContractByAyandehSign(userId, User.Identity!.GetUserLeasingId(), requestFacilityId, false, cancellationToken);
            return new ApiResult(result, ApiResultStatusCode.Success);
            //}

            //throw new AppException(ApiResultStatusCode.LogicError, "درخواست فوق یافت نشد/یا در مرحله 'امضای قرارداد/یا در انتظار امضای قرارداد' نمی باشد");
        }

        [HttpPost("[action]/{requestFacilityId:int}")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> SignContractByManagerOnAyandehSignWithoutChangeStatus(int requestFacilityId, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity!.GetUserId());
            var result = await signContractService.SignContractByAyandehSign(userId, User.Identity!.GetUserLeasingId(), requestFacilityId, true, cancellationToken);
            return new ApiResult(result, ApiResultStatusCode.Success);
        }
    }
}
