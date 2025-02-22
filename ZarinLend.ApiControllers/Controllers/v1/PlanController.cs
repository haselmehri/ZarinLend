using Asp.Versioning;
using Common;
using Common.CustomAttribute;
using Common.CustomFileAttribute;
using Common.Exceptions;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services;
using Services.Dto;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class PlanController : BaseApiController
    {
        private readonly IPlanService planService;
        private readonly ILocationService locationService;

        public PlanController(IPlanService planService, ILocationService locationService)
        {
            this.planService = planService;
            this.locationService = locationService;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<PagingDto<PlanListModel>> SelectPlans(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var plans = await planService.SelectPlans(filter, cancellationToken);

            return plans;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [RequestSizeLimit(bytes: 1 * 1024 * 1024)]
        public virtual async Task<List<PlanMemberModel>> VerifyPlanMembesExcel([MaxFileSize(1 * 1024 * 1024/*Max File Size : 1MB*/),
            AllowedExtensions(new string[] {".xls",".xlsx"},errorMessage:"فقط فایل هایی از نوع اکسل با پسوند (xls,xlsx) می توانید بارگذاری کنید!" )] IFormFile membersExcelFile,
            CancellationToken cancellationToken)
        {

            if (membersExcelFile == null || membersExcelFile.Length == 0)
                throw new AppException(ApiResultStatusCode.LogicError, "لطفا فایل اکسل اعضای طرح را جهت بارگذاری،انتخاب کنید!");

            return await planService.VerifyPlanMembesExcel(membersExcelFile, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> Add([FromForm] PlanAddModel model, IFormFile membersExcelFile, CancellationToken cancellationToken)
        {
            model.Members = JsonConvert.DeserializeObject<List<PlanMemberModel>>(model.JsonMembers);
            if (!model.Members.Any())
                throw new AppException("لیست اعضاء طرح خالی می باشد");

            model.CreatorId = new Guid(User.Identity.GetUserId());
            await planService.Add(model, membersExcelFile, cancellationToken);
            return Ok();
        }

        [HttpPost("[action]/{planId:int}")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<List<PlanMemberModel>> GetMembers(int planId, CancellationToken cancellationToken)
        {
           return await planService.GetMembers( planId, cancellationToken);
        }
    }
}
