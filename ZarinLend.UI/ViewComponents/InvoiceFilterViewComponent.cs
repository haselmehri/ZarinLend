using Common.Utilities;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using Services.Model.Invoice;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class InvoiceFilterViewComponent : ViewComponent
    {
        private readonly IOrganizationService organizationService;

        public InvoiceFilterViewComponent(IOrganizationService organizationService)
        {
            this.organizationService = organizationService;
        }

        public async Task<IViewComponentResult> InvokeAsync(CancellationToken cancellationToken = default)
        {
            var model = new InvoiceFilterModel()
            {
                Shops = await organizationService.SelectOrganizationByOganizationType((short)OrganizationTypeEnum.Shop, cancellationToken),
                StatusList = new List<SelectListItem>
                 {
                     new SelectListItem(){Value = ((int)InvoiceStatus.Register).ToString(), Text =InvoiceStatus.Register.ToDisplay()},
                     new SelectListItem(){Value = ((int)InvoiceStatus.UploadInvoice).ToString(), Text =InvoiceStatus.UploadInvoice.ToDisplay()},
                     new SelectListItem(){Value = ((int)InvoiceStatus.WaitingVerify).ToString(), Text =InvoiceStatus.WaitingVerify.ToDisplay()},
                     new SelectListItem(){Value = ((int)InvoiceStatus.Reject).ToString(), Text =InvoiceStatus.Reject.ToDisplay()},
                     new SelectListItem(){Value = ((int)InvoiceStatus.Approve).ToString(), Text =InvoiceStatus.Approve.ToDisplay()}
                 }
            };
            ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            ViewBag.PersianStartDate = DateTime.Now.AddMonths(-1).GregorianToShamsi();

            ViewBag.EndDate = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.PersianEndDate = DateTime.Now.GregorianToShamsi();
            return await Task.FromResult((IViewComponentResult)View("InvoiceFilter", model));
        }
    }
}
