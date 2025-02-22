using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Services.Model;
using Stimulsoft.System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using WebPush;

namespace Services
{
    public class UserNotificationService : IUserNotificationService, IScopedDependency
    {
        private readonly ILogger<UserNotificationService> logger;
        private readonly IBaseRepository<UserNotification> userNotificationRepository;
        private readonly IUserRepository userRepository;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly SiteSettings siteSettings;

        public UserNotificationService(ILogger<UserNotificationService> logger,
                                       IBaseRepository<UserNotification> userNotificationRepository,
                                       IUserRepository userRepository,
                                       IOptions<SiteSettings> siteSettings,
                                       IWebHostEnvironment webHostEnvironment)
        {
            this.logger = logger;
            this.userNotificationRepository = userNotificationRepository;
            this.userRepository = userRepository;
            this.webHostEnvironment = webHostEnvironment;
            this.siteSettings = siteSettings.Value;
        }

        public virtual async Task SendNotification(List<Guid> userIds, UserNotificationModel notificationModel, Guid senderId, CancellationToken cancellationToken = default)
        {
            if (!userIds.Any())
                throw new AppException("'userIds' cannot be NULL or empty!");

            //check userIds items is valid!
            var validUserVAPIDs = await userRepository.TableNoTracking
                .Where(p => userIds.Contains(p.Id))
                .SelectMany(p => p.UserVAPIDs.Select(x => new { x.Id, x.P256dh, x.Auth, x.Endpoint, x.UserId }))
                .Distinct()
                .ToListAsync(cancellationToken);

            if (!validUserVAPIDs.Any())
                throw new AppException("'userIds' cannot be NULL or empty!");
            else
            {
                foreach (var vapId in validUserVAPIDs)
                {
                    try
                    {
                        string relativeImageUrl = null;
                        if (notificationModel.ImageFile != null)
                        {
                            var userNotificationImagePath = @"UploadFiles\UserNotificationImage";
                            string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, userNotificationImagePath);
                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(notificationModel.ImageFile.FileName)}";
                            relativeImageUrl = $"/UploadFiles/UserNotificationImage/{fileName}";
                            string filePath = Path.Combine(uploadFolder, fileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await notificationModel.ImageFile.CopyToAsync(fileStream);
                            }
                        }

                        PushSubscription subscription = new PushSubscription()
                        {
                            Auth = vapId.Auth,
                            Endpoint = vapId.Endpoint,
                            P256DH = vapId.P256dh
                        };
                        var vapidDetails = new VapidDetails(siteSettings.VAPID.subject, siteSettings.VAPID.publicKey, siteSettings.VAPID.privateKey);

                        var payload = JsonConvert.SerializeObject(
                            new
                            {
                                Title = notificationModel.Title,
                                Body = notificationModel.Message,
                                IconUrl = notificationModel.IconUrl,
                                ImageUrl = relativeImageUrl,
                                Url = notificationModel.Url
                            });

                        using (var webPushClient = new WebPushClient())
                        {
                            await webPushClient.SendNotificationAsync(subscription, payload, vapidDetails);
                        }

                        await userNotificationRepository.AddAsync(new UserNotification()
                        {
                            UserVapidId = vapId.Id,
                            Url = notificationModel.Url,
                            IconUrl = notificationModel.IconUrl,
                            ImageUrl = relativeImageUrl,
                            Message = notificationModel.Message,
                            Title = notificationModel.Title,
                            SenderId = senderId,
                        }, cancellationToken, saveNow: true);
                    }
                    catch (Exception exp)
                    {
                        logger.LogError(exp, exp.Message);
                    }
                }

                //await userNotificationRepository.SaveChangesAsync(cancellationToken);
            }
        }
    }
}