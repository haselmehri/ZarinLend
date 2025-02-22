using Core.Entities.Business.Transaction;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface IWalletTransactionRepository : IRepository<WalletTransaction>
    {

        /// <summary>
        /// اعتبار 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<long> GetBalance(Guid userId, CancellationToken cancellationToken);
     
    }
}