using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.Model;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class NeoZarinService : INeoZarinService, IScopedDependency
    {
        private readonly ILogger<NeoZarinService> logger;
        private readonly INeoZarinRequestSignatureLogRepository neoZarinRequestSignatureLogRepository;

        public NeoZarinService(ILogger<NeoZarinService> logger, INeoZarinRequestSignatureLogRepository neoZarinRequestSignatureLogRepository)
        {
            this.logger = logger;
            this.neoZarinRequestSignatureLogRepository = neoZarinRequestSignatureLogRepository;
        }

        public async Task<bool> SignContract(int requestFacilityId, Guid creatorId, string callbackUrl, string fileUrl, string mobile, string trackId,
            string description, CancellationToken cancellationToken)
        {
            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                using (var client = new HttpClient())
                {
                    //mobile = "09355106005";
                    var param = new SignatureParam()
                    {
                        trackId = trackId,
                        fileUrl = fileUrl,
                        description = description,
                        mobile = mobile,
                        callback = callbackUrl
                    };
                    var jsonParam = JsonConvert.SerializeObject(param);
                    var plainTextBytes = Encoding.UTF8.GetBytes(jsonParam);
                    string requesturl = $"https://api.neozarin.com/api/v2/e-kyc/apply_sign?payload={Convert.ToBase64String(plainTextBytes)}";
                    client.DefaultRequestHeaders.Add("X-Authorization", $"g8O24hRrfqVbFXNf2eZnsbzswIrkow9c6GUFdlqYmyinEKONdXTUV9iwqn9fGAGb");

                    logId = await neoZarinRequestSignatureLogRepository
                        .AddLog(new NeoZarinRequestSignatureLog()
                        {
                            CleanTextParam = jsonParam,
                            Url = requesturl,
                            Mobile = mobile,
                            TrackId = trackId,
                            CreatorId = creatorId
                        });

                    HttpResponseMessage response = await client.GetAsync(requesturl, cancellationToken);
                    string responseContent = await response.Content.ReadAsStringAsync();
                    if (logId > 0)
                    {
                        await neoZarinRequestSignatureLogRepository.UpdateLog(logId, responseContent, cancellationToken);
                        logUpdateSuccessed = true;
                    }
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        if (response.Content == null)
                            throw new AppException("Error in calling 'apply_sign' method or Response IS NULL");
                        else
                        {
                            if (responseContent != null)
                            {
                                var responseContentObject = JObject.Parse(responseContent);
                                if (responseContentObject.ContainsKey("hasError") && Convert.ToBoolean(responseContentObject["hasError"]) &&
                                    responseContentObject.ContainsKey("message") && responseContentObject["message"] != null)
                                {
                                    var errorMessage = responseContentObject["message"].ToString();
                                    if (responseContentObject.ContainsKey("errorCode") && responseContentObject["errorCode"] != null)
                                        errorMessage += $" - errorCode : {responseContentObject["errorCode"]}";

                                    throw new AppException(errorMessage);
                                }
                            }
                            throw new AppException(responseContent);
                        }
                    }

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject != null)
                    {
                        var result = JsonConvert.DeserializeObject<NeoZarinSignResult>(resultObject.ToString());
                        return true;
                    }
                    else
                    {
                        //logger.LogError($"SignContract result(apply_sign) is null. requestUrl : {requesturl} , jsonParam :{jsonParam}.");
                        throw new AppException($"SignContract(apply_sign) result is null. requestUrl : {requesturl} , jsonParam :{jsonParam}.");
                        //return false;
                    }
                }
            }
            catch (Exception exp)
            {
                if (logId > 0 && !logUpdateSuccessed)
                    await neoZarinRequestSignatureLogRepository.UpdateLog(logId, $"responseBody : {responseBody} , exception : {exp.Message}", cancellationToken);

                logger.LogError(exp, exp.Message);
                throw new AppException(exp.Message, exp);
            }
        }
    }
}