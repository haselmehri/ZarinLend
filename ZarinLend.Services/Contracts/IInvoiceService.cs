using Services.Dto;
using Services.Model.Invoice;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IInvoiceService
    {
        Task<PagingDto<InvoiceViewModel>> SearchInvoices(PagingFilterDto filter, CancellationToken cancellationToken);
        Task UploadInvoiceFile(InvoiceImageUploadModel model, bool sendForVerify, CancellationToken cancellationToken);
    }
}