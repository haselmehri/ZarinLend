using Asp.Versioning;
using AutoMapper;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class OrganizationController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IOrganizationService organizationService;

        public OrganizationController(IMapper mapper, IOrganizationService organizationService)
        {
            _mapper = mapper;
            this.organizationService = organizationService;
        }

        [HttpGet("[action]/{organizationTypeId:int}")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<List<SelectListItem>> SelectOrganizationByOganizationType(short organizationTypeId, CancellationToken cancellationToken)
        {
            return await organizationService.SelectOrganizationByOganizationType(organizationTypeId, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ActionResult<PagingDto<OrganizationModel>>> OrganizationList(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var users = await organizationService.SelectOrganization(filter, cancellationToken);

            return users;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> Add([FromForm] OrganizationModel model, CancellationToken cancellationToken)
        {
            await organizationService.Add(model, cancellationToken);
            return Ok();
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> Edit([FromForm] OrganizationModel model, CancellationToken cancellationToken)
        {
            await organizationService.Edit(model, cancellationToken);
            return Ok();
        }
    }
}
