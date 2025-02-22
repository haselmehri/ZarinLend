using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class UserInboxService : IUserInboxService, IScopedDependency
    {
        private readonly ILogger<UserInboxService> logger;
        private readonly IBaseRepository<UserInbox> userInboxRepository;
        private readonly IUserRepository userRepository;

        public UserInboxService(ILogger<UserInboxService> logger,
                                IBaseRepository<UserInbox> userInboxRepository,
                                IUserRepository userRepository)
        {
            this.logger = logger;
            this.userInboxRepository = userInboxRepository;
            this.userRepository = userRepository;
        }

        public virtual async Task SendMessage(List<Guid> userIds, string message, Guid senderId, CancellationToken cancellationToken = default)
        {
            if (!userIds.Any())
                throw new AppException("'userIds' cannot be NULL or empty!");

            //check userIds items is valid!
            var validUserIds = await userRepository.TableNoTracking
                .Where(p => userIds.Contains(p.Id))
                .Select(p => p.Id)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (!validUserIds.Any())
                throw new AppException("'userIds' cannot be NULL or empty!");
            else
            {
                foreach (var userId in validUserIds)
                {
                    await userInboxRepository.AddAsync(new UserInbox()
                    {
                        Message = message,
                        SenderId = senderId,
                        UserId = userId,
                    }, cancellationToken, saveNow: false);
                }

                await userInboxRepository.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
