using Services.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;
using Services.Dto;

namespace Services
{
    public interface IVerifyResultExcelService
    {
        Task SaveVerifyResultFromFile(IFormFile verifyExcelFile, List<ApplicantValidationResultModel> extractResultFromFile, int organizationId,
            Guid creator, CancellationToken cancellationToken = default);

        Task<PagingDto<VerifyResultExcelModel>> GetVerifyResultExcelHistory(int leasingId, PagingFilterDto filter, CancellationToken cancellationToken);
        Task<List<VerifyResultExcelDetailModel>> GetVerifyResultExcelDetail(int leasingId, int verifyResultExcelId, CancellationToken cancellationToken);
    }
}