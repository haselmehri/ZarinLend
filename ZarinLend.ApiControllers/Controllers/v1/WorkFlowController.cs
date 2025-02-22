using Asp.Versioning;
using Common;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using Services.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class WorkFlowController : BaseApiController
    {
        private readonly IWorkFlowStepErrorService workFlowStepErrorService;
        private readonly ILogger<WorkFlowController> logger;

        public WorkFlowController(IWorkFlowStepErrorService workFlowStepErrorService,ILogger<WorkFlowController> logger)
        {
            this.workFlowStepErrorService = workFlowStepErrorService;
            this.logger = logger;
        }


        [HttpPost("[action]/{workFlowId:int}")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> GetWorkFlowDefaultErrors(int workFlowId, CancellationToken cancellationToken)
        {
            var result = await workFlowStepErrorService.GetWorkFlowDefaultErrors((WorkFlowEnum)workFlowId, cancellationToken);
            return new ApiResult<List<WorkFlowStepErrorModel>>(true, ApiResultStatusCode.Success, result);
        }
    }
}
