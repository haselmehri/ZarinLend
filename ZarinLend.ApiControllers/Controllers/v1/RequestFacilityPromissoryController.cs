using Asp.Versioning;
using Common;
using Common.Exceptions;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class RequestFacilityPromissoryController(IRequestFacilityPromissoryService requestFacilityPromissoryService,
                                                     IRequestFacilityService requestFacilityService,
                                                     ILogger<RequestFacilityPromissoryController> logger) : BaseApiController
    {
        [HttpPost("[action]/{requestFacilityId:int}")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> SendSignRequest(int requestFacilityId, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity!.GetUserId());
            if (await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer },
                WorkFlowFormEnum.SignPromissoryByUser, cancellationToken) ||
                await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer },
                WorkFlowFormEnum.WaitingSignPromissoryByUser, cancellationToken))
            {
                await requestFacilityPromissoryService.SendSignRequest(requestFacilityId, userId, cancellationToken);
                return new ApiResult(true, ApiResultStatusCode.Success);
            }

            throw new AppException(ApiResultStatusCode.LogicError, "درخواست فوق یافت نشد/یا در مرحله 'امضای سفته/یا در انتظار امضای سفته' نمی باشد");
        }

        [HttpPost("[action]/{requestFacilityId:int}")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> PromissoryFinalize(int requestFacilityId, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity!.GetUserId());
            if (await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer },
                WorkFlowFormEnum.WaitingSignPromissoryByUser, cancellationToken))
            {
                await requestFacilityPromissoryService.PromissoryFinalize(requestFacilityId, userId, cancellationToken);
                return new ApiResult(true, ApiResultStatusCode.Success);
            }

            throw new AppException(ApiResultStatusCode.LogicError, "درخواست فوق یافت نشد/یا در مرحله 'در انتظار امضای سفته' نمی باشد");
        }
    }
}