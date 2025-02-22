using Core.Data.Repositories;
using Core.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface INeoZarinRequestSignatureLogRepository : IBaseRepository<NeoZarinRequestSignatureLog>
    {
        Task<long> AddLog(NeoZarinRequestSignatureLog neoZarinRequestSignatureLog);
        Task<int> UpdateLog(long id, string result, CancellationToken cancellationToken);
    }
}
