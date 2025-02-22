using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using HttpClientToCurl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZarinLend.Services.Model.AyandehSign;

namespace Services
{
    public class AyandehSignService : IAyandehSignService, IScopedDependency
    {
        public enum AyandehSignContentStatus
        {
            Unknown = 0,
            NotExist = 1,
            NotSigned = 2,
            Expired = 3,
            Signed = 4,
            Delivered = 5
        }
        private const string USER_NAME = "Zarin";
        private const string PASSWORD = "ZarinIsg@1402";
        private const string Base_URL = "https://digisign.ba24.ir";
        private readonly SiteSettings siteSettings;
        private readonly IAyandehSignRequestSignatureLogRepository ayandehSignRequestSignatureLogRepository;
        private readonly ILogger<AyandehSignService> logger;

        public AyandehSignService(IOptionsSnapshot<SiteSettings> siteSettings, IAyandehSignRequestSignatureLogRepository ayandehSignRequestSignatureLogRepository, ILogger<AyandehSignService> logger)
        {
            this.siteSettings = siteSettings.Value;
            this.ayandehSignRequestSignatureLogRepository = ayandehSignRequestSignatureLogRepository;
            this.logger = logger;
        }

        public async Task<GetSigningTokenResult> GetSigningToken(int requestFacilityId, Guid userId, string pdfToSign, string nationalCode, string mobile,
            string title, string hint,string callBackUrl, CancellationToken cancellationToken)
        {
            var token = await GetToken();
            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            if (token == null) throw new AppException("return result from 'token()' is null!");
            try
            {
                string requestUrl = $"{Base_URL}/rest/api/v2/Signing/GetSigningToken";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
                {
                    //var trackId = Guid.NewGuid().ToString();;
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var body = JsonConvert.SerializeObject(
                        new
                        {
                            dataToSign = pdfToSign,
                            direction = "unset",
                            contentType = 2,
                            signType = 1,
                            callbackType = 2,
                            callbackUrl = callBackUrl,
                            hint = hint,
                            title = title,
                            letSignerDownload = true,
                            signerSubjectDn = $"SERIALNUMBER={nationalCode}",
                            handwriteSignature = 0,
                            metadata = $"{requestFacilityId}",
                            notification = new
                            {
                                token = mobile,
                                type = 2
                            },
                            isEncrypted = false
                            //templateName = null,
                            //validityDuration=null,
                            //issuersDn= null,
                            //contentImage= null,
                        });

                    logger.LogInformation($"GetSigningToken Body : {body}");
                    logger.LogInformation($"GetSigningToken Body:callBackUrl : {callBackUrl}");
                    HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");
                    request.Content = content;

                    var curl = client.GenerateCurlInString(request);
                    logId = await ayandehSignRequestSignatureLogRepository
                        .AddLog(new AyandehSignRequestSignatureLog()
                        {
                            Body = body,
                            Url = requestUrl,
                            ServiceName = AyandehSignServiceName.GetSigningToken.ToString(),
                            MethodType = MethodType.Post,
                            CreatorId = userId,
                            RequestFacilityId = requestFacilityId,
                            Curl = curl
                        });
                    
                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await ayandehSignRequestSignatureLogRepository.UpdateLog(logId, responseBody, cancellationToken);
                        logUpdateSuccessed = true;
                    }

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        throw new AppException(response.StatusCode.ToString());

                    logger.LogInformation($"GetSigningToken Response : {responseBody}");

                    JObject resultObject = JObject.Parse(responseBody);
                    if (resultObject != null)
                    {
                        if (resultObject.ContainsKey("signingToken") &&
                            resultObject["signingToken"] != null &&
                            !string.IsNullOrEmpty(Convert.ToString(resultObject["signingToken"])) &&
                            Convert.ToBoolean(resultObject["succeeded"]) &&
                            resultObject.ContainsKey("NotificationOutput") &&
                            ((JObject)resultObject["NotificationOutput"]!).ContainsKey("issuccess") &&
                            Convert.ToBoolean(resultObject["NotificationOutput"]!["issuccess"]))
                            return new GetSigningTokenResult()
                            {
                                SigningToken = resultObject["signingToken"]!.ToString(),
                                IsSuccess = true
                            };
                        else
                        {
                            var errorMessage = resultObject["errorMessage"] != null && resultObject["errorMessage"]!.ToString() != string.Empty ? resultObject["errorMessage"]!.ToString() : string.Empty;
                            errorMessage += resultObject.ContainsKey("NotificationOutput") && resultObject["NotificationOutput"]!["errorMessage"] != null && resultObject["NotificationOutput"]!["errorMessage"]!.ToString() != string.Empty
                                ? resultObject["NotificationOutput"]!["errorMessage"]!.ToString() 
                                :string.Empty;
                            return new GetSigningTokenResult()
                            {
                                IsSuccess = false,
                                ErrorMessage = errorMessage,
                            };
                        }
                    }

                    return new GetSigningTokenResult()
                    {
                        IsSuccess = false,
                        ErrorMessage = "خطا در ارسال نوتیفیکیشن برای امضاء"
                    };

                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await ayandehSignRequestSignatureLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}", cancellationToken);

                //logger.LogError(exp, exp.Message);
                return new GetSigningTokenResult()
                {
                    IsSuccess = false,
                    ErrorMessage = "خطا در ارسال نوتیفیکیشن برای امضآء"
                };
            }
        }

