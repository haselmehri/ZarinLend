using FCMInPWA.Service;
using Microsoft.AspNetCore.Mvc;

namespace FCMInPWA.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class FcmController(IDbHelper dbHelper, FirebaseMessagingService firebaseMessagingService) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<JsonResult> SaveToken(TokenRequest model)
        {
            var result = await dbHelper.SaveToken(model.Token);
            //var subscription = new PushSubscription(endpoint, p256dh, auth);
            //PersistentStorage.SaveSubscription(client, subscription);
            return new JsonResult(result);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification(NotificationRequest request)
        {
            var response = await firebaseMessagingService.SendPushNotificationAsync(request.DeviceToken, request.Title, request.Body);
            return Ok(new { Message = "Notification sent successfully", Response = response });
        }
    }

    public class NotificationRequest
    {
        public string DeviceToken { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public class TokenRequest
    {
        public string Token { get; set; }
    }
}
