using Services.Model.WalletTransaction;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IWalletTransactionService
    {
        /// <summary>
        /// اعتبار 
        /// </summary>
        /// <param name="userId">'userId' is agentId</param>
        /// <returns></returns>
        Task<long> GetBalance(Guid userId, CancellationToken cancellationToken);
        Task<WalletBalanceBaseCardNumber> GetWalletBalanceBaseCard(string cardNumber, Guid userId, CancellationToken cancellationToken);
        Task<List<WalletBalanceBaseCardNumber>> GetWalletBalanceBaseCards(Guid userId, CancellationToken cancellationToken);
        //Task TransferCreditAmountFromBuyerToSeller(SendSmsVerificationModel model, CancellationToken cancellationToken);
    }
}