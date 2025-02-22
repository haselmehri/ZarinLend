using Asp.Versioning;
using Common;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using Services.Dto;
using Services.Model.GlobalSetting;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class GlobalSettingController : BaseApiController
    {
        private readonly IGlobalSettingService globalSettingService;
        private readonly ILogger<GlobalSettingController> _logger;

        public GlobalSettingController(IGlobalSettingService globalSettingService, ILogger<GlobalSettingController> logger)
        {
            this.globalSettingService = globalSettingService;
            _logger = logger;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<PagingDto<GlobalSettingViewModel>> GetGlobalSettingList(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            return await globalSettingService.GetGlobalSettingList(filter, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> Add([FromForm] GlobalSettingModel globalSettingModel, CancellationToken cancellationToken)
        {
            globalSettingModel.CreatorId = new System.Guid(User.Identity!.GetUserId());
            globalSettingModel.UpdaterId = new System.Guid(User.Identity!.GetUserId());
            await globalSettingService.Add(globalSettingModel, cancellationToken);
            return Ok();
        }

    }
}
