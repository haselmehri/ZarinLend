using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using Services.Dto;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Configuration;
using WebFramework.Filters;

namespace Web.UI.Controllers
{
    public class SamatController : BaseMvcController
    {
        private readonly ILogger<SamatController> logger;
        private readonly ISamatService samatService;

        public SamatController(ILogger<SamatController> logger, ISamatService samatService)
        {
            this.logger = logger;
            this.samatService = samatService;
        }


        [HttpPost("[controller]/[action]/{requestFacilityId:int}/{buyerId:guid}")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        public virtual async Task<IActionResult> ReInquiryActiveFacilities(int requestFacilityId, Guid buyerId, CancellationToken cancellationToken)
        {
            await samatService.GetUserFacilitiesFromCentralBank(requestFacilityId, buyerId, new Guid(User.Identity.GetUserId()), cancellationToken);
            return ViewComponent("UserActiveFacilities", new { requestFacilityId = requestFacilityId });
        }

        [HttpPost("[controller]/[action]/{requestFacilityId:int}/{buyerId:guid}")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.BankLeasing)]
        public virtual async Task<IActionResult> ReInquiryCheques(int requestFacilityId, Guid buyerId, CancellationToken cancellationToken)
        {
            await samatService.GetUserBackChequesFromCentralBank(requestFacilityId, buyerId, new Guid(User.Identity.GetUserId()), cancellationToken);
            return ViewComponent("UserBackCheques", new { requestFacilityId = requestFacilityId });
        }
    }
}