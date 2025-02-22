using Services.Model;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Services
{
    public interface IRequestFacilityPromissoryService
    {
        Task<RequestFacilityPromissoryModel?> GetFinalizePromissory(Guid userId, int requestFacilityId, CancellationToken cancellationToken);
        Task<RequestFacilityPromissoryModel> GetRequestFacilityPromissory(Guid userId, int requestFacilityId, CancellationToken cancellationToken);
        Task SendSignRequest(int requestFacilityId, Guid userId, CancellationToken cancellationToken);
        Task PromissoryFinalize(int requestFacilityId, Guid userId, CancellationToken cancellationToken);
    }
}