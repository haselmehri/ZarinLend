using Services.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IRequestFacilityGuarantorWarrantyService
    {
        Task<RequestFacilityGuarantorWarrantyModel> GetWarantyDocument(Guid userId, int requestFacilityGuarantorId, CancellationToken cancellationToken);
        Task<RequestFacilityGuarantorWarrantyModel> GetRequestFacilityGuarantorWarranty(Guid userId, int requestFacilityGuarantorId, CancellationToken cancellationToken);
        Task UploadWaranties(RequestFacilityGuarantorWarrantyModel model, CancellationToken cancellationToken = default);
    }
}