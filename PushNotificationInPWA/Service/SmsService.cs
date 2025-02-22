using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushNotificationInPWA.Models;
using System.Net;

namespace Services
{
    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsService> logger;
        private readonly string API_KEY = "6D305649363039356248526A33327069666177313148796472365544667275684278546D495344437647453D";

        public SmsService(ILogger<SmsService> logger)
        {
            this.logger = logger;
        }

        public async Task<long> SendOtp(string mobile, string otp, string site, CancellationToken cancellationToken)
        {
            using (var client = new HttpClient())
            {
                var message = $"رمز بکبار مصرف : {otp} {Environment.NewLine} @{site} #{otp}";
                string requesturl =
                    $"https://api.kavenegar.com/v1/{API_KEY}/sms/send.json?receptor={mobile}&message={WebUtility.UrlEncode(message)}";

                HttpResponseMessage response = await client.GetAsync(requesturl, cancellationToken);
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject resultObject = JObject.Parse(responseBody);

                if (resultObject.ContainsKey("return") &&
                    JObject.Parse(resultObject["return"].ToString()).ContainsKey("status") &&
                    JObject.Parse(resultObject["return"].ToString())["status"].ToString().Equals("200", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (resultObject.ContainsKey("entries") &&
                        resultObject["entries"].GetType() == typeof(JArray) &&
                        (resultObject["entries"] as JArray).Count > 0)
                    {
                        var result = JsonConvert.DeserializeObject<SmsVerificationModel>((resultObject["entries"] as JArray)[0].ToString());

                        return result.MessageId;
                    }

                    throw new Exception(responseBody);
                }
                else
                {

                    throw new Exception($"error! , status :{resultObject["status"]} , message :{resultObject["message"]} ");
                    if (resultObject.ContainsKey("return") && resultObject.ContainsKey("status") && resultObject.ContainsKey("message"))
                        logger.LogWarning($"error! , status :{resultObject["status"]} , message :{resultObject["message"]} ");

                }
            }

        }
    }
}
