using Common;
using Common.CustomAttribute;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using HttpClientToCurl;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.Model.NeginHub;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZarinLend.Services.Model.Ayandeh.BankAccount;
using ZarinLend.Services.Model.NeginHub;

namespace Services
{
    public class NeginHubService : INeginHubService, IScopedDependency
    {
        private const string USER_NAME = "Payandehmehrdad@gmail.com";
        private const string PASSWORD = "eMdu4drgcr6AXtB*";
        //private const string PASSWORD = "3St721!Q";
        //private const string USER_NAME = "haseli2684@gmail.com";
        //private const string PASSWORD = "XkfY1Bqf8uqJ^Ejm";
        private const string BASE_URL = "https://api.neginhub.com";
        private readonly ILogger<NeginHubService> logger;
        private readonly INeginHubLogRepository neginHubLogRepository;
        private readonly IWebHostEnvironment webHostEnvironment;

        public NeginHubService(ILogger<NeginHubService> logger,
            INeginHubLogRepository neginHubLogRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            this.logger = logger;
            this.neginHubLogRepository = neginHubLogRepository;
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool?> CheckShahkar(CheckShahkarInputModel model, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetToken(creator);
            if (token == null) throw new AppException("return result from 'token' is null!");
            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                string requestUrl = $"{BASE_URL}/api/v4/KYC/CheckShahkar";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
                {
                    var trackId = Guid.NewGuid().ToString();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var body = JsonConvert.SerializeObject(
                        new
                        {
                            nationalCode = model.NationalCode,
                            mobile = model.Mobile,
                            trackId = trackId
                        });
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                    var curl = client.GenerateCurlInString(request);
                    logId = await neginHubLogRepository
                       .AddLog(new NeginHubLog()
                       {
                           Body = body,
                           Url = requestUrl,
                           ServiceName = NeginHubServiceName.CheckShahkar.ToString(),
                           TrackId = trackId,
                           MethodType = MethodType.Post,
                           OpratorId = creator,
                           Curl = curl
                       });
                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await neginHubLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("data") &&
                        resultObject["data"] != null &&
                        ((JObject)resultObject["data"]!).ContainsKey("isMatch") &&
                        resultObject.ContainsKey("meta") &&
                        ((JObject)resultObject["meta"]!).ContainsKey("isSuccess") &&
                        Convert.ToBoolean(((JObject)resultObject["meta"]!)["isSuccess"]))
                    {
                        return Convert.ToBoolean(resultObject["data"]!["isMatch"]!.ToString());
                    }
                    else
                    {
                        if (resultObject.ContainsKey("meta") &&
                           (((JObject)resultObject["meta"]!).ContainsKey("errorType") ||
                           ((JObject)resultObject["meta"]!).ContainsKey("errorMessage") ||
                           ((JObject)resultObject["meta"]!).ContainsKey("code")))
                        {
                            var error = string.Empty;
                            if (((JObject)resultObject["meta"]!).ContainsKey("error_type"))
                                error = $"error_type : ${((JObject)resultObject["meta"]!)["error_type"]}";

                            if (((JObject)resultObject["meta"]!).ContainsKey("error_message"))
                                error += $"-error_message : ${((JObject)resultObject["meta"]!)["error_message"]}";

                            if (((JObject)resultObject["meta"]!).ContainsKey("code"))
                                error += $"-code : ${((JObject)resultObject["meta"]!)["code"]}";

                            throw new AppException(error);
                        }
                        else
                            logger.LogError($"'CheckShahkar' Method : NationalCode : {model.NationalCode}, Mobile : {model.Mobile}. unexpected error!");

                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await neginHubLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError(exp, exp.Message);
                logger.LogError($"responseBody : {responseBody}");
                return null;
            }
        }
        public async Task<GetCivilRegistryDataResultModel?> GetCivilRegistryDataIncludePersonPhotoV4(GetCivilRegistryDataInputModel model, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetToken(creator);
            if (token == null) throw new AppException("return result from 'token' is null!");
            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                string requestUrl = $"{BASE_URL}/api/v4/KYC/GetCivilRegistryDataIncludePersonPhoto";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
                {
                    var trackId = Guid.NewGuid().ToString();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var body = JsonConvert.SerializeObject(
                        new
                        {
                            nationalCode = model.NationalCode,
                            birthDate = model.ShamsiBirthDate,
                            trackId = trackId
                        });
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                    var curl = client.GenerateCurlInString(request);
                    logId = await neginHubLogRepository
                       .AddLog(new NeginHubLog()
                       {
                           Body = body,
                           Url = requestUrl,
                           ServiceName = NeginHubServiceName.GetCivilRegistryDataIncludePersonPhotoV4.ToString(),
                           TrackId = trackId,
                           MethodType = MethodType.Post,
                           OpratorId = creator,
                           Curl = curl
                       });
                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await neginHubLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("data") &&
                        resultObject["data"] != null &&
                        resultObject.ContainsKey("meta") &&
                        ((JObject)resultObject["meta"]!).ContainsKey("isSuccess") &&
                        Convert.ToBoolean(((JObject)resultObject["meta"]!)["isSuccess"]))
                    {
                        return JsonConvert.DeserializeObject<GetCivilRegistryDataResultModel>(resultObject["data"]!.ToString())!;
                    }
                    else
                    {
                        if (resultObject.ContainsKey("meta") &&
                           (((JObject)resultObject["meta"]!).ContainsKey("errorType") ||
                           ((JObject)resultObject["meta"]!).ContainsKey("errorMessage") ||
                           ((JObject)resultObject["meta"]!).ContainsKey("code")))
                        {
                            var error = string.Empty;

                            if (((JObject)resultObject["meta"]!).ContainsKey("errorMessage"))
                                error += $"-errorMessage : ${((JObject)resultObject["meta"]!)["errorMessage"]}";

                            if (((JObject)resultObject["meta"]!).ContainsKey("errors") && ((JObject)resultObject["meta"]!)["errors"] != null)
                            {
                                foreach (JArray item in (JArray)((JObject)resultObject["meta"]!)["errors"]!)
                                    error += $"-{item["message"]}";
                            }

                            logger.LogError($"'GetCivilRegistryData' Method : NationalCode : {model.NationalCode}, BirthDate : {model.ShamsiBirthDate}. error : {resultObject["meta"]}!");
                            return new GetCivilRegistryDataResultModel()
                            {
                                IsSuccess = false,
                                ErrorMessage = error
                            };
                        }
                        else
                            logger.LogError($"'GetCivilRegistryData' Method : NationalCode : {model.NationalCode}, BirthDate : {model.ShamsiBirthDate}. unexpected error!");

                        return new GetCivilRegistryDataResultModel()
                        {
                            IsSuccess = false
                        };
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await neginHubLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError(exp, exp.Message);
                logger.LogError($"responseBody : {responseBody}");
                return new GetCivilRegistryDataResultModel()
                {
                    IsSuccess = false,
                    ErrorMessage = "عدم پاسخگویی سرویس ثبت احوال!"
                };
            }
        }
        public async Task<GetCivilRegistryDataResultModel?> GetCivilRegistryDataV4(GetCivilRegistryDataInputModel model, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetToken(creator);
            if (token == null) throw new AppException("return result from 'token' is null!");
            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                string requestUrl = $"{BASE_URL}/api/v4/KYC/GetCivilRegistryData";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
                {
                    var trackId = Guid.NewGuid().ToString();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var body = JsonConvert.SerializeObject(
                        new
                        {
                            nationalCode = model.NationalCode,
                            birthDate = model.ShamsiBirthDate.Replace("/", string.Empty),
                            trackId = trackId
                        });
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                    var curl = client.GenerateCurlInString(request);
                    logId = await neginHubLogRepository
                       .AddLog(new NeginHubLog()
                       {
                           Body = body,
                           Url = requestUrl,
                           ServiceName = NeginHubServiceName.GetCivilRegistryDataV4.ToString(),
                           TrackId = trackId,
                           MethodType = MethodType.Post,
                           OpratorId = creator,
                           Curl = curl
                       });
                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await neginHubLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("data") &&
                        resultObject["data"] != null &&
                        resultObject.ContainsKey("meta") &&
                        ((JObject)resultObject["meta"]!).ContainsKey("isSuccess") &&
                        Convert.ToBoolean(((JObject)resultObject["meta"]!)["isSuccess"]))
                    {
                        return JsonConvert.DeserializeObject<GetCivilRegistryDataResultModel>(resultObject["data"]!.ToString());
                    }
                    else
                    {
                        if (resultObject.ContainsKey("meta") &&
                           (((JObject)resultObject["meta"]!).ContainsKey("errorType") ||
                           ((JObject)resultObject["meta"]!).ContainsKey("errorMessage") ||
                           ((JObject)resultObject["meta"]!).ContainsKey("code")))
                        {
                            var error = string.Empty;

                            if (((JObject)resultObject["meta"]!).ContainsKey("errorMessage"))
                                error += $"-errorMessage : ${((JObject)resultObject["meta"]!)["errorMessage"]}";

                            if (((JObject)resultObject["meta"]!).ContainsKey("errors") && ((JObject)resultObject["meta"]!)["errors"] != null)
                            {
                                foreach (JArray item in (JArray)((JObject)resultObject["meta"]!)["errors"]!)
                                    error += $"-{item["message"]}";
                            }

                            logger.LogError($"'GetCivilRegistryData' Method : NationalCode : {model.NationalCode}, BirthDate : {model.ShamsiBirthDate}. error : {resultObject["meta"]}!");
                            return new GetCivilRegistryDataResultModel()
                            {
                                IsSuccess = false,
                                ErrorMessage = error
                            };
                        }
                        else
                            logger.LogError($"'GetCivilRegistryData' Method : NationalCode : {model.NationalCode}, BirthDate : {model.ShamsiBirthDate}. unexpected error!");

                        return new GetCivilRegistryDataResultModel()
                        {
                            IsSuccess = false
                        };
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await neginHubLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError(exp, exp.Message);
                logger.LogError($"responseBody : {responseBody}");
                return new GetCivilRegistryDataResultModel()
                {
                    IsSuccess = false,
                    ErrorMessage = "عدم پاسخگویی سرویس ثبت احوال!"
                };
            }
        }
        public async Task<GetCivilRegistryDataResultModel?> GetCivilRegistryDataV1(GetCivilRegistryDataInputModel model, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetToken(creator);
            if (token == null) throw new AppException("return result from 'token' is null!");
            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                string requestUrl = $"{BASE_URL}/api/v1/KYC/GetCivilRegistryData";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
                {
                    var trackId = Guid.NewGuid().ToString();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var body = JsonConvert.SerializeObject(
                        new
                        {
                            nationalCode = model.NationalCode,
                            birthDate = model.ShamsiBirthDate,
                            track_Id = trackId
                        });
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                    var curl = client.GenerateCurlInString(request);

                    logId = await neginHubLogRepository
                       .AddLog(new NeginHubLog()
                       {
                           Body = body,
                           Url = requestUrl,
                           ServiceName = NeginHubServiceName.GetCivilRegistryData.ToString(),
                           TrackId = trackId,
                           MethodType = MethodType.Post,
                           OpratorId = creator,
                           Curl = curl
                       });
                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await neginHubLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("data") &&
                        resultObject["data"] != null &&
                        resultObject.ContainsKey("meta") &&
                        ((JObject)resultObject["meta"]!).ContainsKey("is_success") &&
                        Convert.ToBoolean(((JObject)resultObject["meta"]!)["is_success"]))
                    {
                        return JsonConvert.DeserializeObject<GetCivilRegistryDataResultModel>(resultObject["data"]!.ToString());
                    }
                    else
                    {
                        if (resultObject.ContainsKey("meta") &&
                           (((JObject)resultObject["meta"]!).ContainsKey("error_type") ||
                           ((JObject)resultObject["meta"]!).ContainsKey("error_message") ||
                           ((JObject)resultObject["meta"]!).ContainsKey("code")))
                        {
                            var error = string.Empty;
                            if (((JObject)resultObject["meta"]!).ContainsKey("error_type"))
                                error = $"error_type : ${((JObject)resultObject["meta"]!)["error_type"]}";

                            if (((JObject)resultObject["meta"]!).ContainsKey("error_message"))
                                error += $"-error_message : ${((JObject)resultObject["meta"]!)["error_message"]}";

                            if (((JObject)resultObject["meta"]!).ContainsKey("code"))
                                error += $"-code : ${((JObject)resultObject["meta"]!)["code"]}";

                            throw new AppException(error);
                        }
                        else
                            logger.LogError($"'GetCivilRegistryData' Method : NationalCode : {model.NationalCode}, BirthDate : {model.ShamsiBirthDate}. unexpected error!");

                        return new GetCivilRegistryDataResultModel()
                        {
                            IsSuccess = false
                        };
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await neginHubLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError(exp, exp.Message);
                logger.LogError($"responseBody : {responseBody}");
                return null;
            }
        }
        public async Task<bool?> NationalCodeAndCardVerification(string cardNumber, [NationalCode] string nationalCode, string shamsiBirthdate, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetToken(creator);
            if (token == null) throw new AppException("return result from 'GetToken()' is null!");
            var responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                string requestUrl = $"{BASE_URL}/api/v4/kyc/NationalCodeAndCardVerification";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
                {
                    var trackId = Guid.NewGuid().ToString();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var body = JsonConvert.SerializeObject(
                            new
                            {
                                cardNumber,
                                nationalCode,
                                birthDate = shamsiBirthdate,
                                trackId
                            });
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                    var curl = client.GenerateCurlInString(request);
                    logId = await neginHubLogRepository
                       .AddLog(new NeginHubLog()
                       {
                           Body = body,
                           Url = requestUrl,
                           ServiceName = NeginHubServiceName.NationalCodeAndCardVerification.ToString(),
                           TrackId = trackId,
                           MethodType = MethodType.Post,
                           OpratorId = creator,
                           Curl = curl
                       });

                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await neginHubLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);
                    if (resultObject.ContainsKey("data") &&
                      resultObject["data"] != null &&
                      ((JObject)resultObject["data"]!).ContainsKey("isMatch"))
                    {
                        return Convert.ToBoolean(((JObject)resultObject["data"]!)["isMatch"]);
                    }
                    else
                    {
                        if (resultObject.ContainsKey("meta") &&
                           (((JObject)resultObject["meta"]!).ContainsKey("error_type") ||
                           ((JObject)resultObject["meta"]!).ContainsKey("error_message") ||
                           ((JObject)resultObject["meta"]!).ContainsKey("code")))
                        {
                            var error = string.Empty;
                            if (((JObject)resultObject["meta"]!).ContainsKey("error_type"))
                                error = $"error_type : ${((JObject)resultObject["meta"]!)["error_type"]}";

                            if (((JObject)resultObject["meta"]!).ContainsKey("error_message"))
                                error += $"-error_message : ${((JObject)resultObject["meta"]!)["error_message"]}";

                            if (((JObject)resultObject["meta"]!).ContainsKey("code"))
                                error += $"-code : ${((JObject)resultObject["meta"]!)["code"]}";

                            logger.LogError($"'NationalCodeAndCardVerification' Method - CardNunber : {cardNumber} , NationalCode : {nationalCode} ,Birthdate : {shamsiBirthdate} Error : {error}!");

                            return null;
                        }
                        else
                            logger.LogError($"'NationalCodeAndCardVerification' Method - CardNunber : {cardNumber} , NationalCode : {nationalCode} ,Birthdate : {shamsiBirthdate}. unexpected error!");

                        return null;
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await neginHubLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError($"responseBody : {responseBody}");
                logger.LogError(exp, exp.Message);
                return null;
            }
        }
        public async Task<CardToIbanRasult?> CardToIBAN(string cardNumber, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetToken(creator);
            if (token == null) throw new AppException("return result from 'GetToken()' is null!");
            var responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                string requestUrl = $"{BASE_URL}/api/v1/KYC/CardToIBan";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
                {
                    var trackId = Guid.NewGuid().ToString();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var body = JsonConvert.SerializeObject(
                            new
                            {
                                card = cardNumber,
                                track_id = trackId
                            });
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                    var curl = client.GenerateCurlInString(request);
                    logId = await neginHubLogRepository
                       .AddLog(new NeginHubLog()
                       {
                           Body = body,
                           Url = requestUrl,
                           ServiceName = NeginHubServiceName.CardToIban.ToString(),
                           TrackId = trackId,
                           MethodType = MethodType.Post,
                           OpratorId = creator,
                           Curl = curl
                       });

                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await neginHubLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);
                    if (resultObject.ContainsKey("data") &&
                      resultObject["data"] != null &&
                      resultObject.ContainsKey("meta") &&
                      ((JObject)resultObject["meta"]!).ContainsKey("is_success") &&
                      Convert.ToBoolean(((JObject)resultObject["meta"]!)["is_success"]))
                    {
                        var result = JsonConvert.DeserializeObject<CardToIbanRasult>(resultObject["data"]!.ToString())!;
                        result.NeginHubLogId = logId;
                        result.IsSuccess = true;
                        return result;
                    }
                    else
                    {
                        if (resultObject.ContainsKey("meta") &&
                           (((JObject)resultObject["meta"]!).ContainsKey("error_type") ||
                           ((JObject)resultObject["meta"]!).ContainsKey("error_message") ||
                           ((JObject)resultObject["meta"]!).ContainsKey("code")))
                        {
                            var error = string.Empty;
                            if (((JObject)resultObject["meta"]!).ContainsKey("error_type"))
                                error = $"error_type : ${((JObject)resultObject["meta"]!)["error_type"]}";

                            if (((JObject)resultObject["meta"]!).ContainsKey("error_message"))
                                error += $"-error_message : ${((JObject)resultObject["meta"]!)["error_message"]}";

                            if (((JObject)resultObject["meta"]!).ContainsKey("code"))
                                error += $"-code : ${((JObject)resultObject["meta"]!)["code"]}";

                            logger.LogError($"'CardToIban' Method - CardNunber : {cardNumber}, Error : {error}!");

                            return new CardToIbanRasult()
                            {
                                IsSuccess = false,
                                ErrorMessage = error
                            };
                        }
                        else
                            logger.LogError($"'CardToIban' Method - CardNunber : {cardNumber}. unexpected error!");

                        return new CardToIbanRasult()
                        {
                            IsSuccess = false
                        };
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await neginHubLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError($"responseBody : {responseBody}");
                logger.LogError(exp, exp.Message);
                return new CardToIbanRasult()
                {
                    IsSuccess = false,
                };
            }
        }
        public async Task<GetCivilRegistryDataResultModel> GetCivilRegistryData(GetCivilRegistryDataInputModel model, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetToken(creator);
            if (token == null) throw new AppException("return result from 'token' is null!");
            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                using (var client = new HttpClient())
                {
                    var trackId = Guid.NewGuid().ToString();
                    string requesturl = $"https://api.neginhub.com/api/v1/KYC/GetCivilRegistryData";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var body = JsonConvert.SerializeObject(
                        new
                        {
                            nationalCode = model.NationalCode,
                            birthDate = model.ShamsiBirthDate,
                            track_id = trackId
                        });
                    HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");

                    logId = await neginHubLogRepository
                       .AddLog(new NeginHubLog()
                       {
                           Body = body,
                           Url = requesturl,
                           ServiceName = NeginHubServiceName.GetCivilRegistryData.ToString(),
                           TrackId = trackId,
                           MethodType = MethodType.Post,
                           OpratorId = creator
                       });
                    HttpResponseMessage response = await client.PostAsync(requesturl, content, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await neginHubLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("data") &&
                        resultObject["data"] != null &&
                        resultObject.ContainsKey("meta") &&
                        ((JObject)resultObject["meta"]).ContainsKey("is_success") &&
                        Convert.ToBoolean(((JObject)resultObject["meta"])["is_success"]))
                    {
                        return JsonConvert.DeserializeObject<GetCivilRegistryDataResultModel>(resultObject["data"].ToString());
                    }
                    else
                    {
                        if (resultObject.ContainsKey("meta") &&
                           (((JObject)resultObject["meta"]).ContainsKey("error_type") ||
                           ((JObject)resultObject["meta"]).ContainsKey("error_message") ||
                           ((JObject)resultObject["meta"]).ContainsKey("code")))
                        {
                            var error = string.Empty;
                            if (((JObject)resultObject["meta"]).ContainsKey("error_type"))
                                error = $"error_type : ${((JObject)resultObject["meta"])["error_type"]}";

                            if (((JObject)resultObject["meta"]).ContainsKey("error_message"))
                                error += $"-error_message : ${((JObject)resultObject["meta"])["error_message"]}";

                            if (((JObject)resultObject["meta"]).ContainsKey("code"))
                                error += $"-code : ${((JObject)resultObject["meta"])["code"]}";

                            throw new AppException(error);
                        }
                        else
                            logger.LogError($"'GetCivilRegistryData' Method : NationalCode : {model.NationalCode}, BirthDate : {model.ShamsiBirthDate}. unexpected error!");

                        return null;
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await neginHubLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError(exp, exp.Message);
                logger.LogError($"responseBody : {responseBody}");
                return null;
            }
        }
        public async Task<SanaInquieryDataResultModel?> SanaInquiryV5(SanaInquieryDataInputModel model, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetToken(creator);
            if (token == null) throw new AppException("return result from 'token' is null!");

            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;

            try
            {
                string requestUrl = $"{BASE_URL}/api/v5/Kyc/SanaInquiry";

                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
                {
                    var trackId = Guid.NewGuid().ToString();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var body = JsonConvert.SerializeObject(
                        new
                        {
                            nationalCode = model.NationalCode,
                            type = model.Type
                        });

                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                    var curl = client.GenerateCurlInString(request);
                    logId = await neginHubLogRepository
                       .AddLog(new NeginHubLog()
                       {
                           Body = body,
                           Url = requestUrl,
                           ServiceName = NeginHubServiceName.SanaInquiryV5.ToString(),
                           TrackId = trackId,
                           MethodType = MethodType.Post,
                           OpratorId = creator,
                           Curl = curl
                       });

                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await neginHubLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("data") &&
                        resultObject["data"] != null &&
                        resultObject.ContainsKey("meta") &&
                        ((JObject)resultObject["meta"]).ContainsKey("isSuccess") &&
                        Convert.ToBoolean(((JObject)resultObject["meta"])["isSuccess"]))
                    {
                        return JsonConvert.DeserializeObject<SanaInquieryDataResultModel>(resultObject["data"].ToString());
                    }
                    else
                    {
                        if (resultObject.ContainsKey("meta") &&
                           (((JObject)resultObject["meta"]).ContainsKey("errorMessage") ||
                           ((JObject)resultObject["meta"]).ContainsKey("code")))
                        {
                            var error = string.Empty;

                            if (((JObject)resultObject["meta"]).ContainsKey("errorMessage"))
                                error += $"-errorMessage : {((JObject)resultObject["meta"])["errorMessage"]}";

                            logger.LogError($"'SanaInquiryV5' Method : NationalCode : {model.NationalCode}, error : {resultObject["meta"]}!");

                            return new SanaInquieryDataResultModel()
                            {
                                IsSuccess = false,
                                ErrorMessage = error
                            };
                        }
                        else
                            logger.LogError($"'SanaInquiryV5' Method : NationalCode : {model.NationalCode}, unexpected error!");

                        return new SanaInquieryDataResultModel()
                        {
                            IsSuccess = false
                        };
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await neginHubLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError(exp, exp.Message);
                logger.LogError($"responseBody : {responseBody}");
                return new SanaInquieryDataResultModel()
                {
                    IsSuccess = false,
                    ErrorMessage = "عدم پاسخگویی سرویس سامانه ثنا!"
                };
            }
        }
        public async Task<PostalCodeInquieryDataResultModel?> PostalCodeInquiry(PostalCodeInquieryDataInputModel model, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetToken(creator);
            if (token == null) throw new AppException("return result from 'token' is null!");

            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;

            try
            {
                string requestUrl = $"{BASE_URL}/api/v4/kyc/PostalCodeInquiry";

                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
                {
                    if (string.IsNullOrWhiteSpace(model.TrackId))
                        model.TrackId = null;
                    var trackId = model.TrackId ?? Guid.NewGuid().ToString();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var body = JsonConvert.SerializeObject(
                        new
                        {
                            postalCode = model.PostalCode,
                            inquiryType = model.InquiryType,
                            trackId = trackId
                        });

                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                    var curl = client.GenerateCurlInString(request);
                    logId = await neginHubLogRepository
                       .AddLog(new NeginHubLog()
                       {
                           Body = body,
                           Url = requestUrl,
                           ServiceName = NeginHubServiceName.PostalCodeInquiry.ToString(),
                           TrackId = trackId,
                           MethodType = MethodType.Post,
                           OpratorId = creator,
                           Curl = curl
                       });

                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await neginHubLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("data") &&
                        resultObject["data"] != null &&
                        resultObject.ContainsKey("meta") &&
                        ((JObject)resultObject["meta"]).ContainsKey("isSuccess") &&
                        Convert.ToBoolean(((JObject)resultObject["meta"])["isSuccess"]))
                    {
                        return new PostalCodeInquieryDataResultModel()
                        {
                            IsSuccess = true,
                            Data = resultObject["data"]?.ToString(),
                            ErrorMessage = ((JObject)resultObject["meta"])["errorMessage"]?.ToString(),
                            Code = Convert.ToInt32(((JObject)resultObject["meta"])["code"]),
                            ErrorType = ((JObject)resultObject["meta"])["errorType"]?.ToString()
                        };
                    }
                    else
                    {
                        if (resultObject.ContainsKey("meta") &&
                           (((JObject)resultObject["meta"]).ContainsKey("errorMessage") ||
                           ((JObject)resultObject["meta"]).ContainsKey("code")))
                        {
                            var error = string.Empty;

                            if (((JObject)resultObject["meta"]).ContainsKey("errorMessage"))
                                error += $"-errorMessage : {((JObject)resultObject["meta"])["errorMessage"]}";

                            logger.LogError($"'PostalCodeInquiry' Method : PostalCode : {model.PostalCode}, error : {resultObject["meta"]}!");

                            return new PostalCodeInquieryDataResultModel()
                            {
                                IsSuccess = false,
                                ErrorMessage = error,
                                Code = Convert.ToInt32(((JObject)resultObject["meta"])["code"]),
                                ErrorType = ((JObject)resultObject["meta"])["errorType"]?.ToString()
                            };
                        }
                        else
                            logger.LogError($"'PostalCodeInquiry' Method : PostalCode : {model.PostalCode}, unexpected error!");

                        return new PostalCodeInquieryDataResultModel()
                        {
                            IsSuccess = false
                        };
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await neginHubLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError(exp, exp.Message);
                logger.LogError($"responseBody : {responseBody}");
                return new PostalCodeInquieryDataResultModel()
                {
                    IsSuccess = false,
                    ErrorMessage = "عدم پاسخگویی سرویس اطلاعات کد پستی!"
                };
            }
        }
        public async Task<ClientIdAndAccountNumbersAyandehResultModel> GetClientIdAndAccountNumbersAyandeh(ClientIdAndAccountNumbersAyandehInputModel model, Guid? creator, CancellationToken cancellationToken) 
        {
            var token = await GetToken(creator);
            if (token == null) throw new AppException("return result from 'token' is null!");

            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;

            try
            {
                string requestUrl = $"{BASE_URL}/api/v4/Ebank/GetClientIdAndAccountNumbers";

                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
                {
                    if (model.TrackId == default)
                        model.TrackId = null;
                    var trackId = model.TrackId ?? Guid.NewGuid();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var body = JsonConvert.SerializeObject(
                        new
                        {
                            nationalCode = model.NationalCode,
                            trackId = trackId.ToString()
                        });

                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                    var curl = client.GenerateCurlInString(request);
                    logId = await neginHubLogRepository
                       .AddLog(new NeginHubLog()
                       {
                           Body = body,
                           Url = requestUrl,
                           ServiceName = NeginHubServiceName.PostalCodeInquiry.ToString(),
                           TrackId = trackId.ToString(),
                           MethodType = MethodType.Post,
                           OpratorId = creator,
                           Curl = curl
                       });

                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await neginHubLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);
                    var result = System.Text.Json.JsonSerializer.Deserialize<ClientIdAndAccountNumbersAyandehResultModel>(responseBody, new System.Text.Json.JsonSerializerOptions() 
                    { 
                        PropertyNameCaseInsensitive = true
                    });

                    if (!result.Meta.IsSuccess)
                        logger.LogError($"'GetClientIdAndAccountNumbers' Method : NationalCode : {model.NationalCode}, error : {result.Meta}!");

                    return result;
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await neginHubLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError(exp, exp.Message);
                logger.LogError($"responseBody : {responseBody}");
                return new ClientIdAndAccountNumbersAyandehResultModel()
                {
                    Meta = new() { IsSuccess = false, Message = "عدم پاسخگویی سرویس استعلام شماره حساب بانک آینده!" }
                };
            }
        }
        public async Task<DepositToIbanResponseModel> DepositToIbanAsync(DepositToIbanInputModel model, Guid operatorId, CancellationToken cancellationToken)
        {
            var token = await GetToken(operatorId);
            if (token == null) throw new AppException("return result from 'token' is null!");

            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;

            try
            {
                string requestUrl = $"{BASE_URL}/api/v3/KYC/DepositToIban";

                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
                {
                    if (string.IsNullOrEmpty(model.TrackId))
                        model.TrackId = null;
                    var trackId = model.TrackId ?? Guid.NewGuid().ToString();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var body = JsonConvert.SerializeObject(
                        new
                        {
                            deposit = model.Deposit,
                            bankCode = model.BankCode,
                            trackId = trackId.ToString()
                        });

                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                    var curl = client.GenerateCurlInString(request);
                    logId = await neginHubLogRepository
                       .AddLog(new NeginHubLog()
                       {
                           Body = body,
                           Url = requestUrl,
                           ServiceName = NeginHubServiceName.PostalCodeInquiry.ToString(),
                           TrackId = trackId.ToString(),
                           MethodType = MethodType.Post,
                           OpratorId = operatorId,
                           Curl = curl
                       });

                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await neginHubLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);
                    var result = System.Text.Json.JsonSerializer.Deserialize<DepositToIbanResponseModel>(responseBody, new System.Text.Json.JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (!result.Meta.IsSuccess)
                        logger.LogError($"'DepositToIbanAsync' Method : Deposit : {model.Deposit}, error : {result.Meta}!");

                    return result;
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await neginHubLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError(exp, exp.Message);
                logger.LogError($"responseBody : {responseBody}");
                return new DepositToIbanResponseModel()
                {
                    Meta = new() { IsSuccess = false, Message = "عدم پاسخگویی سرویس استعلام شماره شبا بانک آینده!" }
                };
            }
        }
        #region base services
        private async Task<string> GetToken(Guid? operatorId)
        {
            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {                
                var requestUrl = "https://api.neginhub.com/api/v1/Users/getToken";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
                {
                    var grant_type = "password";

                    var body = JsonConvert.SerializeObject(new { grant_type, Username = USER_NAME, Password = PASSWORD });
                    client.DefaultRequestHeaders.Add("Authorization", $"Basic Og==");

                    HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");
                    request.Content = content;

                    var curl = client.GenerateCurlInString(request);
                    logId = await neginHubLogRepository
                       .AddLog(new NeginHubLog()
                       {
                           Body = body,
                           Url = requestUrl,
                           ServiceName = NeginHubServiceName.GetToken.ToString(),
                           TrackId = string.Empty,
                           MethodType = MethodType.Post,
                           OpratorId = operatorId,
                           Curl = curl
                       });

                    HttpResponseMessage response = await client.SendAsync(request, default);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await neginHubLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        JObject resultObject = JObject.Parse(responseBody);

                        if (resultObject.ContainsKey("access_token") && resultObject["access_token"] != null)
                        {
                            return resultObject["access_token"].ToString();
                        }
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        JObject resultObject = JObject.Parse(responseBody);

                        if (resultObject.ContainsKey("meta") &&
                            resultObject["meta"] != null)
                        {
                            var error = string.Empty;
                            if (((JObject)resultObject["meta"]).ContainsKey("error_type"))
                                error = $"error_type : ${((JObject)resultObject["meta"])["error_type"]}";

                            if (((JObject)resultObject["meta"]).ContainsKey("error_message"))
                                error += $"-error_message : ${((JObject)resultObject["meta"])["error_message"]}";

                            if (((JObject)resultObject["meta"]).ContainsKey("code"))
                                error += $"-code : ${((JObject)resultObject["meta"])["code"]}";

                            if (error != string.Empty)
                                throw new AppException(error);
                        }
                    }

                    return null;

                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await neginHubLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError(exp, exp.Message);
            }
            return null;
        }
        private async Task<string> RefreshToken(string refreshToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string requesturl = "https://api.neginhub.com/api/v1/Users/getToken";
                    var grant_type = "refresh_token";

                    var body = JsonConvert.SerializeObject(new { grant_type, refresh_token = refreshToken });

                    HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(requesturl, content);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JObject resultObject = JObject.Parse(responseBody);

                        if (resultObject.ContainsKey("access_token") && resultObject["access_token"] != null)
                        {
                            return resultObject["access_token"].ToString();
                        }
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JObject resultObject = JObject.Parse(responseBody);

                        if (resultObject.ContainsKey("meta") &&
                            (((JObject)resultObject["meta"]).ContainsKey("error_type") ||
                            ((JObject)resultObject["meta"]).ContainsKey("error_message") ||
                            ((JObject)resultObject["meta"]).ContainsKey("code")))
                        {
                            var error = string.Empty;
                            if (((JObject)resultObject["meta"]).ContainsKey("error_type"))
                                error = $"error_type : ${((JObject)resultObject["meta"])["error_type"]}";

                            if (((JObject)resultObject["meta"]).ContainsKey("error_message"))
                                error += $"-error_message : ${((JObject)resultObject["meta"])["error_message"]}";

                            if (((JObject)resultObject["meta"]).ContainsKey("code"))
                                error += $"-code : ${((JObject)resultObject["meta"])["code"]}";

                            logger.LogError(error);

                            throw new AppException(error);
                        }
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

        #endregion base services
    }
}
