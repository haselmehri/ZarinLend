using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface INeoZarinService
    {
        Task<bool> SignContract(int requestFacilityId, Guid creatorId, string callbackUrl, string fileUrl, string mobile, string trackId,
            string description, CancellationToken cancellationToken);
    }
}