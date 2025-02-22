using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IRequestFacilityInsuranceIssuanceService
    {
        Task InsuranceIssuance(RequestFacilityInsuranceIssuanceModel model, IFormFile file, CancellationToken cancellationToken = default);
    }
}