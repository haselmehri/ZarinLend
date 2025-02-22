using Services.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IRequestFacilityCardIssuanceService
    {
        Task CardIssuance(RequestFacilityCardIssuanceModel model, CancellationToken cancellationToken = default);
        Task EditBonCard(RequestFacilityEditCardIssuanceModel model, CancellationToken cancellationToken = default);
        Task<RequestFacilityCardIssuanceModel> GetCardIssuance(int requestFacilityId, CancellationToken cancellationToken = default);
    }
}