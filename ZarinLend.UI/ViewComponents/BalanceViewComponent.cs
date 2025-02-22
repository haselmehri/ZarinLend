using Common;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class BalanceViewComponent : ViewComponent
    {
        private readonly IWalletTransactionService walletTransactionService;

        public BalanceViewComponent(IWalletTransactionService walletTransactionService)
        {
            this.walletTransactionService = walletTransactionService;
        }

        public async Task<IViewComponentResult> InvokeAsync(CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity.GetUserId());
            var balance = await walletTransactionService.GetBalance(userId, cancellationToken);
            return await Task.FromResult((IViewComponentResult)View("UserBalance",
                new UserBalance()
                {
                    Balance = balance
                }));
        }
    }
}
