using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.Model.AccountStatement;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZarinLend.Services.Model.Promissory;
using HttpClientToCurl;
using Services.Model;

namespace Services
{
    public class FinnotechService : IFinnotechService, IScopedDependency
    {
        /*
         * https://console.finnotech.ir/login
         * User: 0012386251
         * Pass: MehrdadPayandeh1369@
         */
        private const string CLIENT_ID = "zarinpal1";
        private const string CLIENT_SECRET_ID = "635ebbba67c8550d40c7";
        private const string CLIENT_NID = "0079770940";
        private const string BASE_URL = "https://apibeta.finnotech.ir";
        private const string AYANDEH_BANK_CODE = "062";
        private const string REFRESH_CODE_CARD_TO_IBAN = "lE19S1pfgSB08sHtNDmEp4Qm9NTzUU0XmnvXW9OrzNCOWsKBZNZv9biv2ftJ9ZkZnC9XYNq3ewUx3Tpgbj2tMuTByzZ9xVT83GsRs6e6Usn8wo8VQnO2CvtyV7aU96kZCxvVYeOQ6uNZnHxCLXosVGJhgpa0WZJjxfGaK2ilEu8wzCtFOrFxpw4w6gwODaP4d1EJRB1QUVXRt6lcjACbRZXbssAg4dYTNzfqVsfccprv3sylRwLNcQqdPHvITgyI";
        private const string REFRESH_CODE_CIF_INQUIRY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJiYW5rIjoiMDYyIiwiY2xpZW50SWQiOiJ6YXJpbnBhbDEiLCJjbGllbnRJbmZvIjoiMzM4ZGYyYmYyM2RmNzVkZDM1NzYzMjcxMmE4NDJhMTUzZTVjMjFmODg2ZmE3ZjRjZDdjZmIzYzczYjU5NTExOWY3YjY4YzU1MTI3MjY2ZTgzZmY0ODhlYjMzMjQ0NWIzMThkNTQwNGEzZGUwMTMzYTRjYmY0ZmQyNjEyOTUyNDAwYjI4NzhjODQ4ZmM1MGIwZWE5MTgzODJhNTYxM2QxZTY4Mzg5YWYxMjYzYWI3MjUyZmE3NWY5YjZiYzA3MzY1YmRiOTFjMmNjZmEyMWJmMzBjMjhjMjE2NmY5OTYzODdkZWY1YTk1MGRmYzJhNzg0MDIyMDkyMmZiNzRlNDBlZWI0ZDgzYTJjOTBmNmM4MmJkNDYwNjJjNjQ0YmY4NjlkMmZhMGM0MWQ1OTQ4M2VhNjVlMDhiYWRlMWZhNmRkYmM4YTg2ZTVmMzM1ZTM4MDM4YTQiLCJjcmVhdGlvbkRhdGUiOiIxNDAzMDkwNjE1MTkyNCIsInRpbWVzdGFtcCI6MTczMjYyMTc2NDA3NCwiZ3JhbnRUeXBlIjoiQ0xJRU5UX0NSRURFTlRJQUxTIiwicmVmcmVzaFRva2VuSWQiOiJlNzU4N2UyNS03ZjBiLTQzZGYtYWQ3MC03YjMyMTRhMjk5ZjciLCJ2ZXJzaW9uIjoiNCIsInRva2VuSWQiOiIzNGE1MDU4NS02OGU4LTQzYTctOWY4MC0yMDExNzFiZTE2ZTkiLCJ0b2tlblR5cGUiOiJBQ0NFU1NfVE9LRU4iLCJsaWZlVGltZSI6ODY0MDAwMDAwLCJpYXQiOjE3MzI2MjE3NjQsImV4cCI6MTczMzQ4NTc2NH0.FMJY4TyYM2Dv5pFziZbIRwR6Yd29Fz_ZpCcRCcmFuf8";
        private const string REFRESH_CODE_CHARGE_CARD = "R0nXzG4JK2iaBiRXBciDz5edQ4WsxEGa02UOH3MMLKLWyJY4Wwe89QfXzHBNXit0XlMdJxoNRIFq6Xuf1euPiuajtnMFJm5meDvh8ZAPuHpYE7MD1MRK9TCAfwtUvuU8Su8OrnLJOgiyCkFwnlkmLBYH3zDdLDDIhXvcROinjLtFPMai3n5Yj0oimweSTrNzF2s2qa7Hx5XbWPW5gFka3nRXNsu69QIWtOWLFivNIWtYOdGlOlTXDSfRONCs99fP";
        public const string ACCOUNT_STATEMENT_SCOPE = "oak:statement:get";
        public const string CIP_INQUIRY_SCOPE = "oak:cif-inquiry:get";
        //public const string PROMISSORY_SCOPES = "promissory:publish-request:post,promissory:inquiry:get,promissory:finalize:post,sign:promissory-sign-request:post,promissory:delete:get,promissory:publish-request-inquiry:get,sign:status-inquiry:get,sign:signed-document:get";
        public const string PROMISSORY_SCOPES = "promissory:publish-request:post,promissory:inquiry:get,promissory:finalize:post,sign:promissory-sign-request:post,promissory:delete:get,promissory:publish-request-inquiry:get,sign:status-inquiry:get";
        private readonly ILogger<FinnotechService> logger;
        private readonly IFinotechLogRepository finotechLogRepository;

