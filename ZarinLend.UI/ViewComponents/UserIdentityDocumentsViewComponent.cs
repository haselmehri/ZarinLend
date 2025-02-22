using Common;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Web.UI.ViewComponents
{
    public class UserIdentityDocumentsViewComponent : ViewComponent
    {
        private readonly IUserService userService;

        public UserIdentityDocumentsViewComponent(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<DocumentModel> userIdentitydocuments, CancellationToken cancellationToken)
        {
            return await Task.FromResult((IViewComponentResult)View("UserIdentityDocuments", userIdentitydocuments));
        }
    }
}
