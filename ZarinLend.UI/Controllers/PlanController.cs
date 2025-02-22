using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Model;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.UI.Controllers
{
    public class PlanController : BaseMvcController
    {
        private readonly IPlanService planService;

        public PlanController(IPlanService planService)
        {
            this.planService = planService;
        }

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin)]
        public async Task<ActionResult<PlanModel>> Add(CancellationToken cancellationToken)
        {
            ViewBag.Title = "ایجاد طرح تسهیلاتی جدید";
            var model = await planService.PrepareModelForAdd(cancellationToken);
            return View("Add", model);
        }

        //[CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin)]
        //public async Task<ActionResult<OrganizationModel>> Edit(int Id, CancellationToken cancellationToken)
        //{
        //    var model = await organizationService.PrepareModelForEdit(Id, cancellationToken);
        //    model.IsEditMode = true;
        //    return View(viewName: "AddEdit", model: model);
        //}

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin)]
        public IActionResult List()
        {
            ViewBag.Title = "لیست طرح های تسهیلات";
            return View();
        }
    }
}