        public async Task<AyandehSignGetDataModel> GetData(string signingToken, CancellationToken cancellationToken)
        {
            var token = await GetToken();
            if (token == null) throw new AppException("return result from 'token()' is null!");
            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                using (var client = new HttpClient())
                {
                    var trackId = Guid.NewGuid().ToString();
                    string requesturl = $"{Base_URL}/rest/api/v2/Signing/GetData";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var body = JsonConvert.SerializeObject(
                        new
                        {
                            signingToken
                        });

                    logger.LogInformation($"GetData Body : {body}");
                    HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");

                    logId = await ayandehSignRequestSignatureLogRepository
                        .AddLog(new AyandehSignRequestSignatureLog()
                        {
                            Body = body,
                            Url = requesturl,
                            ServiceName = AyandehSignServiceName.GetData.ToString(),
                            MethodType = MethodType.Post,
                            CreatorId = null
                        });
                    HttpResponseMessage response = await client.PostAsync(requesturl, content, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await ayandehSignRequestSignatureLogRepository.UpdateLog(logId, responseBody, cancellationToken);
                        logUpdateSuccessed = true;
                    }

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        throw new AppException(response.StatusCode.ToString());

                    logger.LogInformation($"GetData Response : {responseBody}");

                    JObject resultObject = JObject.Parse(responseBody);
                    if (resultObject != null)
                    {
                        if (resultObject.ContainsKey("succeeded") && Convert.ToBoolean(resultObject["succeeded"]))
                        {
                            return JsonConvert.DeserializeObject<AyandehSignGetDataModel>(resultObject.ToString());
                        }
                        else if (resultObject.ContainsKey("errorMessage") && !string.IsNullOrEmpty(Convert.ToString(resultObject["errorMessage"])))
                        {
                            logger.LogError($"GetData, errorMessage :{resultObject["errorMessage"]} | ! Body : {body} | Response : {responseBody}");
                            return null;
                        }
                    }

                    logger.LogError($"GetData unexpected error! Body : {body} | Response : {responseBody}");
                    return null;
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await ayandehSignRequestSignatureLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}", cancellationToken);

                logger.LogError(exp, exp.Message);
                return null;
            }
        }
        public async Task<AyandehSignContentStatus?> CheckStatus(string signingToken, CancellationToken cancellationToken)
        {
            var token = await GetToken();
            if (token == null) throw new AppException("return result from 'token()' is null!");
            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                using (var client = new HttpClient())
                {
                    var trackId = Guid.NewGuid().ToString();
                    string requesturl = $"{Base_URL}/rest/api/v2/Signing/CheckStatus";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var body = JsonConvert.SerializeObject(
                        new
                        {
                            signingToken
                        });

                    logger.LogInformation($"CheckStatus Body : {body}");
                    HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");

                    logId = await ayandehSignRequestSignatureLogRepository
                      .AddLog(new AyandehSignRequestSignatureLog()
                      {
                          Body = body,
                          Url = requesturl,
                          ServiceName = AyandehSignServiceName.CheckStatus.ToString(),
                          MethodType = MethodType.Post,
                          CreatorId = null
                      });

                    HttpResponseMessage response = await client.PostAsync(requesturl, content, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await ayandehSignRequestSignatureLogRepository.UpdateLog(logId, responseBody, cancellationToken);
                        logUpdateSuccessed = true;
                    }

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        throw new AppException(response.StatusCode.ToString());

                    logger.LogInformation($"CheckStatus Response : {responseBody}");

                    int ayandehSignContentStatusValue;
                    if (responseBody != null && int.TryParse(responseBody, out ayandehSignContentStatusValue))
                    {
                        return (AyandehSignContentStatus)Enum.ToObject(typeof(AyandehSignContentStatus), ayandehSignContentStatusValue);
                    }
                    else
                    {
                        logger.LogError($"CheckStatus unexpected error! Body : {body} | Response : {responseBody}");
                        return null;
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await ayandehSignRequestSignatureLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}", cancellationToken);

                logger.LogError(exp, exp.Message);
                return null;
            }
        }
        private async Task<string?> GetToken()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string requesturl = $"{Base_URL}/rest/api/v2/auth/login";

                    var body = JsonConvert.SerializeObject(new { userName = USER_NAME, password = PASSWORD });

                    HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(requesturl, content);

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        throw new AppException(response.StatusCode.ToString());

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject resultObject = JObject.Parse(responseBody);
                    logger.LogInformation(responseBody);
                    if (resultObject != null)
                    {
                        if (resultObject.ContainsKey("accessToken") && !string.IsNullOrEmpty(Convert.ToString(resultObject["accessToken"])))
                            return resultObject["accessToken"]!.ToString();
                        else if (resultObject.ContainsKey("errorMessage") && !string.IsNullOrEmpty(Convert.ToString(resultObject["errorMessage"])))
                            throw new AppException(resultObject["errorMessage"]!.ToString());
                    }
                    return null;
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
            }
            return null;
        }
    }
}