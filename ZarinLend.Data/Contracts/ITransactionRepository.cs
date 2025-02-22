using Core.Entities.Business.Transaction;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface ITransactionRepository : IBaseRepository<Transaction>
    {
        Task<long> GetBalance(Guid userId, CancellationToken cancellationToken);
    }
}