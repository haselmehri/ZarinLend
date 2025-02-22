using Services.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IUserNotificationService
    {
        Task SendNotification(List<Guid> userIds, UserNotificationModel notificationModel, Guid senderId, CancellationToken cancellationToken = default);
    }
}