using Asp.Versioning;
using Common;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using Services.Dto;
using Services.Model.IranCreditScoring;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class IranCreditScoringResultRuleController : BaseApiController
    {
        private readonly IIranCreditScoringResultRuleService iranCreditScoringResultRuleService;
        private readonly ILogger<IranCreditScoringResultRuleController> _logger;

        public IranCreditScoringResultRuleController(IIranCreditScoringResultRuleService  iranCreditScoringResultRuleService, ILogger<IranCreditScoringResultRuleController> logger)
        {
            this.iranCreditScoringResultRuleService = iranCreditScoringResultRuleService;
            _logger = logger;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<PagingDto<IranCreditScoringResultRuleListModel>> List(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            return await iranCreditScoringResultRuleService.GetList(filter, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> Add([FromForm] IranCreditScoringResultRuleAddEditModel model, CancellationToken cancellationToken)
        {
            model.CreatorId = new System.Guid(User.Identity.GetUserId());
            await iranCreditScoringResultRuleService.Add(model, cancellationToken);
            return Ok();
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> Edit([FromForm] IranCreditScoringResultRuleAddEditModel model, CancellationToken cancellationToken)
        {
            model.UpdaterId = new System.Guid(User.Identity.GetUserId());
            await iranCreditScoringResultRuleService.Edit(model, cancellationToken);
            return Ok();
        }

        [HttpDelete("[action]/{id:int}")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            await iranCreditScoringResultRuleService.Delete(id, cancellationToken);
            return Ok();
        }
    }
}
