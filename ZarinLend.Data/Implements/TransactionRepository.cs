﻿using Common;
using Common.Exceptions;
using Common.Utilities;
using Core.Entities.Business.Transaction;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository, IScopedDependency
    {
        public TransactionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        /// <summary>
        /// اعتبار واقعی 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<long> GetBalance(Guid userId, CancellationToken cancellationToken)
        {
            var balance = await TableNoTracking
                                .Where(p => p.UserId.Equals(userId))
                                .SumAsync(p => p.Amount, cancellationToken)
                                .ConfigureAwait(false);

            return balance;
        }
    }
}