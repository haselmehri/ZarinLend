using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.Model;
using Services.Model.AccountStatement;
using Services.Model.ZarinPal;
using Stimulsoft.Report;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class ZarinpalService : IZarinpalService, IScopedDependency
    {
        private readonly string CLIENT_ID = "34";
        private readonly SiteSettings siteSettings;
        private readonly ILogger<ZarinpalService> logger;

        public ZarinpalService(ILogger<ZarinpalService> logger, IOptionsSnapshot<SiteSettings> siteSettings)
        {
            this.logger = logger;
            this.siteSettings = siteSettings.Value;
        }

        public async Task<string> GetAccessToken( string code,string codeVerifier)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            using (var client = new HttpClient(clientHandler))
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://connect.zarinpal.com/api/oauth/token");
                var content = new MultipartFormDataContent
                {
                    { new StringContent("authorization_code"), "grant_type" },
                    { new StringContent(CLIENT_ID), "client_id" },
                    { new StringContent(codeVerifier), "code_verifier" },
                    { new StringContent(code), "code" },
                    { new StringContent(siteSettings.ZarinPalCallBackUrl), "redirect_uri" }
                };
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                JObject resultObject = JObject.Parse(responseBody);
                return resultObject != null && resultObject.ContainsKey("access_token") ? resultObject[propertyName: "access_token"].ToString() : string.Empty;
            }
        }

        public async Task<ZarinpalUserData> GetUserData(string accessToken)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://next.zarinpal.com/api/v4/graphql/");
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
                var content = new StringContent("{\"query\":\"query {\\r\\n  Me {\\r\\n    ws_id\\r\\n    id\\r\\n    first_name\\r\\n    last_name\\r\\n    full_name\\r\\n    cell_number\\r\\n    gender\\r\\n    ssn\\r\\n    level\\r\\n    birthday\\r\\n    email\\r\\n    verifications\\r\\n    {\\r\\n      type\\r\\n      data\\r\\n      verify\\r\\n    }\\r\\n  }\\r\\n  Addresses {\\r\\n    title\\r\\n    type\\r\\n    address\\r\\n    tax_id\\r\\n    postal_code\\r\\n    landline\\r\\n    location\\r\\n    is_postal_code_verified\\r\\n  }\\r\\n  BankAccounts{\\r\\n    id\\r\\n    iban\\r\\n    status\\r\\n    type\\r\\n    is_legal\\r\\n    issuing_bank {\\r\\n      slug_image\\r\\n    }\\r\\n    expired_at\\r\\n    name\\r\\n    holder_name\\r\\n    created_at\\r\\n    updated_at\\r\\n    deleted_at    \\r\\n  }\\r\\n}\\r\\n\",\"variables\":{}}", null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    JObject resultObject = JObject.Parse(responseBody);
                    var result = JsonConvert.DeserializeObject<ZarinpalUserData>(responseBody);
                    //var result2 = JsonConvert.DeserializeObject<ZarinpalUserData>(resultObject.ToString());
                    return result;
                }

                return null;
            }
        }

        //public string MakeCodeVerifier()
        //{
        //    byte[] randomBytes = new byte[32];
        //    using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
        //    {
        //        rngCsp.GetBytes(randomBytes);
        //    }
        //    string base64String = Convert.ToBase64String(randomBytes);
        //    string codeVerifier = base64String.Replace("/", "_").Replace("+", "-").Replace("=", "");
        //    CookieManager.Set(HttpContext, CookieManager.CookieKeys.CodeVerifier, codeVerifier, SameSiteMode.Unspecified, false, false, false, DateTime.Now.AddHours(1));
        //    return codeVerifier;
        //}

        //maybe this method to be better than!!!
        public string MakeCodeVerifier()
        {
            byte[] randomBytes = new byte[32];
            RandomNumberGenerator.Create().GetBytes(randomBytes);

            string base64String = Convert.ToBase64String(randomBytes);
            string codeVerifier = base64String.Replace("/", "_").Replace("+", "-").Replace("=", "");
            return codeVerifier;
        }

        public string MakeCodeChallenge(string verifier)
        {
            byte[] hashedBytes;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] verifierBytes = Encoding.UTF8.GetBytes(verifier);
                hashedBytes = sha256.ComputeHash(verifierBytes);
            }
            string base64String = Convert.ToBase64String(hashedBytes);
            string codeChallenge = base64String.Replace("/", "_").Replace("+", "-").Replace("=", "");
            return codeChallenge;
        }

        public string GenerateAuthorizationUrl(string challengeCode)
        {
            string url = "https://connect.zarinpal.com/api/oauth/authorizeInit?";
            url += "scope=*&";
            url += "response_type=code&";
            url += $"client_id={CLIENT_ID}&";
            url += $"code_challenge={challengeCode}&";
            url += "code_challenge_method=S256&";
            url += "state=1001&";
            url += $"redirect_uri={siteSettings.ZarinPalCallBackUrl}";
            return url;
        }
    }
}
