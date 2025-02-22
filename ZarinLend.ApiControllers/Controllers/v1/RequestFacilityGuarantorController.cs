using Asp.Versioning;
using Common;
using Common.Exceptions;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using Services.Dto;
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
    public class RequestFacilityGuarantorController : BaseApiController
    {

        private readonly ILogger<RequestFacilityGuarantorController> logger;
        private readonly IRequestFacilityGuarantorService requestFacilityGuarantorService;
        private readonly IRequestFacilityService requestFacilityService;

        public RequestFacilityGuarantorController(ILogger<RequestFacilityGuarantorController> logger, IRequestFacilityGuarantorService requestFacilityGuarantorService,
            IRequestFacilityService requestFacilityService)
        {
            this.logger = logger;
            this.requestFacilityGuarantorService = requestFacilityGuarantorService;
            this.requestFacilityService = requestFacilityService;
        }

        [HttpPost("[action]/{requestFacilityId:int}")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> RegisterGuarantor(int requestFacilityId, CancellationToken cancellationToken)
        {
            if (!await requestFacilityService.IsOpenRequestFacility(requestFacilityId, cancellationToken))
                throw new AppException("درخواست تسهیلات فوق فعال نمی باشد یا در مرحله ثبت ضامن نمی باشد!");

            var installmentList = await requestFacilityGuarantorService.RegisterGuarantor(new Guid(User.Identity.GetUserId()), requestFacilityId, cancellationToken);
            return new ApiResult(true, ApiResultStatusCode.Success);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<PagingDto<RequestFacilityGuarantorListModel>> GetRequests(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            filter.FilterList.Add(new Filter()
            {
                Operator = Operator.Equal,
                PropertyName = "GuarantorUserId",
                PropertyValue = new Guid(User.Identity.GetUserId())
            });
            return await requestFacilityGuarantorService.GetRequests(filter, new List<RoleEnum>() { RoleEnum.Buyer }, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<PagingDto<RequestFacilityGuarantorListModel>> GetAllRequests(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            return await requestFacilityGuarantorService.GetRequests(filter, new List<RoleEnum>() { RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.ZarinLendExpert }, cancellationToken);
        }

        [HttpPost("[action]/{requestFacilityGuarantorId:int}")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> CancelByUser(int requestFacilityGuarantorId, CancellationToken cancellationToken)
        {
            await requestFacilityGuarantorService.CancelByUser(requestFacilityGuarantorId, new Guid(User.Identity.GetUserId()), cancellationToken);
            return new ApiResult(true, ApiResultStatusCode.Success);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> VerifyGuarantorByZarinLend(RequestFacilityGuarantorStatusModel model, CancellationToken cancellationToken)
        {
            model.OpratorId = new Guid(User.Identity.GetUserId());
            return await requestFacilityGuarantorService.VerifyGuarantorByZarinLend(model, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<bool> VerifyGuaranteesByZarinLend(RequestFacilityGuarantorStatusModel model, CancellationToken cancellationToken)
        {
            model.OpratorId = new Guid(User.Identity.GetUserId());
            return await requestFacilityGuarantorService.VerifyGuaranteesByZarinLend(model, cancellationToken);
        }
    }
}
