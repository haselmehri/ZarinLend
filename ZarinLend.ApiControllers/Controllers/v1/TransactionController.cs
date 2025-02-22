using Asp.Versioning;
using Common;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Dto;
using Services.Model.Transaction;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class TransactionController : BaseApiController
    {
        private readonly ITransactionService transactionService;

        public TransactionController(ITransactionService transactionService )
        {
            this.transactionService = transactionService;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<PagingDto<TransactionModel>> GetTransactionList(PagingFilterDto filter, CancellationToken cancellationToken)
        {
            return await transactionService.GetTransactionList(new Guid(User.Identity.GetUserId()), filter, cancellationToken);
        }
    }
}
