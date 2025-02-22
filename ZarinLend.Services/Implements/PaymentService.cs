using Common;
using Core.Data.Repositories;
using Core.Entities.Business.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Dto;
using Services.Model.Payment;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class PaymentService : IPaymentService, IScopedDependency
    {

        private readonly ILogger<PaymentService> logger;
        private readonly IBaseRepository<SamanInternetPayment> samanInternetPaymentRepository;
        private readonly IBaseRepository<ZarinPalInternetPayment> zarinpalInternetPaymentRepository;

        public PaymentService(ILogger<PaymentService> logger,
                              IBaseRepository<SamanInternetPayment> samanInternetPaymentRepository,
                              IBaseRepository<ZarinPalInternetPayment> zarinpalInternetPaymentRepository)
        {
            this.logger = logger;
            this.samanInternetPaymentRepository = samanInternetPaymentRepository;
            this.zarinpalInternetPaymentRepository = zarinpalInternetPaymentRepository;
        }

        public async Task<PagingDto<IntenetBankPaymentModel>> GetPaymentList(Guid userId, PagingFilterDto filter, CancellationToken cancellationToken)
        {
            var samanIpgQuery = samanInternetPaymentRepository.TableNoTracking
                .Where(p => p.UserId.Equals(userId))
                .Select(p => new
                {
                    p.Amount,
                    p.PaymentType,
                    CreateDate = p.CreatedDate,
                    p.UpdateDate,
                    p.IsSuccess,
                    MaskedPan = p.TransactionDetail_MaskedPan,
                    p.RefNum,
                    TraceNum = p.TraceNo,
                    Rrn = p.RRN,
                    p.UserId,
                    IpgType = IpgType.SamanIPG
                });
            var zarinPalIpgQuery = zarinpalInternetPaymentRepository.TableNoTracking
                .Where(p => p.UserId.Equals(userId))
                .Select(p => new
                {
                    p.Amount,
                    p.PaymentType,
                    CreateDate = p.CreatedDate,
                    p.UpdateDate,
                    p.IsSuccess,
                    MaskedPan = p.Card_Pan,
                    RefNum = p.Ref_Id,
                    TraceNum = p.Authority,
                    Rrn = string.Empty,
                    p.UserId,
                    IpgType = IpgType.ZarinpalIPG
                });

            var filterList = await samanIpgQuery.Union(zarinPalIpgQuery)
                        .OrderByDescending(p => p.UpdateDate)
                        .ThenByDescending(p => p.CreateDate)
                        .Skip((filter.Page - 1) * filter.PageSize)
                        .Take(filter.PageSize)
                .Select(p => new IntenetBankPaymentModel
                {
                    //Id = p.Id,
                    Amount = p.Amount,
                    PaymentType = p.PaymentType,
                    CreateDate = p.CreateDate,
                    UpdateDate = p.UpdateDate,
                    IsSuccess = p.IsSuccess,
                    MaskedPan = p.MaskedPan,
                    RefNum = p.RefNum,
                    TraceNum = p.TraceNum,
                    Rrn = p.Rrn,
                    IpgType = p.IpgType,
                    UserId = p.UserId,
                })
                .ToListAsync(cancellationToken);

            var totalRowCounts = await samanIpgQuery.Union(zarinPalIpgQuery).CountAsync();

            return new PagingDto<IntenetBankPaymentModel>()
            {
                CurrentPage = filter.Page,
                Data = filterList,
                TotalRowCount = totalRowCounts,
                TotalPages = (int)Math.Ceiling(Convert.ToDouble(totalRowCounts) / filter.PageSize)
            };
        }
    }
}
