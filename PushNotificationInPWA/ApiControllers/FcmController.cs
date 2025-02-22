using Microsoft.AspNetCore.Mvc;
using PushNotificationInPWA.Service;

namespace PushNotificationInPWA.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FcmController(IDbHelper dbHelper) : ControllerBase
    {
        [HttpPost("[action]/{token}")]
        public async Task<JsonResult> SaveToken(string token)
        {
            var result = await dbHelper.SaveToken(token);
            //var subscription = new PushSubscription(endpoint, p256dh, auth);
            //PersistentStorage.SaveSubscription(client, subscription);
            return new JsonResult(result);
        }
    }
}