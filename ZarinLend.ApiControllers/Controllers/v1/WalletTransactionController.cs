using Asp.Versioning;
using AutoMapper;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class WalletTransactionController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IWalletTransactionService walletTransactionService;

        public WalletTransactionController(IMapper mapper, IWalletTransactionService walletTransactionService)
        {
            _mapper = mapper;
            this.walletTransactionService = walletTransactionService;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin)]
        public virtual async Task<long> AddWalletCharge(Guid agentId, CancellationToken cancellationToken)
        {
            var balance = await walletTransactionService.GetBalance(agentId, cancellationToken);
            return balance;
        }
    }
}
