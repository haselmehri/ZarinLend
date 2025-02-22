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
    public class UserSmsService : IUserSmsService, IScopedDependency
    {
        private readonly ILogger<UserSmsService> logger;
        private readonly IBaseRepository<UserSms> userSmsRepository;
        private readonly IUserRepository userRepository;
        private readonly ISmsService smsService;

        public UserSmsService(ILogger<UserSmsService> logger,
                                IBaseRepository<UserSms> userSmsRepository,
                                IUserRepository userRepository,
                                ISmsService smsService)
        {
            this.logger = logger;
            this.userSmsRepository = userSmsRepository;
            this.userRepository = userRepository;
            this.smsService = smsService;
        }

        public virtual async Task SendMessage(List<Guid> userIds, string message, Guid senderId, CancellationToken cancellationToken = default)
        {
            if (!userIds.Any())
                throw new AppException("'userIds' cannot be NULL or empty!");

            //check userIds items is valid!
            var validUserIds = await userRepository.TableNoTracking
                .Where(p => userIds.Contains(p.Id))
                .Select(p => new { p.Id, p.Person.Mobile })
                .Distinct()
                .ToListAsync(cancellationToken);

            if (!validUserIds.Any())
                throw new AppException("'userIds' cannot be NULL or empty!");
            else
            {
                foreach (var userInfo in validUserIds)
                {
                    //smsService.SendOtp();
                    await userSmsRepository.AddAsync(new UserSms()
                    {
                        Message = message,
                        Mobile = userInfo.Mobile,
                        SenderId = senderId,
                        UserId = userInfo.Id,
                        
                    }, cancellationToken, saveNow: false);
                }

                await userSmsRepository.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