        public FinnotechService(ILogger<FinnotechService> logger, IFinotechLogRepository finotechLogRepository)
        {
            this.logger = logger;
            this.finotechLogRepository = finotechLogRepository;
        }

        public string GenerateUrlToGetAuthorizationCode(string redirectUrl, string scope, string orderId)
        {
            return $"{BASE_URL}/dev/v2/oauth2/authorize?client_id={CLIENT_ID}&response_type=code&redirect_uri={redirectUrl.ToLower()}&scope={scope}&bank={AYANDEH_BANK_CODE}&state={orderId}";
        }

        public async Task<AccountStatementModel?> GetAccountStatement(AccountStatementInput filter, string code, string redirectUrl, Guid? creator, CancellationToken cancellationToken)
        {
            //oak:statement:get
            //redirectUrl = "https://www.google.com";
            var token = await GetAuthorizationCodeToken(code, redirectUrl);
            if (token == null) throw new AppException("return result from 'GetAuthorizationCodeToken()' is null!");
            var responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                using (var client = new HttpClient())
                {
                    var trackId = Guid.NewGuid().ToString();
                    filter.fromDate = filter.fromDate.Length > 6 ? filter.fromDate.Substring(2) : filter.fromDate;
                    filter.toDate = filter.toDate.Length > 6 ? filter.toDate.Substring(2) : filter.toDate;
                    string requesturl = $"{BASE_URL}/oak/v2/clients/{CLIENT_ID}/deposits/{filter.deposit}/statement?toDate={filter.toDate}&fromDate={filter.fromDate}&trackId={trackId}";

                    if (!string.IsNullOrEmpty(filter.fromTime))
                        requesturl += $"&fromTime={filter.fromTime}";

                    if (!string.IsNullOrEmpty(filter.toTime))
                        requesturl += $"&toTime={filter.toTime}";

                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    logId = await finotechLogRepository
                        .AddLog(new FinotechLog()
                        {
                            Url = requesturl,
                            ServiceName = FinotechServiceName.AccountStatement.ToString(),
                            TrackId = trackId,
                            MethodType = MethodType.Get,
                            OpratorId = creator
                        });

                    HttpResponseMessage response = await client.GetAsync(requesturl, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await finotechLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject != null)
                    {
                        logger.LogWarning($"GetAccountStatement :TrackId : {trackId} ,  deposit :{filter.deposit} , Response : {responseBody}!");
                        return JsonConvert.DeserializeObject<AccountStatementModel>(resultObject.ToString())!;
                    }
                    else
                    {
                        logger.LogWarning($"GetAccountStatement : deposit :{filter.deposit}. unexpected error!");
                        return null;
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await finotechLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError($"responseBody : {responseBody}");
                logger.LogError(exp, exp.Message);
                return null;
            }
        }
        //public async Task<CardToIbanRasult> CardToIBAN(string cardNumber, Guid? creator, CancellationToken cancellationToken)
        //{
        //    var token = await GetClientCredentialToken(REFRESH_CODE_CARD_TO_IBAN);
        //    if (token == null) throw new AppException("return result from 'GetClientCredentialToken()' is null!");
        //    var responseBody = string.Empty;
        //    long logId = 0;
        //    var logUpdateSuccessed = false;
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            var trackId = Guid.NewGuid().ToString();
        //            string requesturl = $"{BASE_URL}/facility/v2/clients/{CLIENT_ID}/cardToIban?version=2&trackId={trackId}&card={cardNumber}";
        //            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        //            logId = await finotechLogRepository
        //                .AddLog(new FinotechLog()
        //                {
        //                    Url = requesturl,
        //                    ServiceName = FinotechServiceName.CardToIban.ToString(),
        //                    TrackId = trackId,
        //                    MethodType = MethodType.Get,
        //                    OpratorId = creator
        //                });

        //            HttpResponseMessage response = await client.GetAsync(requesturl, cancellationToken);
        //            responseBody = await response.Content.ReadAsStringAsync();

        //            if (logId > 0)
        //            {
        //                await finotechLogRepository.UpdateLog(logId, responseBody);
        //                logUpdateSuccessed = true;
        //            }

        //            JObject resultObject = JObject.Parse(responseBody);

        //            if (resultObject != null)
        //            {
        //                logger.LogWarning($"CardToIBAN :TrackId : {trackId} ,  CardNumber :{cardNumber} , Response : {responseBody}!");
        //                return JsonConvert.DeserializeObject<CardToIbanRasult>(resultObject.ToString());
        //            }
        //            else
        //            {
        //                logger.LogWarning($"CardToIBAN : cardNumber :{cardNumber}. unexpected error!");
        //                return null;
        //            }
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        if (logId > 0 && !logUpdateSuccessed)
        //            await finotechLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

        //        logger.LogError($"responseBody : {responseBody}");
        //        logger.LogError(exp, exp.Message);
        //        return null;
        //    }
        //}

        public async Task<string> CifInquiry(string nationalCode, Guid? creator, CancellationToken cancellationToken)
        {
            //var token = await GetClientCredentialToken(REFRESH_CODE_CIF_INQUIRY);
            var token = await GetClientCredentialToken(CIP_INQUIRY_SCOPE);
            if (token == null) throw new AppException("return result from 'GetClientCredentialToken()' is null!");
            var responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                using (var client = new HttpClient())
                {
                    var trackId = Guid.NewGuid().ToString();
                    string requesturl = $"{BASE_URL}/oak/v2/clients/{CLIENT_ID}/users/{nationalCode}/cifInquiry?trackId={trackId}";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    logId = await finotechLogRepository
                        .AddLog(new FinotechLog()
                        {
                            Url = requesturl,
                            ServiceName = FinotechServiceName.CifInquiry.ToString(),
                            TrackId = trackId,
                            MethodType = MethodType.Get,
                            OpratorId = creator
                        });

                    HttpResponseMessage response = await client.GetAsync(requesturl, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await finotechLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject != null)
                    {
                        logger.LogWarning($"CifInquiry :TrackId : {trackId} ,  NationalCode :{nationalCode} , Response : {responseBody}!");
                        if (resultObject["status"].ToString().Equals("DONE", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (resultObject.ContainsKey("result"))
                            {
                                if (((JObject)resultObject["result"]).ContainsKey("cif"))
                                    return resultObject["result"]["cif"].ToString();
                                else if (((JObject)resultObject["result"]).ContainsKey("message") &&
                                        !string.IsNullOrEmpty(Convert.ToString(resultObject["result"]["message"])))
                                {
                                    throw new AppException(resultObject["result"]["message"].ToString());
                                }
                            }
                        }
                        else if (resultObject["status"].ToString().Equals("FAILED", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (resultObject.ContainsKey("error") && ((JObject)resultObject["error"]).ContainsKey("message")
                               && (!string.IsNullOrEmpty(Convert.ToString(resultObject["error"]["message"])) || !string.IsNullOrEmpty(Convert.ToString(resultObject["error"]["code"]))))
                                throw new AppException($"{Convert.ToString(resultObject["error"]["message"])}-{Convert.ToString(resultObject["error"]["code"])}");
                        }
                        return null;
                    }
                    else
                    {
                        logger.LogWarning($"NationalCode : cardNumber :{nationalCode}. unexpected error!");
                        return null;
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await finotechLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError($"responseBody : {responseBody}");

                throw new AppException(exp.Message, exp);
            }
        }
        public async Task<Tuple<bool, string>> ChargeCard(int requestFacilityId, string cardNumber, long amount, Guid creator, CancellationToken cancellationToken)
        {
            return new Tuple<bool, string>(true, "ok");
            var token = await GetCodeToken(REFRESH_CODE_CHARGE_CARD);
            if (token == null) throw new AppException("return result from 'GetCodeToken()' is null!");
            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                using (var client = new HttpClient())
                {
                    var trackId = Guid.NewGuid().ToString();
                    string requesturl = $"{BASE_URL}/oak/v2/clients/zarinpal1/deposits/0302203911004/chargeCard?trackId={trackId}";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var content = JsonConvert.SerializeObject(
                        new
                        {
                            amount = 20001,// amount,
                            card = "6362144800312139",//cardNumber
                        });
                    var body = new StringContent(content, Encoding.UTF8, "application/json");

                    logId = await finotechLogRepository
                        .AddLog(new FinotechLog()
                        {
                            RequestFacilityId = requestFacilityId,
                            Url = requesturl,
                            ServiceName = FinotechServiceName.ChargeCard.ToString(),
                            TrackId = trackId,
                            Body = content,
                            MethodType = MethodType.Post,
                            OpratorId = creator
                        });
                    HttpResponseMessage response = await client.PostAsync(requesturl, body, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await finotechLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("status") &&
                        resultObject["status"].ToString().Equals("done", StringComparison.CurrentCultureIgnoreCase) &&
                        resultObject.ContainsKey("result"))
                    {
                        if ((resultObject["result"] as JObject).ContainsKey("chargeState") &&
                            resultObject["result"]["chargeState"].ToString().Equals("done", StringComparison.CurrentCultureIgnoreCase))
                            return new Tuple<bool, string>(true, null);
                        else
                        {
                            var message = $"message : {resultObject["result"]["message"]}";
                            if ((resultObject["result"] as JObject).ContainsKey("farsi_message"))
                                message += $", farsi_message : {resultObject["result"]["farsi_message"]}";
                            if ((resultObject["result"] as JObject).ContainsKey("chargeState"))
                                message += $", chargeState : {resultObject["result"]["chargeState"]}";
                            if ((resultObject["result"] as JObject).ContainsKey("code"))
                                message += $", code : {resultObject["result"]["code"]}";
                            if ((resultObject["result"] as JObject).ContainsKey("description"))
                                message += $", description : {resultObject["result"]["description"]}";

                            logger.LogWarning($"status : {resultObject["status"]} , {message} ");
                            return new Tuple<bool, string>(false, message);
                        }
                    }
                    else
                    {
                        if (resultObject.ContainsKey("error") &&
                            (resultObject["error"] as JObject).ContainsKey("code") &&
                            (resultObject["error"] as JObject).ContainsKey("message"))
                        {
                            var message = $"message : {resultObject["error"]["message"]}";
                            if ((resultObject["error"] as JObject).ContainsKey("farsi_message"))
                                message += $", farsi_message : {resultObject["error"]["farsi_message"]}";
                            if ((resultObject["error"] as JObject).ContainsKey("chargeState"))
                                message += $", chargeState : {resultObject["error"]["chargeState"]}";
                            if ((resultObject["error"] as JObject).ContainsKey("code"))
                                message += $", code : {resultObject["error"]["code"]}";
                            if ((resultObject["error"] as JObject).ContainsKey("description"))
                                message += $", description : {resultObject["error"]["description"]}";

                            logger.LogWarning($"status : {resultObject["status"]} , {message} ");
                            return new Tuple<bool, string>(false, message);
                        }
                        else
                            logger.LogWarning($"CardCharge : cardNumber :{cardNumber}. unexpected error! responseBody :{responseBody}");

                        return new Tuple<bool, string>(false, responseBody);
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await finotechLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError(exp, exp.Message);
                logger.LogError($"responseBody : {responseBody}");
                return new Tuple<bool, string>(false, null);
            }
        }

        #region promissory methods
        public async Task<FinotechBaseResult<PromissoryPublishResponseModel>> PromissoryPublishRequest(PromissoryPublishRequestModel model, string trackId, Guid? creator, CancellationToken cancellationToken)
        {
            //oak:statement:get
            //redirectUrl = "https://www.google.com";
            var token = await GetClientCredentialToken(PROMISSORY_SCOPES);
            if (token == null) throw new AppException("return result from 'GetClientCredentialToken()' is null!");
            var responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                string requestUrl = $"{BASE_URL}/promissory/v2/clients/{CLIENT_ID}/promissoryPublishRequest?trackId={trackId}";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var body = JsonConvert.SerializeObject(model);
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                    var curl = client.GenerateCurlInString(request);
                    logId = await finotechLogRepository
                        .AddLog(new FinotechLog()
                        {
                            Body = body,
                            Url = requestUrl,
                            ServiceName = FinotechServiceName.PromissoryPublishRequest.ToString(),
                            TrackId = trackId,
                            MethodType = MethodType.Post,
                            OpratorId = creator,
                            Curl = curl
                        });

                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await finotechLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);
                    logger.LogWarning($"PromissoryPublishRequest :TrackId : {trackId} ,  body :{body} , Response : {responseBody}!");
                    if (resultObject != null &&
                        resultObject["status"]!.ToString() == "DONE" &&
                        resultObject.ContainsKey("result") &&
                        ((JObject)resultObject["result"]!).ContainsKey("unSignedPdf") &&
                        ((JObject)resultObject["result"]!).ContainsKey("promissoryId") &&
                        ((JObject)resultObject["result"]!).ContainsKey("requestId"))
                    {
                        return JsonConvert.DeserializeObject<FinotechBaseResult<PromissoryPublishResponseModel>>(resultObject.ToString())!;
                    }
                    else
                    {
                        var result = JsonConvert.DeserializeObject<FinotechBaseResult<PromissoryPublishResponseModel>>(resultObject!.ToString())!;
                        result.IsSuccess = false;
                        return result;
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await finotechLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError($"PromissoryPublishRequest : responseBody : {responseBody}");
                logger.LogError(exp, exp.Message);
                return new FinotechBaseResult<PromissoryPublishResponseModel>()
                {
                    IsSuccess = false,
                    Error = new FinotechErrorResult()
                    {
                        Code = "-100",
                        Message = "خطا در صدور سفته",
                    }
                };
            }
        }
        public async Task<FinotechBaseResult<PromissorySignRequestResponseModel>> PromissorySignRequest(PromissorySignRequestModel model, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetClientCredentialToken(PROMISSORY_SCOPES);
            if (token == null) throw new AppException("return result from 'GetClientCredentialToken()' is null!");
            var responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                var trackId = Guid.NewGuid().ToString();
                string requestUrl = $"{BASE_URL}/sign/v2/clients/{CLIENT_ID}/request?trackId={trackId}";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var body = JsonConvert.SerializeObject(model);
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                    var curl = client.GenerateCurlInString(request);
                    logId = await finotechLogRepository
                        .AddLog(new FinotechLog()
                        {
                            Body = body,
                            Url = requestUrl,
                            ServiceName = FinotechServiceName.PromissorySignRequest.ToString(),
                            TrackId = trackId,
                            MethodType = MethodType.Post,
                            OpratorId = creator,
                            Curl = curl
                        });

                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await finotechLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);
                    logger.LogWarning($"Sign Request :TrackId : {trackId} ,  body :{body} , Response : {responseBody}!");
                    if (resultObject != null &&
                        resultObject.ContainsKey("result"))
                    {
                        return JsonConvert.DeserializeObject<FinotechBaseResult<PromissorySignRequestResponseModel>>(resultObject.ToString())!;
                    }
                    else
                    {
                        var result = JsonConvert.DeserializeObject<FinotechBaseResult<PromissorySignRequestResponseModel>>(resultObject!.ToString())!;
                        result.IsSuccess = false;
                        return result;
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await finotechLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError($"Sign Request : responseBody : {responseBody}");
                logger.LogError(exp, exp.Message);
                return new FinotechBaseResult<PromissorySignRequestResponseModel>()
                {
                    IsSuccess = false,
                    Error = new FinotechErrorResult()
                    {
                        Code = "-100",
                        Message = "خطا در ثبت درخواست امضا دیجیتال توسط کاربر نهایی",
                    }
                };
            }
        }
        public async Task<FinotechBaseResult<PromissoryFinalizeResponseModel>> PromissoryFinalize(string registrationId, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetClientCredentialToken(PROMISSORY_SCOPES);
            if (token == null) throw new AppException("return result from 'GetClientCredentialToken()' is null!");
            var responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                var trackId = Guid.NewGuid().ToString();
                string requestUrl = $"{BASE_URL}/promissory/v2/clients/{CLIENT_ID}/promissoryFinalize?trackId={trackId}";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var body = JsonConvert.SerializeObject(new { registrationId });
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                    var curl = client.GenerateCurlInString(request);
                    logId = await finotechLogRepository
                        .AddLog(new FinotechLog()
                        {
                            Body = body,
                            Url = requestUrl,
                            ServiceName = FinotechServiceName.PromissoryFinalize.ToString(),
                            TrackId = trackId,
                            MethodType = MethodType.Post,
                            OpratorId = creator,
                            Curl = curl
                        });

                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();
                    logger.LogWarning($"PromissoryFinalize :RegistrationId : {registrationId} ,  body :{body} , Response : {responseBody}!");
                    if (logId > 0)
                    {
                        await finotechLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject != null &&
                        resultObject.ContainsKey("result") &&
                        ((JObject)resultObject["result"]!).ContainsKey("multiSignedPdf") &&
                        ((JObject)resultObject["result"]!).ContainsKey("promissoryId"))
                    {
                        return JsonConvert.DeserializeObject<FinotechBaseResult<PromissoryFinalizeResponseModel>>(resultObject.ToString())!;
                    }
                    else
                    {
                        var result = JsonConvert.DeserializeObject<FinotechBaseResult<PromissoryFinalizeResponseModel>>(resultObject!.ToString())!;
                        result.IsSuccess = false;
                        return result;
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await finotechLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError($"PromissoryFinalize : responseBody : {responseBody}");
                logger.LogError(exp, exp.Message);
                return new FinotechBaseResult<PromissoryFinalizeResponseModel>()
                {
                    IsSuccess = false,
                    Error = new FinotechErrorResult()
                    {
                        Code = "-100",
                        Message = "خطا در نهایی کردن سفته الکترونیک",
                    }
                };
            }
        }
        public async Task<FinotechBaseResult<PromissoryStatusInquiryResponseModel>> PromissoryStatusInquiry(string registrationId, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetClientCredentialToken(PROMISSORY_SCOPES);
            if (token == null) throw new AppException("return result from 'GetClientCredentialToken()' is null!");
            var responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                var trackId = Guid.NewGuid().ToString();
                string requestUrl = $"{BASE_URL}/sign/v2/clients/{CLIENT_ID}/statusInquiry?trackId={trackId}&registrationId={registrationId}";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUrl))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var curl = client.GenerateCurlInString(request);
                    logId = await finotechLogRepository
                        .AddLog(new FinotechLog()
                        {
                            Url = requestUrl,
                            ServiceName = FinotechServiceName.StatusInquiry.ToString(),
                            TrackId = trackId,
                            MethodType = MethodType.Get,
                            OpratorId = creator,
                            Curl = curl
                        });

                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();
                    logger.LogWarning($"StatusInquiry :RegistrationId : {registrationId} , Response : {responseBody}!");
                    if (logId > 0)
                    {
                        await finotechLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject != null &&
                        resultObject["status"]!.ToString() == "DONE" &&
                        resultObject.ContainsKey("result") &&
                        ((JObject)resultObject["result"]!).ContainsKey("signingStatus"))
                    {
                        return JsonConvert.DeserializeObject<FinotechBaseResult<PromissoryStatusInquiryResponseModel>>(resultObject.ToString())!;
                    }
                    else
                    {
                        var result = JsonConvert.DeserializeObject<FinotechBaseResult<PromissoryStatusInquiryResponseModel>>(resultObject!.ToString())!;
                        result.IsSuccess = false;
                        return result;
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await finotechLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError($"StatusInquiry : responseBody : {responseBody}");
                logger.LogError(exp, exp.Message);
                return new FinotechBaseResult<PromissoryStatusInquiryResponseModel>()
                {
                    IsSuccess = false,
                    Error = new FinotechErrorResult()
                    {
                        Code = "-100",
                        Message = "خطا در استعلام وضعیت امضا کاربر نهایی",
                    }
                };
            }
        }
        public async Task<FinotechBaseResult<PromissoryStatusInquiryResponseModel>> PromissorySignedDocument(string signingTrackId, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetClientCredentialToken(PROMISSORY_SCOPES);
            if (token == null) throw new AppException("return result from 'GetClientCredentialToken()' is null!");
            var responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                var trackId = Guid.NewGuid().ToString();
                string requestUrl = $"{BASE_URL}/promissory/v2/clients/{CLIENT_ID}/signedDocument?trackId={trackId}&signingTrackId={signingTrackId}";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUrl))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var curl = client.GenerateCurlInString(request);
                    logId = await finotechLogRepository
                        .AddLog(new FinotechLog()
                        {
                            Url = requestUrl,
                            ServiceName = FinotechServiceName.PromissorySignedDocument.ToString(),
                            TrackId = trackId,
                            MethodType = MethodType.Get,
                            OpratorId = creator,
                            Curl = curl
                        });

                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();
                    logger.LogWarning($"SignedDocument :signingTrackId : {signingTrackId} , Response : {responseBody}!");
                    if (logId > 0)
                    {
                        await finotechLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject != null &&
                        resultObject["status"]!.ToString() == "DONE" &&
                        resultObject.ContainsKey("result") &&
                        ((JObject)resultObject["result"]!).ContainsKey("signingStatus"))
                    {
                        return JsonConvert.DeserializeObject<FinotechBaseResult<PromissoryStatusInquiryResponseModel>>(resultObject.ToString())!;
                    }
                    else
                    {
                        var result = JsonConvert.DeserializeObject<FinotechBaseResult<PromissoryStatusInquiryResponseModel>>(resultObject!.ToString())!;
                        result.IsSuccess = false;
                        return result;
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await finotechLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError($"StatusInquiry : responseBody : {responseBody}");
                logger.LogError(exp, exp.Message);
                return new FinotechBaseResult<PromissoryStatusInquiryResponseModel>()
                {
                    IsSuccess = false,
                    Error = new FinotechErrorResult()
                    {
                        Code = "-100",
                        Message = "خطا در دریافت سند امضا شده",
                    }
                };
            }
        }
        public async Task<string?> PromissoryPublishRequestInquiry(string nationalCode, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetClientCredentialToken(PROMISSORY_SCOPES);
            if (token == null) throw new AppException("return result from 'GetClientCredentialToken()' is null!");
            var responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                var trackId = Guid.NewGuid().ToString();
                string requestUrl = $"{BASE_URL}/promissory/v2/clients/{CLIENT_ID}/promissoryPublishRequestInquiry?trackId={trackId}&issuerIdCode={nationalCode}";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUrl))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var curl = client.GenerateCurlInString(request);
                    logId = await finotechLogRepository
                        .AddLog(new FinotechLog()
                        {
                            Url = requestUrl,
                            ServiceName = FinotechServiceName.PromissoryPublishRequestInquiry.ToString(),
                            TrackId = trackId,
                            MethodType = MethodType.Get,
                            OpratorId = creator,
                            Curl = curl
                        });

                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();
                    logger.LogWarning($"PromissoryPublishRequestInquiry :issuerIdCode : {nationalCode} , Response : {responseBody}!");
                    if (logId > 0)
                    {
                        await finotechLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject != null &&
                        resultObject["status"]!.ToString() == "DONE" &&
                        resultObject.ContainsKey("result") &&
                        ((JObject)resultObject["result"]!).ContainsKey("requestId"))
                    {
                        return Convert.ToString(resultObject["result"]!["requestId"]);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await finotechLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError($"PromissoryPublishRequestInquiry : responseBody : {responseBody}");
                logger.LogError(exp, exp.Message);
                return null;
            }
        }
        public async Task<bool?> PromissoryDelete(string requestId, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetClientCredentialToken(PROMISSORY_SCOPES);
            if (token == null) throw new AppException("return result from 'GetClientCredentialToken()' is null!");
            var responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                var trackId = Guid.NewGuid().ToString();
                string requestUrl = $"{BASE_URL}/promissory/v2/clients/{CLIENT_ID}/promissoryDelete?trackId={trackId}&requestId={requestId}";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUrl))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var curl = client.GenerateCurlInString(request);
                    logId = await finotechLogRepository
                        .AddLog(new FinotechLog()
                        {
                            Url = requestUrl,
                            ServiceName = FinotechServiceName.PromissoryDelete.ToString(),
                            TrackId = trackId,
                            MethodType = MethodType.Get,
                            OpratorId = creator,
                            Curl = curl
                        });

                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await finotechLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject != null &&
                        resultObject["status"]!.ToString() == "DONE" &&
                        resultObject["responseCode"]!.ToString() == "FN-PSKZ-20000300000")
                    {
                        logger.LogWarning($"promissoryDelete : RequestId : {requestId} , Response : {responseBody}!");
                        return true;
                    }
                    else
                    {
                        logger.LogWarning($"promissoryDelete : responseBody : {responseBody} , unexpected error!");
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await finotechLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}");

                logger.LogError($"promissoryDelete : responseBody : {responseBody}");
                logger.LogError(exp, exp.Message);
                return null;
            }
        }

        #endregion promissory methods

        private async Task<string> GetClientCredentialTokenByRefreshToken(string refreshToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string requesturl = $"{BASE_URL}/dev/v2/oauth2/token";
                    var grant_type = "refresh_token";
                    var token_type = "CLIENT-CREDENTIAL";
                    var auth_type = "CLIENT-CREDENTIAL";
                    var bank = "062";

                    var plainTextBytes = Encoding.UTF8.GetBytes($"{CLIENT_ID}:{CLIENT_SECRET_ID}");

                    var body = JsonConvert.SerializeObject(new { grant_type, token_type, auth_type, refresh_token = refreshToken, bank });
                    client.DefaultRequestHeaders.Add("Authorization", $"Basic {Convert.ToBase64String(plainTextBytes)}");

                    HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(requesturl, content);

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("status") && resultObject["status"].ToString().Equals("done", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if ((resultObject["result"] as JObject).ContainsKey("value"))
                            return (resultObject["result"] as JObject)["value"].ToString();
                    }
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
            }
            return null;
        }
        private async Task<string> GetClientCredentialToken(string scopes)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string requesturl = $"{BASE_URL}/dev/v2/oauth2/token";
                    var grant_type = "client_credentials";
                    var nid = "0012386251";

                    var plainTextBytes = Encoding.UTF8.GetBytes($"{CLIENT_ID}:{CLIENT_SECRET_ID}");

                    var body = JsonConvert.SerializeObject(new { grant_type, nid, scopes });
                    client.DefaultRequestHeaders.Add("Authorization", $"Basic {Convert.ToBase64String(plainTextBytes)}");

                    HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(requesturl, content);

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject != null && resultObject.ContainsKey("status") && resultObject["status"]!.ToString().Equals("done", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if ((resultObject["result"] as JObject)!.ContainsKey("value"))
                            return (resultObject["result"] as JObject)!["value"]!.ToString();
                    }
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
            }
            return null;
        }
        private async Task<string> GetCodeToken(string refreshToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string requesturl = $"{BASE_URL}/dev/v2/oauth2/token";
                    var grant_type = "refresh_token";
                    var token_type = "CODE";
                    var auth_type = "CLIENT-CREDENTIAL";
                    var bank = "062";

                    var plainTextBytes = Encoding.UTF8.GetBytes($"{CLIENT_ID}:{CLIENT_SECRET_ID}");

                    var body = JsonConvert.SerializeObject(new { grant_type, token_type, auth_type, refresh_token = refreshToken, bank });
                    client.DefaultRequestHeaders.Add("Authorization", $"Basic {Convert.ToBase64String(plainTextBytes)}");
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(requesturl, content);

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("status") && resultObject["status"].ToString().Equals("done", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if ((resultObject["result"] as JObject).ContainsKey("value"))
                            return (resultObject["result"] as JObject)["value"].ToString();
                    }
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
            }
            return null;
        }
        private async Task<string> GetAuthorizationCodeToken(string code, string redirectUrl)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string requesturl = $"{BASE_URL}/dev/v2/oauth2/token";
                    var grant_type = "authorization_code";

                    var plainTextBytes = Encoding.UTF8.GetBytes($"{CLIENT_ID}:{CLIENT_SECRET_ID}");

                    var body = JsonConvert.SerializeObject(new { grant_type, code, redirect_uri = redirectUrl.ToLower(), bank = AYANDEH_BANK_CODE });
                    logger.LogInformation(body);
                    client.DefaultRequestHeaders.Add("Authorization", $"Basic {Convert.ToBase64String(plainTextBytes)}");

                    HttpContent content = new StringContent(body, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(requesturl, content);

                    string responseBody = await response.Content.ReadAsStringAsync();
                    logger.LogInformation(responseBody);

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("status") && resultObject["status"].ToString().Equals("done", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if ((resultObject["result"] as JObject).ContainsKey("value"))
                            return (resultObject["result"] as JObject)["value"].ToString();
                    }
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
            }
            return null;
        }

        //do implement get token
    }
}
