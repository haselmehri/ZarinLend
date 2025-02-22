using Common;
using Common.Exceptions;
using Core.Data.Repositories;
using Core.Entities;
using HttpClientToCurl;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.Model.IranCreditScoring;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Common.Enums;

namespace Services
{
    public class IranCreditScoringService : IIranCreditScoringService, IScopedDependency
    {
        private const string USER_NAME = "Payandehmehrdad@gmail.com";
        private const string PASSWORD = "eMdu4drgcr6AXtB*";
        private readonly ILogger<IranCreditScoringService> logger;
        private readonly IRequestFacilityService requestFacilityService;
        private readonly IRequestFacilityGuarantorService requestFacilityGuarantorService;
        private readonly IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository;
        private readonly IRequestFacilityGuarantorWorkFlowStepRepository requestFacilityGuarantorWorkFlowStepRepository;
        private readonly IBaseRepository<IranCreditScoring> iranCreditScoringRepository;
        private readonly IBaseRepository<IranCreditScoringDocument> iranCreditScoringDocumentRepository;
        private readonly INeginHubLogRepository neginHubLogRepository;
        private readonly IWebHostEnvironment webHostEnvironment;

        public IranCreditScoringService(ILogger<IranCreditScoringService> logger,
            IRequestFacilityService requestFacilityService,
            IRequestFacilityGuarantorService requestFacilityGuarantorService,
            IRequestFacilityWorkFlowStepRepository requestFacilityWorkFlowStepRepository,
            IRequestFacilityGuarantorWorkFlowStepRepository requestFacilityGuarantorWorkFlowStepRepository,
            IBaseRepository<IranCreditScoring> iranCreditScoringRepository,
            IBaseRepository<IranCreditScoringDocument> iranCreditScoringDocumentRepository,
            INeginHubLogRepository neginHubLogRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            this.logger = logger;
            this.requestFacilityService = requestFacilityService;
            this.requestFacilityGuarantorService = requestFacilityGuarantorService;
            this.requestFacilityWorkFlowStepRepository = requestFacilityWorkFlowStepRepository;
            this.requestFacilityGuarantorWorkFlowStepRepository = requestFacilityGuarantorWorkFlowStepRepository;
            this.iranCreditScoringRepository = iranCreditScoringRepository;
            this.iranCreditScoringDocumentRepository = iranCreditScoringDocumentRepository;
            this.neginHubLogRepository = neginHubLogRepository;
            this.webHostEnvironment = webHostEnvironment;
        }

        public virtual async Task<bool> GoToNextStepFromVerifyPaymentToZarinLend(Guid userId, int requestFacilityId, CancellationToken cancellationToken = default)
        {
            await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId, WorkFlowFormEnum.PaymentVerifyShahkarAndSamatService,
                    StatusEnum.Approved, userId, userId, "تایید اتوماتیک بدون پرداخت واعتبارسنجی", cancellationToken: cancellationToken);

            await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId, WorkFlowFormEnum.VerifyShahkarAndSamatService,
                    StatusEnum.Approved, userId, userId, "تایید اتوماتیک بدون پرداخت واعتبارسنجی", cancellationToken: cancellationToken);

