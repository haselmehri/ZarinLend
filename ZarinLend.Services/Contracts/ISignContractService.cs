using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface ISignContractService
    {
        Task<Tuple<bool,string>> SignContractByNeoZarin(Guid userId, int requestFacilityId, byte[] contract,string baseUrl, CancellationToken cancellationToken = default);
        Task<bool> SignContractByAyandehSign(Guid userId, int requestFacilityId, byte[] contract, CancellationToken cancellationToken = default);
        Task<bool> SignContractByAyandehSign(Guid bankManagerUserId, int leasingId, int requestFacilityId, bool justSignContract, CancellationToken cancellationToken = default);
    }
}