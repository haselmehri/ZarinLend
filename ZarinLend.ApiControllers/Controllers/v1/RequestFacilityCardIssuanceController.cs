using Asp.Versioning;
using Common;
using Common.Exceptions;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    public class RequestFacilityCardIssuanceController : BaseApiController
    {
        private readonly IRequestFacilityCardIssuanceService requestFacilityInsuranceIssuanceService;
        private readonly IRequestFacilityService requestFacilityService;
        private readonly ILogger<RequestFacilityInsuranceIssuanceController> logger;

        public RequestFacilityCardIssuanceController(IRequestFacilityCardIssuanceService requestFacilityInsuranceIssuanceService,
            IRequestFacilityService requestFacilityService,
            ILogger<RequestFacilityInsuranceIssuanceController> logger)
        {
            this.requestFacilityInsuranceIssuanceService = requestFacilityInsuranceIssuanceService;
            this.requestFacilityService = requestFacilityService;
            this.logger = logger;
        }


        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> CardIssuance([FromForm] RequestFacilityCardIssuanceModel model, CancellationToken cancellationToken)
        {
            model.CreatorId = new Guid(User.Identity!.GetUserId());
            if (await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(model.RequestFacilityId
                , new List<RoleEnum> { RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.ZarinLendExpert },
                WorkFlowFormEnum.CardIssuance, cancellationToken))
            {
                model.CardNumber = model.CardNumber.Replace("-", string.Empty);
                await requestFacilityInsuranceIssuanceService.CardIssuance(model, cancellationToken);
                return new ApiResult(true, ApiResultStatusCode.Success);
            }

            throw new AppException(ApiResultStatusCode.LogicError, "درخواست فوق یافت نشد/یا در مرحله 'صدور بن کارت' نمی باشد");
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> EditBonCard([FromForm] RequestFacilityEditCardIssuanceModel model, CancellationToken cancellationToken)
        {
            model.CreatorId = new Guid(User.Identity!.GetUserId());
            if (await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(model.RequestFacilityId
                , new List<RoleEnum> { RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.ZarinLendExpert },
                WorkFlowFormEnum.CompleteBonCardInfo, cancellationToken))
            {
                await requestFacilityInsuranceIssuanceService.EditBonCard(model, cancellationToken);
                return new ApiResult(true, ApiResultStatusCode.Success);
            }

            throw new AppException(ApiResultStatusCode.LogicError, "درخواست فوق یافت نشد/یا در مرحله 'صدور بن کارت' نمی باشد");
        }

        [HttpPost("[action]/{requestFacilityId:int}")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert, RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> GetCardIssuance(int requestFacilityId, CancellationToken cancellationToken)
        {
            var model = await requestFacilityInsuranceIssuanceService.GetCardIssuance(requestFacilityId, cancellationToken);
            return new ApiResult<RequestFacilityCardIssuanceModel>(true, ApiResultStatusCode.Success, model);
        }
    }
}