            await requestFacilityWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityId, WorkFlowFormEnum.VerifyIranCreditScoring,
                    StatusEnum.Approved, buyerId: userId, opratorId: userId,
                    statusDescription: "تایید اتوماتیک بدون پرداخت واعتبارسنجی", cancellationToken: cancellationToken);

            return true;
        }

        //private async Task ApprovedVerifyIranCreditScoreStepByGuarantor(Guid userId, int requestFacilityGuarantorId, CancellationToken cancellationToken)
        //{
        //    if (await requestFacilityGuarantorService
        //       .CheckRequestFacilityWaitingSpecifiedStepAndRole(userId, requestFacilityGuarantorId, new List<RoleEnum> { RoleEnum.Buyer }, WorkFlowFormEnum.VerifyIranCreditScoringByGuarantor,
        //       cancellationToken))
        //    {
        //        await requestFacilityGuarantorWorkFlowStepRepository.ChangeCurrentStepAndGoToNextStep(requestFacilityGuarantorId, WorkFlowFormEnum.VerifyIranCreditScoringByGuarantor,
        //            StatusEnum.Approved,
        //            guarantorUserId: userId,
        //            opratorId: userId,
        //            statusDescription: "تایید اتوماتیک بعد از دریافت نتیجه ایرانیان",
        //            cancellationToken: cancellationToken);
        //    }
        //}

        private async Task ApprovedVerifyIranCreditScoreStep(Guid userId, int requestFacilityId, CancellationToken cancellationToken)
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

        public async Task<bool> ExistVerifyResult(int requestFacilityId, int expireAfterFewDays = 30, CancellationToken cancellationToken = default)
        {
            return await iranCreditScoringRepository.TableNoTracking
                                                    .AnyAsync(p => p.RequestFacilityId.Equals(requestFacilityId) &&
                                                                   p.CreatedDate.AddDays(expireAfterFewDays).Date >= DateTime.Now.Date, cancellationToken);
        }

        public async Task<bool> ExistVerifyResult(Guid userId, int expireAfterFewDays = 30, CancellationToken cancellationToken = default)
        {
            return await iranCreditScoringRepository.TableNoTracking
                                                    .AnyAsync(p => p.CreatorId.Equals(userId) &&
                                                                   p.CreatedDate.AddDays(expireAfterFewDays).Date >= DateTime.Now.Date, cancellationToken);
        }
        public async Task<IranCreditScoringModel> GetVerifyResult(int requestFacilityId, CancellationToken cancellationToken = default)
        {
            var result = await iranCreditScoringRepository.TableNoTracking
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
                .FirstOrDefaultAsync(cancellationToken);

            return result;
        }
        public async Task<IranCreditScoringModel> GetVerifyResult(Guid userId, CancellationToken cancellationToken = default)
        {
            var result = await iranCreditScoringRepository.TableNoTracking
                .Where(p => p.CreatorId.Equals(userId))
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
        public async Task<IranCreditScoringModel> GetVerifyResultByRequestFacilityGaurantor(int requestFacilityGuarantorId, CancellationToken cancellationToken = default)
        {
            var result = (await iranCreditScoringRepository.TableNoTracking
                .Where(p => p.RequestFacilityGuarantorId.Equals(requestFacilityGuarantorId))
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
                Description = model.Description,
                Risk = model.Risk,
                Score = model.Score,

            };

            if (model.IranCreditScoringRequestType == IranCreditScoringRequestType.ForFacilityRequest)
                iranCreditScoring.RequestFacilityId = model.RequestId;
            else if (model.IranCreditScoringRequestType == IranCreditScoringRequestType.ForFacilityRequestGuarantor)
                iranCreditScoring.RequestFacilityGuarantorId = model.RequestId;

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

            if (model.IranCreditScoringRequestType == IranCreditScoringRequestType.ForFacilityRequest)
                await ApprovedVerifyIranCreditScoreStep(userId, model.RequestId, cancellationToken);
            //else if (model.IranCreditScoringRequestType == IranCreditScoringRequestType.ForFacilityRequestGuarantor)
            //    await ApprovedVerifyIranCreditScoreStepByGuarantor(userId, model.RequestId, cancellationToken);

            return model;
        }

        #region base services
        public async Task<string?> Request(RequestModel model, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetToken();
            if (token == null) throw new AppException("return result from 'token' is null!");
            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                string requestUrl = $"https://api.neginhub.com/api/v1/CreditScore/Request";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
                {
                    var trackId = Guid.NewGuid().ToString();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var body = JsonConvert.SerializeObject(
                        new
                        {
                            mobile = model.MobileNumber,
                            real_national_code = model.RealPersonNationalCode,
                            track_id = trackId
                        });
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                    var curl = client.GenerateCurlInString(request);
                    logId = await neginHubLogRepository
                      .AddLog(new NeginHubLog()
                      {
                          Body = body,
                          Url = requestUrl,
                          ServiceName = NeginHubServiceName.CreditScore_Request.ToString(),
                          TrackId = trackId,
                          MethodType = MethodType.Post,
                          OpratorId = creator,
                          Curl = curl
                      });

                    HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        return null;

                    responseBody = await response.Content.ReadAsStringAsync();

                    if (logId > 0)
                    {
                        await neginHubLogRepository.UpdateLog(logId, responseBody);
                        logUpdateSuccessed = true;
                    }

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("data") &&
                      resultObject["data"] != null)
                    {
                        return resultObject["data"]!.ToString();
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
                            logger.LogWarning($"'Request' Method : LegalPersonNationalCode : {model.LegalPersonNationalCode}, MobileNumber : {model.MobileNumber}, RealPersonNationalCode : {model.RealPersonNationalCode}. unexpected error!");

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

        public async Task<string?> Validate(string hashCode, string otp, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetToken();
            if (token == null) throw new AppException("return result from 'token' is null!");
            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                string requestUrl = $"https://api.neginhub.com/api/v1/CreditScore/Validate";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUrl))
                {
                    var trackId = Guid.NewGuid().ToString();

                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var body = JsonConvert.SerializeObject(
                         new
                         {
                             hash_code = hashCode,
                             otp = otp,
                             track_id = trackId
                         });
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                    var curl = client.GenerateCurlInString(request);
                    logId = await neginHubLogRepository
                      .AddLog(new NeginHubLog()
                      {
                          Body = body,
                          Url = requestUrl,
                          ServiceName = NeginHubServiceName.CreditScore_Validate.ToString(),
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

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        return null;

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("data") &&
                        resultObject["data"] != null)
                    {
                        return resultObject["data"]!.ToString();
                    }
                    else if (resultObject.ContainsKey("meta") &&
                            resultObject["meta"] != null)
                    {
                        var error = string.Empty;
                        if (((JObject)resultObject["meta"]!).ContainsKey("error_type"))
                            error = $"error_type : ${((JObject)resultObject["meta"]!)["error_type"]}";

                        if (((JObject)resultObject["meta"]!).ContainsKey("error_message"))
                            error += $"-error_message : ${((JObject)resultObject["meta"]!)["error_message"]}";

                        if (((JObject)resultObject["meta"]!).ContainsKey("code"))
                            error += $"-code : ${((JObject)resultObject["meta"]!)["code"]}";

                        logger.LogError(error);

                        if (error != string.Empty)
                            throw new AppException(error);
                    }

                    return null;
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

        public async Task<StatusResultModel?> Status(string hashCode, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetToken();
            if (token == null) throw new AppException("return result from 'token' is null!");
            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                string requestUrl = $"https://api.neginhub.com/api/v1/CreditScore/Status?hashCode={hashCode}";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUrl))
                {
                    var trackId = Guid.NewGuid().ToString();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var curl = client.GenerateCurlInString(request);
                    logId = await neginHubLogRepository
                     .AddLog(new NeginHubLog()
                     {
                         Url = requestUrl,
                         ServiceName = NeginHubServiceName.CreditScore_Status.ToString(),
                         TrackId = trackId,
                         MethodType = MethodType.Get,
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

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        return null;

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("data") &&
                        resultObject["data"] != null)
                    {
                        return JsonConvert.DeserializeObject<StatusResultModel>(resultObject["data"]!.ToString());
                    }
                    else if (resultObject.ContainsKey("meta") &&
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

                        if (error != string.Empty)
                            throw new AppException(error);
                    }

                    return null;
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

        public async Task<string?> PdfReport(string reportCode, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetToken();
            if (token == null) throw new AppException("return result from 'token' is null!");
            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                string requestUrl = $"https://api.neginhub.com/api/v1/CreditScore/Pdf?reportCode={reportCode}";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUrl))
                {
                    var trackId = Guid.NewGuid().ToString();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var curl = client.GenerateCurlInString(request);
                    logId = await neginHubLogRepository
                        .AddLog(new NeginHubLog()
                        {
                            Url = requestUrl,
                            ServiceName = NeginHubServiceName.CreditScore_Pdf.ToString(),
                            TrackId = trackId,
                            MethodType = MethodType.Get,
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

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        return null;

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("data") &&
                        resultObject["data"] != null)
                    {
                        //return pdf file to base64 format
                        return resultObject["data"]!.ToString();
                    }
                    else if (resultObject.ContainsKey("meta") ||
                             resultObject["meta"] != null)
                    {
                        var error = string.Empty;
                        if (((JObject)resultObject["meta"]!).ContainsKey("error_type"))
                            error = $"error_type : ${((JObject)resultObject["meta"]!)["error_type"]}";

                        if (((JObject)resultObject["meta"]!).ContainsKey("error_message"))
                            error += $"-error_message : ${((JObject)resultObject["meta"]!)["error_message"]}";

                        if (((JObject)resultObject["meta"]!).ContainsKey("code"))
                            error += $"-code : ${((JObject)resultObject["meta"]!)["code"]}";

                        if (error != string.Empty)
                            throw new AppException(error);
                    }
                    return null;
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

        public async Task<string?> Json(string reportCode, Guid? creator, CancellationToken cancellationToken)
        {
            var token = await GetToken();
            if (token == null) throw new AppException("return result from 'token' is null!");
            string responseBody = string.Empty;
            long logId = 0;
            var logUpdateSuccessed = false;
            try
            {
                string requestUrl = $"https://api.neginhub.com/api/v1/CreditScore/Json?reportCode={reportCode}";
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUrl))
                {
                    var trackId = Guid.NewGuid().ToString();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var curl = client.GenerateCurlInString(request);
                    logId = await neginHubLogRepository
                        .AddLog(new NeginHubLog()
                        {
                            Url = requestUrl,
                            ServiceName = NeginHubServiceName.CreditScore_Json.ToString(),
                            TrackId = trackId,
                            MethodType = MethodType.Get,
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

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        return null;

                    JObject resultObject = JObject.Parse(responseBody);

                    if (resultObject.ContainsKey("data") &&
                        resultObject["data"] != null)
                    {
                        //return pdf file to base64 format
                        return resultObject["data"]!.ToString();
                    }
                    else if (resultObject.ContainsKey("meta") &&
                             resultObject["meta"] != null)
                    {
                        var error = string.Empty;
                        if (((JObject)resultObject["meta"]!).ContainsKey("error_type"))
                            error = $"error_type : ${((JObject)resultObject["meta"]!)["error_type"]}";

                        if (((JObject)resultObject["meta"]!).ContainsKey("error_message"))
                            error += $"-error_message : ${((JObject)resultObject["meta"]!)["error_message"]}";

                        if (((JObject)resultObject["meta"]!).ContainsKey("code"))
                            error += $"-code : ${((JObject)resultObject["meta"]!)["code"]}";

                        if (error != string.Empty)
                            throw new AppException(error);
                    }
                    return null;
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

        private async Task<string?> GetToken()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string requesturl = "https://api.neginhub.com/api/v1/Users/getToken";
                    var grant_type = "password";

                    var body = JsonConvert.SerializeObject(new { grant_type, Username = USER_NAME, Password = PASSWORD });
                    client.DefaultRequestHeaders.Add("Authorization", $"Basic Og==");

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
                logger.LogError(exp, exp.Message);
            }
            return null;
        }
        private async Task<string?> RefreshToken(string refreshToken)
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
