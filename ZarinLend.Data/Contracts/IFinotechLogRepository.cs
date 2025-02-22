using Core.Data.Repositories;
using Core.Entities;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface IFinotechLogRepository : IBaseRepository<FinotechLog>
    {
        Task<long> AddLog(FinotechLog finotechLog);
        Task<long> UpdateLog(long id, string result);
    }
}
