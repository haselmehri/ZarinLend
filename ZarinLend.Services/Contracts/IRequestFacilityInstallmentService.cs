using Services.Dto;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IRequestFacilityInstallmentService
    {
        Task<List<RequestFacilityInstallmentModel>> SelectInstallment(Guid userId, int requestFacilityId, CancellationToken cancellationToken);
        Task<RequestFacilityInstallmentModel> GetById(int id, CancellationToken cancellationToken);
        Task<RequestFacilityInstallmentModel> PrepareForPayment(int id, CancellationToken cancellationToken);
        Task<RequestFacilityInstallmentModel> CheckExistUnpaidInstallmentBeforeThis(Guid userId, int id, CancellationToken cancellationToken);
        Task<PagingDto<RequestFacilityInstallmentModel>> Search(PagingFilterDto filter, bool executeForExport, CancellationToken cancellationToken);
    }
}