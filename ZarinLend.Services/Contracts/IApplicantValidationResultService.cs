using Services.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;

namespace Services
{
    public interface IApplicantValidationResultService
    {
        Task<List<ApplicantValidationResultModel>> SelectValidationResult(int requestFacilityId, int organizationId, CancellationToken cancellationToken);
        Task<List<ApplicantValidationResultModel>> SelectValidationResult(int requestFacilityId, CancellationToken cancellationToken);
        List<ApplicantValidationResultModel> ExtractVerifyResultFromExcel(Stream stream);
    }
}