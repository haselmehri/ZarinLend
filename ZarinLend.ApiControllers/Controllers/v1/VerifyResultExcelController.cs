using Asp.Versioning;
using Common;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using Services.Dto;
using Services.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class VerifyResultExcelController : BaseApiController
    {

        private readonly ILogger<VerifyResultExcelController> logger;
        private readonly IVerifyResultExcelService verifyResultExcelServic;

        public VerifyResultExcelController(ILogger<VerifyResultExcelController> logger, IVerifyResultExcelService verifyResultExcelServic)
        {
            this.logger = logger;
            this.verifyResultExcelServic = verifyResultExcelServic;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> GetVerifyResultExcelHistory(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var VerifyResultExcelHistoryList = await verifyResultExcelServic.GetVerifyResultExcelHistory(User.Identity.GetUserLeasingId(), filter, cancellationToken);
            return new ApiResult<PagingDto<VerifyResultExcelModel>>(true, ApiResultStatusCode.Success, VerifyResultExcelHistoryList);
        }

        [HttpPost("[action]/{verifyResultExcelId:int}")]
        [CustomAuthorize(RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> GetVerifyResultExcelDetailHistory(int verifyResultExcelId, CancellationToken cancellationToken)
        {
            var VerifyResultExcelDetailList = await verifyResultExcelServic.GetVerifyResultExcelDetail(User.Identity.GetUserLeasingId(), verifyResultExcelId, cancellationToken);
            return new ApiResult<List<VerifyResultExcelDetailModel>>(true, ApiResultStatusCode.Success, VerifyResultExcelDetailList);
        }

    }
}
