using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Model.GlobalSetting;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.UI.Controllers
{

    public class TransactionController : BaseMvcController
    {
        private GlobalSettingViewModel GlobalSettingModel { get; set; }

        public TransactionController()
        {

        }

        [CustomAuthorize(RoleEnum.Buyer)]
        public ActionResult TransactionList()
        {
            ViewBag.Title = "تاریخچه تراکنش ها";
            return View();
        }

    }
}