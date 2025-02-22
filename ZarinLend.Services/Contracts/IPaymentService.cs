using Services.Dto;
using Services.Model;
using Services.Model.Payment;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IPaymentService
    {
        Task<PagingDto<IntenetBankPaymentModel>> GetPaymentList(Guid userId, PagingFilterDto filter, CancellationToken cancellationToken);
    }
}