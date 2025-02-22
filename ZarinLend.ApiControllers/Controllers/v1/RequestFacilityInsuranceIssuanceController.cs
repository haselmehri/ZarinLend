using Asp.Versioning;
using Common;
using Common.Exceptions;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class RequestFacilityInsuranceIssuanceController : BaseApiController
    {
        private readonly IRequestFacilityInsuranceIssuanceService requestFacilityInsuranceIssuanceService;
        private readonly IRequestFacilityService requestFacilityService;
        private readonly ILogger<RequestFacilityInsuranceIssuanceController> logger;

        public RequestFacilityInsuranceIssuanceController(IRequestFacilityInsuranceIssuanceService requestFacilityInsuranceIssuanceService, 
            IRequestFacilityService requestFacilityService,
            ILogger<RequestFacilityInsuranceIssuanceController> logger)
        {
            this.requestFacilityInsuranceIssuanceService = requestFacilityInsuranceIssuanceService;
            this.requestFacilityService = requestFacilityService;
            this.logger = logger;
        }


        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> InsuranceIssuance([FromForm] RequestFacilityInsuranceIssuanceModel model, IFormFile file, CancellationToken cancellationToken)
        {
            //if (file == null)
            //    throw new AppException(ApiResultStatusCode.LogicError, "لطفا تصویر چک را جهت بارگذاری انتخاب کنید!");

            model.CreatorId = new Guid(User.Identity.GetUserId());
            if (await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole( model.RequestFacilityId
                , new List<RoleEnum> { RoleEnum.SuperAdmin, RoleEnum.Admin },
                WorkFlowFormEnum.InsuranceIssuance, cancellationToken))
            {
                await requestFacilityInsuranceIssuanceService.InsuranceIssuance(model, file, cancellationToken);
                return new ApiResult(true, ApiResultStatusCode.Success);
            }

            throw new AppException(ApiResultStatusCode.LogicError, "درخواست فوق یافت نشد/یا در مرحله 'صدور بیمه نمی باشد' نمی باشد");
        }
    }
}
