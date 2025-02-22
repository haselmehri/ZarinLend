using Core.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface IAyandehSignRequestSignatureLogRepository : IBaseRepository<AyandehSignRequestSignatureLog>
    {
        Task<long> AddLog(AyandehSignRequestSignatureLog ayandehSignRequestSignatureLog);
        Task<int> UpdateLog(long id, string result, CancellationToken cancellationToken);
    }
}
