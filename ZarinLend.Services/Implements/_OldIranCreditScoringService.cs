using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.Model.IranCreditScoring;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class _OldIranCreditScoringService : _OldIIranCreditScoringService, IScopedDependency
    {
        private const string USER_NAME = "Hiradzarin";
        private const string PASSWORD = "h1D@Z$jp";
        private readonly ILogger<_OldIranCreditScoringService> logger;
        private readonly IRequestFacilityService requestFacilityService;
        private readonly IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository;
        private readonly IRepository<IranCreditScoring> iranCreditScoringRepository;
        private readonly IRepository<IranCreditScoringDocument> iranCreditScoringDocumentRepository;
        private readonly IWebHostEnvironment webHostEnvironment;

        public _OldIranCreditScoringService(ILogger<_OldIranCreditScoringService> logger,
            IRequestFacilityService requestFacilityService,
            IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository,
            IRepository<IranCreditScoring> iranCreditScoringRepository,
            IRepository<IranCreditScoringDocument> iranCreditScoringDocumentRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            this.logger = logger;
            this.requestFacilityService = requestFacilityService;
            this.requestFacilityWorkFlowStepRepository = requestFacilityWorkFlowStepRepository;
            this.iranCreditScoringRepository = iranCreditScoringRepository;
            this.iranCreditScoringDocumentRepository = iranCreditScoringDocumentRepository;
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task ApprovedVerifyIranCreditScoreStep(Guid userId, int requestFacilityId, CancellationToken cancellationToken)
        {
            if (await requestFacilityService
               .CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.VerifyIranCreditScoring,
               cancellationToken))
            {
                await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId, WorkFlowFormEnum.VerifyIranCreditScoring,
                    StatusEnum.Approved,
                    buyerId: userId,
                    opratorId: userId,
                    statusDescription: "تایید اتوماتیک بعد از دریافت نتیجه ایرانیان",
                    cancellationToken: cancellationToken);
            }
        }

        public async Task<IranCreditScoringModel> GetVerifyResult(int requestFacilityId, CancellationToken cancellationToken = default)
        {
            var result = (await iranCreditScoringRepository.TableNoTracking
                .Where(p => p.RequestFacilityId.Equals(requestFacilityId))
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new IranCreditScoringModel
                {
                    Score = p.Score,
                    Risk = p.Risk,
                    Description = p.Description,
                    CreateDate = p.CreatedDate,
                    PdfUrl = p.IranCreditScoringDocuments.Any(x => x.DocumentType == DocumentType.IranCreditScoringPDF && !x.IsDeleted) ?
                                          p.IranCreditScoringDocuments.First(x => x.DocumentType == DocumentType.IranCreditScoringPDF && !x.IsDeleted).FilePath :
                                          null,
                    XmlUrl = p.IranCreditScoringDocuments.Any(x => x.DocumentType == DocumentType.IranCreditScoringXML && !x.IsDeleted) ?
                                          p.IranCreditScoringDocuments.First(x => x.DocumentType == DocumentType.IranCreditScoringXML && !x.IsDeleted).FilePath :
                                          null,
                    JsonUrl = p.IranCreditScoringDocuments.Any(x => x.DocumentType == DocumentType.IranCreditScoringJSON && !x.IsDeleted) ?
                                          p.IranCreditScoringDocuments.First(x => x.DocumentType == DocumentType.IranCreditScoringJSON && !x.IsDeleted).FilePath :
                                          null,
                })
                .FirstOrDefaultAsync());

            return result;
        }

        public async Task<IranCreditScoringModel> GetVerifyResult(Guid userId, CancellationToken cancellationToken = default)
        {
            var result = await iranCreditScoringRepository.TableNoTracking
                .Where(p => p.RequestFacility.BuyerId.Equals(userId))
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new IranCreditScoringModel
                {
                    CreateDate = p.CreatedDate,
                    Score = p.Score,
                    Risk = p.Risk,
                    Description = p.Description,
                    PdfUrl = p.IranCreditScoringDocuments.Any(x => x.DocumentType == DocumentType.IranCreditScoringPDF && !x.IsDeleted) ?
                                          p.IranCreditScoringDocuments.First(x => x.DocumentType == DocumentType.IranCreditScoringPDF && !x.IsDeleted).FilePath :
                                          null,
                    XmlUrl = p.IranCreditScoringDocuments.Any(x => x.DocumentType == DocumentType.IranCreditScoringXML && !x.IsDeleted) ?
                                          p.IranCreditScoringDocuments.First(x => x.DocumentType == DocumentType.IranCreditScoringXML && !x.IsDeleted).FilePath :
                                          null,
                    JsonUrl = p.IranCreditScoringDocuments.Any(x => x.DocumentType == DocumentType.IranCreditScoringJSON && !x.IsDeleted) ?
                                          p.IranCreditScoringDocuments.First(x => x.DocumentType == DocumentType.IranCreditScoringJSON && !x.IsDeleted).FilePath :
                                          null,
                })
                .FirstOrDefaultAsync(cancellationToken);

            return result;
        }

        public async Task<IranCreditScoringModel> SaveVerify(Guid userId, IranCreditScoringModel model, CancellationToken cancellationToken = default)
        {
            if (model == null)
            {
                logger.LogError("'model' is null");
                return null;
            }
            var iranCreditScoringPath = @"UploadFiles\IranCreditScoring";
            string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, iranCreditScoringPath);

            #region Save File On Disk & Insert Record In Database
            var iranCreditScoring = new IranCreditScoring()
            {
                CreatorId = userId,
                RequestFacilityId = model.RequestId,
                Description = model.Description,
                Risk = model.Risk,
                Score = model.Score,

            };
            await iranCreditScoringRepository.AddAsync(iranCreditScoring, cancellationToken);
            model.CreateDate = iranCreditScoring.CreatedDate;

            const string baseRelativePath = "/UploadFiles/IranCreditScoring";
            #region save Pdf
            if (!string.IsNullOrEmpty(model.PdfBase64String))
            {
                var fileName = $"{Guid.NewGuid()}.pdf";
                var relativePath = $"{baseRelativePath}/{fileName}";
                string filePath = Path.Combine(uploadFolder, fileName);
                using var stream = File.Create(filePath);
                await stream.WriteAsync(Convert.FromBase64String(model.PdfBase64String));
                model.PdfUrl = relativePath;

                #region Add File Info To DB
                await iranCreditScoringDocumentRepository.AddAsync(
                    new IranCreditScoringDocument()
                    {
                        DocumentType = DocumentType.IranCreditScoringPDF,
                        FilePath = relativePath,
                        Status = DocumentStatus.Active,
                        FileType = FileType.Pdf,
                        IranCreditScoringId = iranCreditScoring.Id
                    }, cancellationToken);
                #endregion
            }
            #endregion save Pdf

            #region save Json
            if (!string.IsNullOrEmpty(model.Json))
            {
                var fileName = $"{Guid.NewGuid()}.json";
                var relativePath = $"{baseRelativePath}/{fileName}";
                string filePath = Path.Combine(uploadFolder, fileName);
                using var stream = File.Create(filePath);
                await stream.WriteAsync(System.Text.Encoding.UTF8.GetBytes(model.Json));
                model.JsonUrl = relativePath;

                #region Add File Info To DB
                await iranCreditScoringDocumentRepository.AddAsync(
                    new IranCreditScoringDocument()
                    {
                        DocumentType = DocumentType.IranCreditScoringJSON,
                        FilePath = relativePath,
                        Status = DocumentStatus.Active,
                        FileType = FileType.Json,
                        IranCreditScoringId = iranCreditScoring.Id
                    }, cancellationToken);
                #endregion
            }
            #endregion save Json

            #region save Xml
            if (!string.IsNullOrEmpty(model.Json))
            {
                var fileName = $"{Guid.NewGuid()}.xml";
                var relativePath = $"{baseRelativePath}/{fileName}";
                string filePath = Path.Combine(uploadFolder, fileName);
                using var stream = File.Create(filePath);
                await stream.WriteAsync(System.Text.Encoding.UTF8.GetBytes(model.Xml));
                model.XmlUrl = relativePath;

                #region Add File Info To DB
                await iranCreditScoringDocumentRepository.AddAsync(
                    new IranCreditScoringDocument()
                    {
                        DocumentType = DocumentType.IranCreditScoringXML,
                        FilePath = relativePath,
                        Status = DocumentStatus.Active,
                        FileType = FileType.Xml,
                        IranCreditScoringId = iranCreditScoring.Id
                    }, cancellationToken);
                #endregion
            }
            #endregion save Xml

            #endregion

            await ApprovedVerifyIranCreditScoreStep(userId, model.RequestId, cancellationToken);

            return model;
        }

        #region base services
        public async Task<string> Request(RequestModel model, TokenModel token, CancellationToken cancellationToken)
        {
            //var token = await GetToken(cancellationToken);
            if (token == null) throw new AppException("return result from 'token' is null!");
            try
            {
                using (var client = new HttpClient())
                {
                    var trackId = Guid.NewGuid().ToString();
                    string requesturl = $"https://app.ics24.ir/b2b/api/request";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
                    client.DefaultRequestHeaders.Add("x-apikey", $"{token.ApiKey}");
                    client.DefaultRequestHeaders.Add("x-version", $"2.0");
                    var data = new[]
                    {
                        //new KeyValuePair<string, string>("LegalPersonNationalCode", model.LegalPersonNationalCode),
                        new KeyValuePair<string, string>("MobileNumber", model.MobileNumber),
                        new KeyValuePair<string, string>("RealPersonNationalCode", model.RealPersonNationalCode)
                    };

                    HttpResponseMessage response = await client.PostAsync(requesturl, content: new FormUrlEncodedContent(data), cancellationToken);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        return null;

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("hasError") &&
                      resultObject["hasError"].ToString().Equals("false", StringComparison.CurrentCultureIgnoreCase) &&
                      resultObject.ContainsKey("data") &&
                      resultObject["data"] != null)
                    {
                        return resultObject["data"].ToString();
                    }
                    else
                    {
                        if (resultObject.ContainsKey("messages") &&
                           resultObject["messages"].GetType() == typeof(JArray) &&
                           ((JArray)resultObject["messages"]).Count > 0)
                        {
                            var errors = "'Request' method return errors => ";
                            foreach (var item in (JArray)resultObject["messages"])
                            {
                                errors += $"reason : {item["reason"]},message : {item["message"]} ; ";
                            }
                            logger.LogError(errors);
                        }
                        else
                            logger.LogWarning($"Request : LegalPersonNationalCode : {model.LegalPersonNationalCode}, MobileNumber : {model.MobileNumber}, RealPersonNationalCode : {model.RealPersonNationalCode}. unexpected error!");

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

        public async Task<Tuple<string, JObject>> Validate(string hashCode, string otp, TokenModel token, CancellationToken cancellationToken)
        {
            //var token = await GetToken(cancellationToken);
            if (token == null) throw new AppException("return result from 'token' is null!");
            try
            {
                using (var client = new HttpClient())
                {
                    var trackId = Guid.NewGuid().ToString();
                    string requesturl = $"https://app.ics24.ir/b2b/api/request/{hashCode}/validate";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
                    client.DefaultRequestHeaders.Add("x-apikey", $"{token.ApiKey}");
                    client.DefaultRequestHeaders.Add("x-version", $"2.0");
                    var data = new[]
                    {
                        //new KeyValuePair<string, string>("LegalPersonNationalCode", model.LegalPersonNationalCode),
                        new KeyValuePair<string, string>("Token", otp)
                    };

                    HttpResponseMessage response = await client.PostAsync(requesturl, content: new FormUrlEncodedContent(data), cancellationToken);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        return null;

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("hasError") &&
                      resultObject["hasError"].ToString().Equals("false", StringComparison.CurrentCultureIgnoreCase) &&
                      resultObject.ContainsKey("data") &&
                      resultObject["data"] != null)
                    {
                        return new Tuple<string, JObject>(resultObject["data"].ToString(), null);
                    }
                    else
                    {
                        if (resultObject.ContainsKey("messages") &&
                           resultObject["messages"].GetType() == typeof(JArray) &&
                           ((JArray)resultObject["messages"]).Count > 0)
                        {
                            var errors = "'Validate' method return errors => ";
                            foreach (var item in (JArray)resultObject["messages"])
                            {
                                errors += $"reason : {item["reason"]},message : {item["message"]} ; ";
                            }
                            logger.LogError(errors);
                        }
                        else
                            logger.LogError($"Validate . unexpected error!");

                        return new Tuple<string, JObject>(null, resultObject);
                    }
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, exp.Message);
                return new Tuple<string, JObject>(null, null);
            }
        }

        public async Task<StatusResultModel> Status(string hashCode, TokenModel token, CancellationToken cancellationToken)
        {
            //var token = await GetToken(cancellationToken);
            if (token == null) throw new AppException("return result from 'token' is null!");
            try
            {
                using (var client = new HttpClient())
                {
                    var trackId = Guid.NewGuid().ToString();
                    string requesturl = $"https://app.ics24.ir/b2b/api/request/{hashCode}/Status";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
                    client.DefaultRequestHeaders.Add("x-apikey", $"{token.ApiKey}");
                    client.DefaultRequestHeaders.Add("x-version", $"2.0");

                    HttpResponseMessage response = await client.GetAsync(requesturl, cancellationToken);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        return null;

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("hasError") &&
                      resultObject["hasError"].ToString().Equals("false", StringComparison.CurrentCultureIgnoreCase) &&
                      resultObject.ContainsKey("data") &&
                      resultObject["data"] != null)
                    {
                        return JsonConvert.DeserializeObject<StatusResultModel>(resultObject["data"].ToString());
                    }
                    else
                    {
                        if (resultObject.ContainsKey("messages") &&
                            resultObject["messages"].GetType() == typeof(JArray) &&
                            ((JArray)resultObject["messages"]).Count > 0)
                        {
                            var errors = "'Status' method return errors => ";
                            foreach (var item in (JArray)resultObject["messages"])
                            {
                                errors += $"reason : {item["reason"]},message : {item["message"]} ; ";
                            }
                            logger.LogError(errors);
                        }
                        else
                            logger.LogError($"Status . unexpected error!");

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

        public async Task<string> PdfReport(string reportCode, TokenModel token, CancellationToken cancellationToken)
        {
            //var token = await GetToken(cancellationToken);
            if (token == null) throw new AppException("return result from 'token' is null!");
            try
            {
                using (var client = new HttpClient())
                {
                    var trackId = Guid.NewGuid().ToString();
                    string requesturl = $"https://app.ics24.ir/b2b/api/request/{reportCode}/pdfReport";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
                    //client.DefaultRequestHeaders.Add("x-apikey", $"{token.ApiKey}");
                    //client.DefaultRequestHeaders.Add("x-version", $"2.0");

                    HttpResponseMessage response = await client.GetAsync(requesturl, cancellationToken);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        return null;

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("hasError") &&
                      resultObject["hasError"].ToString().Equals("false", StringComparison.CurrentCultureIgnoreCase) &&
                      resultObject.ContainsKey("data") &&
                      resultObject["data"] != null)
                    {
                        //return pdf file to base64 format
                        return resultObject["data"].ToString();
                    }
                    else
                    {
                        if (resultObject.ContainsKey("messages") &&
                            resultObject["messages"].GetType() == typeof(JArray) &&
                            ((JArray)resultObject["messages"]).Count > 0)
                        {
                            var errors = "'PdfReport' method return errors => ";
                            foreach (var item in (JArray)resultObject["messages"])
                            {
                                errors += $"reason : {item["reason"]},message : {item["message"]} ; ";
                            }
                            logger.LogError(errors);
                        }
                        else
                            logger.LogError($"PdfReport . unexpected error!");

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

        public async Task<string> Xml(string reportCode, TokenModel token, CancellationToken cancellationToken)
        {
            //var token = await GetToken(cancellationToken);
            if (token == null) throw new AppException("return result from 'token' is null!");
            try
            {
                using (var client = new HttpClient())
                {
                    var trackId = Guid.NewGuid().ToString();
                    string requesturl = $"https://app.ics24.ir/report/{reportCode}/xml";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
                    client.DefaultRequestHeaders.Add("x-apikey", $"{token.ApiKey}");
                    client.DefaultRequestHeaders.Add("x-version", $"2.0");

                    HttpResponseMessage response = await client.GetAsync(requesturl, cancellationToken);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        return null;

                    string responseBody = await response.Content.ReadAsStringAsync();
                    if (response.Content.Headers.ContentType.MediaType == "application/xml")
                        return responseBody;

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("hasError") &&
                      resultObject["hasError"].ToString().Equals("false", StringComparison.CurrentCultureIgnoreCase) &&
                      resultObject.ContainsKey("data") &&
                      resultObject["data"] != null)
                    {
                        //return pdf file to base64 format
                        return resultObject["data"].ToString();
                    }
                    else
                    {
                        if (resultObject.ContainsKey("messages") &&
                            resultObject["messages"].GetType() == typeof(JArray) &&
                            ((JArray)resultObject["messages"]).Count > 0)
                        {
                            var errors = "'PdfReport' method return errors => ";
                            foreach (var item in (JArray)resultObject["messages"])
                            {
                                errors += $"reason : {item["reason"]},message : {item["message"]} ; ";
                            }
                            logger.LogError(errors);
                        }
                        else
                            logger.LogError($"PdfReport . unexpected error!");

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

        public async Task<string> Json(string reportCode, TokenModel token, CancellationToken cancellationToken)
        {
            //var token = await GetToken(cancellationToken);
            if (token == null) throw new AppException("return result from 'token' is null!");
            try
            {
                using (var client = new HttpClient())
                {
                    var trackId = Guid.NewGuid().ToString();
                    string requesturl = $"https://app.ics24.ir/report/{reportCode}/json";
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.AccessToken}");
                    client.DefaultRequestHeaders.Add("x-apikey", $"{token.ApiKey}");
                    client.DefaultRequestHeaders.Add("x-version", $"2.0");

                    HttpResponseMessage response = await client.GetAsync(requesturl, cancellationToken);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        return null;

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("hasError") &&
                      resultObject["hasError"].ToString().Equals("false", StringComparison.CurrentCultureIgnoreCase) &&
                      resultObject.ContainsKey("data") &&
                      resultObject["data"] != null)
                    {
                        //return pdf file to base64 format
                        return resultObject["data"].ToString();
                    }
                    else
                    {
                        if (resultObject.ContainsKey("messages") &&
                            resultObject["messages"].GetType() == typeof(JArray) &&
                            ((JArray)resultObject["messages"]).Count > 0)
                        {
                            var errors = "'Json' method return errors => ";
                            foreach (var item in (JArray)resultObject["messages"])
                            {
                                errors += $"reason : {item["reason"]},message : {item["message"]} ; ";
                            }
                            logger.LogError(errors);
                        }
                        else
                            logger.LogError($"PdfReport . unexpected error!");

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

        public async Task<TokenModel> GetToken(CancellationToken cancellationToken = default)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string requesturl = "https://app.ics24.ir/connect/token";

                    var data = new[]
                    {
                        new KeyValuePair<string, string>("Username", USER_NAME),
                        new KeyValuePair<string, string>("Password", PASSWORD),
                    };
                    //client.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
                    HttpResponseMessage response = await client.PostAsync(requesturl, content: new FormUrlEncodedContent(data), cancellationToken: cancellationToken);

                    string responseBody = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        return null;

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("hasError") &&
                        resultObject["hasError"].ToString().Equals("false", StringComparison.CurrentCultureIgnoreCase) &&
                        resultObject.ContainsKey("data") &&
                        resultObject["data"] != null)
                    {
                        return JsonConvert.DeserializeObject<TokenModel>(resultObject["data"].ToString());
                    }
                    else
                    {
                        if (resultObject.ContainsKey("messages") &&
                            resultObject["messages"].GetType() == typeof(JArray) &&
                            ((JArray)resultObject["messages"]).Count > 0)
                        {
                            var errors = "'GetToken' method return errors => ";
                            foreach (var item in (JArray)resultObject["messages"])
                            {
                                errors += $"reason : {item["reason"]},message : {item["message"]} ; ";
                            }
                            logger.LogError(errors);
                        }
                        else
                            logger.LogError($"GetToken . unexpected error!");
                    }
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
