using Common;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Model;
using Services.Model.IranCreditScoring;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.UI.Controllers
{
    public class RequestFacilityInsuranceIssuanceController : BaseMvcController
    {
        private readonly IRequestFacilityService requestFacilityService;

        public RequestFacilityInsuranceIssuanceController(IRequestFacilityService requestFacilityService)
        {
            this.requestFacilityService = requestFacilityService;
        }
    }
}