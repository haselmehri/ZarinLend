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
    public class RequestFacilityWarrantyController : BaseApiController
    {
        private readonly IRequestFacilityWarrantyService requestFacilityWarrantyService;
        private readonly IRequestFacilityService requestFacilityService;
        private readonly ILogger<RequestFacilityWarrantyController> _logger;

        public RequestFacilityWarrantyController(IRequestFacilityWarrantyService requestFacilityWarrantyService, 
                                                 IRequestFacilityService requestFacilityService,
                                                 ILogger<RequestFacilityWarrantyController> logger)
        {
            this.requestFacilityWarrantyService = requestFacilityWarrantyService;
            this.requestFacilityService = requestFacilityService;
            _logger = logger;
        }


        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> UploadWaranties([FromForm] RequestFacilityWarrantyAddModel model, CancellationToken cancellationToken)
        {
            if (model.ChequeFile == null)
                throw new AppException(ApiResultStatusCode.LogicError, "لطفا تصویر چک را جهت بارگذاری انتخاب کنید!");

            if (model.PromissoryNoteFile == null)
                throw new AppException(ApiResultStatusCode.LogicError, "لطفا فایل PDF سفته را جهت بارگذاری انتخاب کنید!");

            model.UserId = new Guid(User.Identity!.GetUserId());
            if (await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(model.UserId, model.RequestFacilityId, new List<RoleEnum> { RoleEnum.Buyer },
                WorkFlowFormEnum.RegisterGuarantees, cancellationToken))
            {
                await requestFacilityWarrantyService.UploadWaranties(model, cancellationToken);
                return new ApiResult(true, ApiResultStatusCode.Success);
            }

            throw new AppException(ApiResultStatusCode.LogicError, "درخواست فوق یافت نشد/یا در مرحله 'ثبت و بارگذاری چک' نمی باشد");
        }
    }
}
