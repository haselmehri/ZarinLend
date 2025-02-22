using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class OrganizationController : BaseMvcController
    {
        private readonly IOrganizationService organizationService;

        public OrganizationController(IOrganizationService organizationService)
        {
            this.organizationService = organizationService;
        }

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin)]
        public async Task<ActionResult<OrganizationModel>> Add(CancellationToken cancellationToken)
        {
            var model = await organizationService.PrepareModelForAdd(cancellationToken);
            return View("AddEdit", model);
        }

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin)]
        public async Task<ActionResult<OrganizationModel>> Edit(int Id, CancellationToken cancellationToken)
        {
            var model = await organizationService.PrepareModelForEdit(Id, cancellationToken);
            model.IsEditMode = true;
            return View(viewName: "AddEdit", model: model);
        }

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin)]
        public IActionResult List()
        {
            return View();
        }
    }
}