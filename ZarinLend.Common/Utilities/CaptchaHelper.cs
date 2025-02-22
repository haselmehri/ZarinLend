using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Common.Utilities
{
    public static class CaptchaHelper
    {
        public static async Task<bool> GoogleCaptchaValidation(string secret, string token, CancellationToken cancellationToken)
        {
            using var client = new HttpClient();
            var dict = new Dictionary<string, string> { { "secret", secret }, { "response", token } };

            HttpResponseMessage response;
            using (var req = new HttpRequestMessage(HttpMethod.Post, "https://www.google.com/recaptcha/api/siteverify") { Content = new FormUrlEncodedContent(dict) })
            {
                response = await client.SendAsync(req, cancellationToken);
            }

            var responseString = await response.Content.ReadAsStringAsync();

            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseString);

            return jsonResponse.success == true && jsonResponse.score > 0.6;
        }

    }
}
