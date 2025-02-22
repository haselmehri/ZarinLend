using Common.Utilities;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class RequestFacilityFilterViewComponent : ViewComponent
    {
        private readonly IRequestFacilityService requestFacilityService;

        public RequestFacilityFilterViewComponent( IRequestFacilityService requestFacilityService)
        {
            this.requestFacilityService = requestFacilityService;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool showWaitingStepsFilter = false,bool showFacilityStatus = false,CancellationToken cancellationToken = default)
        {
            ViewBag.ShowWaitingSteps = showWaitingStepsFilter;
            ViewBag.ShowFacilityStatus = showFacilityStatus;

            ViewBag.StartDate = DateTime.Now.AddMonths(-1).ToString("yyyy/MM/dd");
            ViewBag.PersianStartDate = DateTime.Now.AddMonths(-1).GregorianToShamsi();

            ViewBag.EndDate = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.PersianEndDate = DateTime.Now.GregorianToShamsi();

            var model = await requestFacilityService.PrepareFilterModelForSearch(new List<RoleEnum>() { RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.ZarinLendExpert }, cancellationToken);
            return await Task.FromResult((IViewComponentResult)View("RequestFacilityFilter", model));
        }
    }
}
