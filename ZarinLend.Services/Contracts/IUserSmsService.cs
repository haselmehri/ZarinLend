using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IUserSmsService
    {
        Task SendMessage(List<Guid> userIds, string message, Guid senderId, CancellationToken cancellationToken = default);
    }
}