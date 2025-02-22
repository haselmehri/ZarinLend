using Microsoft.Extensions.Options;
using PushNotificationInPWA.Models;
using static PushNotificationInPWA.Models.GoogleNotification;
using System.Net.Http.Headers;
using System.Runtime;
using CorePush.Firebase;
using FirebaseAdmin.Messaging;

namespace PushNotificationInPWA.Service
{
    public interface INotificationService
    {
        Task<ResponseModel> SendNotification(NotificationModel notificationModel);
    }

    public class NotificationService //: INotificationService
    {
        private readonly FcmNotificationSetting _fcmNotificationSetting;
        public NotificationService(IOptions<FcmNotificationSetting> settings)
        {
            _fcmNotificationSetting = settings.Value;
        }

        public async Task SendNotification(string token, string title, string body)
        {
            var message = new Message()
            {
                Token = token,                
                Notification = new Notification()
                {
                    Title = title,
                    Body = body,            
                }
            };

            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            Console.WriteLine("Successfully sent message: " + response);
        }
    }
}
