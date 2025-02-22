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
    public class RequestFacilityGuarantorWarrantyController : BaseApiController
    {
        private readonly IRequestFacilityGuarantorWarrantyService requestFacilityGuarantorWarrantyService;
        private readonly IRequestFacilityGuarantorService requestFacilityGuarantorService;
        private readonly ILogger<RequestFacilityGuarantorWarrantyController> _logger;

        public RequestFacilityGuarantorWarrantyController(IRequestFacilityGuarantorWarrantyService requestFacilityGuarantorWarrantyService, 
                                                          IRequestFacilityGuarantorService requestFacilityGuarantorService,
                                                          ILogger<RequestFacilityGuarantorWarrantyController> logger)
        {
            this.requestFacilityGuarantorWarrantyService = requestFacilityGuarantorWarrantyService;
            this.requestFacilityGuarantorService = requestFacilityGuarantorService;
            _logger = logger;
        }


        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> UploadWaranties([FromForm] RequestFacilityGuarantorWarrantyModel model, CancellationToken cancellationToken)
        {
            if (model.ChequeFile == null)
                throw new AppException(ApiResultStatusCode.LogicError, "لطفا تصویر چک را جهت بارگذاری انتخاب کنید!");

            model.UserId = new Guid(User.Identity.GetUserId());
            if (await requestFacilityGuarantorService.CheckRequestFacilityWaitingSpecifiedStepAndRole(model.UserId, model.RequestFacilityGuarantorId, new List<RoleEnum> { RoleEnum.Buyer },
                WorkFlowFormEnum.RegisterGuaranteesByGuarantor, cancellationToken))
            {
                await requestFacilityGuarantorWarrantyService.UploadWaranties(model, cancellationToken);
                return new ApiResult(true, ApiResultStatusCode.Success);
            }

            throw new AppException(ApiResultStatusCode.LogicError, "درخواست فوق یافت نشد/یا در مرحله 'ثبت و بارگذاری چک' نمی باشد");
        }
    }
}
