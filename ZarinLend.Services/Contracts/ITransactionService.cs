using Services.Dto;
using Services.Model.Transaction;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface ITransactionService 
    {
        /// <summary>
        /// withdraw verify shahkar and samat fee
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="userId"></param>
        /// <param name="requestFacilityId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Withdrawal(long amount, Guid userId, Guid creatorId, int? requestFacilityId, string description = "برداشت هزینه اعتبار سنجی", int? requestFacilityGuarantorId = null, CancellationToken cancellationToken = default);

        Task<long> GetBalance(Guid userId, CancellationToken cancellationToken);
        Task<PagingDto<TransactionModel>> GetTransactionList(Guid userId, PagingFilterDto filter, CancellationToken cancellationToken);
    }
}
