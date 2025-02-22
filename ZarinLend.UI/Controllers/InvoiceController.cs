using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.UI.Controllers
{
    public class InvoiceController : BaseMvcController
    {
        public InvoiceController()
        {
        }

        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.ZarinLendExpert)]
        public IActionResult Search()
        {
            return View();
        }

        [CustomAuthorize(RoleEnum.Seller)]
        public IActionResult List()
        {
            return View();
        }
    }
}