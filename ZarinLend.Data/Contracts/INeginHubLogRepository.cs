using Core.Entities;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface INeginHubLogRepository : IBaseRepository<NeginHubLog>
    {
        Task<long> AddLog(NeginHubLog finotechLog);
        Task<long> UpdateLog(long id, string result);
    }
}
