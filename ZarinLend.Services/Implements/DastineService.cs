using Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class DastineService : IDastineService, IScopedDependency
    {
        private const string BaseUrl = "https://dss.pki.co.ir";
        private readonly ILogger<DastineService> logger;
        private readonly IWebHostEnvironment webHostEnvironment;

        public DastineService(ILogger<DastineService> logger, IWebHostEnvironment webHostEnvironment)
        {
            this.logger = logger;
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> PDFDigestForMultiSign(string pdfData, string signerCertificate, string signatureFieldName, Guid creatorId, CancellationToken cancellationToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string requesturl = $"{BaseUrl}/api/CryptoService/PDFDigestForMultiSign";
                    var jsonParam = JsonConvert.SerializeObject(
                        new
                        {
                            pdfData,
                            signerCertificate,
                            certificationLevel = 0,
                            dateTime = string.Format("{0:s}", DateTime.Now.Date),
                            reason = "sign contract",
                            location = "location",
                            imageDataUrl = "",
                            page = 1,
                            lowerLeftX = 0,
                            lowerLeftY = 0,
                            upperRightX = 0,
                            upperRightY = 0,
                            signatureFieldName = signatureFieldName,
                            hashAlgorithm = 0
                        });

                    HttpContent content = new StringContent(jsonParam, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(requesturl, content, cancellationToken);


                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        logger.LogError(response.Content != null ? response.Content.ToString() : "Error in calling 'PDFDigestForMultiSign' method");

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject != null)
                    {
                        //var result = JsonConvert.DeserializeObject<NeoZarinSignResult>(resultObject.ToString());
                        //await neoZarinRequestSignatureLogRepository.AddAsync(new NeoZarinRequestSignatureLog()
                        //{
                        //    CleanTextParam = jsonParam,
                        //    HasError = result.HasError,
                        //    CreatorId = creatorId,
                        //    Mobile = mobile,
                        //    RequestFacilityId = requestFacilityId,
                        //    RequestUrl = requesturl,
                        //    ResponseMessage = resultObject.ToString(),
                        //    TrackId = trackId,
                        //}, cancellationToken);
                        return resultObject["result"].ToString();
                    }
                    else
                    {
                        logger.LogWarning($"PDFDigestForMultiSign result is null. requestUrl : {requesturl} , jsonParam :{jsonParam}.");
                        return null;
                    }
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return null;
            }
        }

        public async Task<string> PutPDFSignatureForMultiSign(string pdfData, string signerCertificate,string signature, string signatureFieldName, Guid creatorId, CancellationToken cancellationToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string requesturl = $"{BaseUrl}/api/CryptoService/PutPDFSignatureForMultiSign";
                    var jsonParam = JsonConvert.SerializeObject(
                        new
                        {
                            pdfData,
                            signerCertificate,
                            certificationLevel = 0,
                            dateTime = string.Format("{0:s}", DateTime.Now.Date),
                            reason = "sign contract",
                            location = "location",
                            imageDataUrl = "",
                            page = 1,
                            lowerLeftX = 0,
                            lowerLeftY = 0,
                            upperRightX = 0,
                            upperRightY = 0,
                            signatureFieldName = signatureFieldName,
                            hashAlgorithm = 0,
                            signature = signature
                        });

                    HttpContent content = new StringContent(jsonParam, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(requesturl, content, cancellationToken);


                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        logger.LogError(response.Content != null ? response.Content.ToString() : "Error in calling 'PutPDFSignatureForMultiSign' method");

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject != null)
                    {
                        //var result = JsonConvert.DeserializeObject<NeoZarinSignResult>(resultObject.ToString());
                        //await neoZarinRequestSignatureLogRepository.AddAsync(new NeoZarinRequestSignatureLog()
                        //{
                        //    CleanTextParam = jsonParam,
                        //    HasError = result.HasError,
                        //    CreatorId = creatorId,
                        //    Mobile = mobile,
                        //    RequestFacilityId = requestFacilityId,
                        //    RequestUrl = requesturl,
                        //    ResponseMessage = resultObject.ToString(),
                        //    TrackId = trackId,
                        //}, cancellationToken);
                        return resultObject["result"].ToString();
                    }
                    else
                    {
                        logger.LogWarning($"PutPDFSignatureForMultiSign result is null. requestUrl : {requesturl} , jsonParam :{jsonParam}.");
                        return null;
                    }
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return null;
            }
        }        
    }
}