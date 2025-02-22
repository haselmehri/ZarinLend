using Microsoft.AspNetCore.Http;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IRequestFacilityWarrantyService
    {
        Task<List<DocumentModel>> GetRequestFacilityWarantyDocument(Guid userId, int requestFacilityId, CancellationToken cancellationToken);
        Task<List<RequestFacilityWarrantyModel>> GetRequestFacilityWaranty(Guid userId, int requestFacilityId, CancellationToken cancellationToken);
        Task UploadWaranties(RequestFacilityWarrantyAddModel model, CancellationToken cancellationToken = default);
    }
}