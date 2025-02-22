using Core.Entities.Business.Transaction;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface IWalletTransactionRepository : IBaseRepository<WalletTransaction>
    {
        Task<long> GetBalance(Guid userId, CancellationToken cancellationToken);
    }
}