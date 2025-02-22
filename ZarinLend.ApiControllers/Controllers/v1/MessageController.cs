using Asp.Versioning;
using Common;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class MessageController : BaseApiController
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IUserInboxService userInboxService;
        private readonly IUserSmsService userSmsService;
        private readonly IUserNotificationService userNotificationService;

        public MessageController(IWebHostEnvironment webHostEnvironment,
                                 IUserInboxService userInboxService,
                                 IUserSmsService userSmsService,
                                 IUserNotificationService userNotificationService)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.userInboxService = userInboxService;
            this.userSmsService = userSmsService;
            this.userNotificationService = userNotificationService;
        }


        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> SendMessage([FromForm] UserMessageModel model, CancellationToken cancellationToken)
        {
            model.SenderId = new Guid(User.Identity.GetUserId());
            if (model.SendToInbox)
                await userInboxService.SendMessage(model.UserIds, model.InboxMessageContent, model.SenderId, cancellationToken);

            if (model.SendSms)
                await userSmsService.SendMessage(model.UserIds, model.InboxMessageContent, model.SenderId, cancellationToken);

            if (model.SendNotification)
                await userNotificationService.SendNotification(model.UserIds,model.UserNotificationModel, model.SenderId, cancellationToken);

            return new ApiResult(true, ApiResultStatusCode.Success);
        }
    }
}
