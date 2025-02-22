using Common;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class UserWalletBalanceViewComponent : ViewComponent
    {
        private readonly IWalletTransactionService walletTransactionService;

        public UserWalletBalanceViewComponent(IWalletTransactionService walletTransactionService)
        {
            this.walletTransactionService = walletTransactionService;
        }

        public async Task<IViewComponentResult> InvokeAsync( CancellationToken cancellationToken)
        {
            return await Task.FromResult((IViewComponentResult)View("UserWalletBalance", await walletTransactionService.GetBalance(new Guid(User.Identity.GetUserId()), cancellationToken)));
        }
    }
}
