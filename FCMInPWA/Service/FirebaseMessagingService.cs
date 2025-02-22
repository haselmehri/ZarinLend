using FirebaseAdmin.Messaging;

namespace FCMInPWA.Service
{
    public class FirebaseMessagingService
    {
        public async Task<string> SendPushNotificationAsync(string deviceToken, string title, string body)
        {
            var message = new Message()
            {
                Token = deviceToken,
                Notification = new Notification
                {
                    Title = title,
                    Body = body,
                    //ImageUrl = 
                }
            };

            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return response;
            }
            catch (FirebaseMessagingException ex)
            {
                // Handle error here (log, return error response, etc.)
                return $"Error: {ex.Message}";
            }
        }
    }
}
