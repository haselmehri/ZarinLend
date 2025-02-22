using Asp.Versioning;
using Common;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using Services.Dto;
using Services.Model.Invoice;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class InvoiceController : BaseApiController
    {
        private readonly IInvoiceService invoiceService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(IInvoiceService invoiceService, ILogger<InvoiceController> logger)
        {
            this.invoiceService = invoiceService;
            _logger = logger;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.ZarinLendExpert, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<PagingDto<InvoiceViewModel>> SearchInvoice(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            return await invoiceService.SearchInvoices(filter, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Seller, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<PagingDto<InvoiceViewModel>> ShopInvoices(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            filter.FilterList.Add(new Filter()
            {
                Operator = Operator.Equal,
                PropertyName = "OrganizationId",
                PropertyValue = User.Identity.GetSellerOrganizationId()
            });

            return await invoiceService.SearchInvoices(filter, cancellationToken);
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Seller, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> UploadInvoiceFile([FromForm] InvoiceImageUploadModel model, CancellationToken cancellationToken)
        {
            model.ShopOrganizationId = User.Identity.GetSellerOrganizationId();
            await invoiceService.UploadInvoiceFile(model,false, cancellationToken);
            return Ok();
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Seller, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> UploadInvoiceFileAndSendToVerify([FromForm] InvoiceImageUploadModel model, CancellationToken cancellationToken)
        {
            model.ShopOrganizationId = User.Identity.GetSellerOrganizationId();
            await invoiceService.UploadInvoiceFile(model,true, cancellationToken);
            return Ok();
        }        
    }
}
