using Common;
using Core.Data.Repositories;
using Core.Entities.Business.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Dto;
using Services.Model;
using Services.Model.Transaction;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class TransactionService : ITransactionService, IScopedDependency
    {
        private readonly ILogger<TransactionService> logger;
        private readonly ITransactionRepository transactionRepository;

        public TransactionService(ILogger<TransactionService> logger, ITransactionRepository transactionRepository)
        {
            this.logger = logger;
            this.transactionRepository = transactionRepository;
        }

        public async Task<PagingDto<TransactionModel>> GetTransactionList(Guid userId, PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var query = transactionRepository.TableNoTracking.Where(p => p.UserId.Equals(userId));

            var filterList = await query
                        .OrderByDescending(p => p.UpdateDate)
                        .ThenByDescending(p => p.CreatedDate)
                        .Skip((filter.Page - 1) * filter.PageSize)
                        .Take(filter.PageSize)
                .Select(p => new TransactionModel
                {
                    //Id = p.Id,
                    Amount = p.Amount,
                    TransactionType = p.TransactionType,
                    CreateDate = p.CreatedDate,
                    UpdateDate = p.UpdateDate,
                    Description = p.Description
                })
                .ToListAsync(cancellationToken);

            var totalRowCounts = await query.CountAsync();

            return new PagingDto<TransactionModel>()
            {
                CurrentPage = filter.Page,
                Data = filterList,
                TotalRowCount = totalRowCounts,
                TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCounts) / filter.PageSize)
            };
        }

        public async Task Withdrawal(long amount, Guid userId, Guid creatorId, int? requestFacilityId,string description= "برداشت هزینه اعتبار سنجی", int? requestFacilityGuarantorId = null, CancellationToken cancellationToken = default)
        {
            await transactionRepository.AddAsync(new Transaction()
            {
                Amount = amount,
                CreatorId = creatorId,
                UserId = userId,
                Description = description,               
                TransactionType = TransactionTypeEnum.Withdraw,
                TransactionReason = new TransactionReason()
                {
                    RequestFacilityId = requestFacilityId,
                    RequestFacilityGuarantorId = requestFacilityGuarantorId
                }
            }, cancellationToken);
        }


        /// <summary>
        /// اعتبار واقعی 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<long> GetBalance(Guid userId, CancellationToken cancellationToken)
        {
            return await transactionRepository.GetBalance(userId,cancellationToken);
        }
    }
}