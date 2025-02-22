using Common.Utilities;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebFramework.Api;

namespace Web.UI.Controllers
{
    public class FileController : BaseMvcController
    {
        private readonly ILogger<FileController> logger;
        private readonly INeoZarinCallBackSignatureLogService neoZarinCallBackSignatureLogService;
        private readonly IRequestFacilityService requestFacilityService;
        private readonly IAyandehSignService ayandehSignService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ISmsService smsService;

        public FileController(ILogger<FileController> logger, INeoZarinCallBackSignatureLogService neoZarinCallBackSignatureLogService,
           IRequestFacilityService requestFacilityService, IAyandehSignService ayandehSignService,
           IWebHostEnvironment webHostEnvironment, ISmsService smsService)
        {
            this.logger = logger;
            this.neoZarinCallBackSignatureLogService = neoZarinCallBackSignatureLogService;
            this.requestFacilityService = requestFacilityService;
            this.ayandehSignService = ayandehSignService;
            this.webHostEnvironment = webHostEnvironment;
            this.smsService = smsService;
        }

        [AllowAnonymous]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}/{fileName}")]
        public async Task<IActionResult> ContractForSignature(int requestFacilityId, string fileName)
        {
            logger.LogInformation($"fileName : {fileName}");
            logger.LogInformation($"requestFacilityId : {requestFacilityId}");
            var filepath = Path.Combine($@"{this.webHostEnvironment.WebRootPath}\UploadFiles\RequestFacilityContract\{fileName}");
            var mimeType = FileExtensions.GetMimeTypeForFileExtension(fileName);

            byte[] fileBytes;

            if (System.IO.File.Exists(filepath))
            {
                logger.LogInformation($"fileName : {fileName}, File Exists");
                fileBytes = await System.IO.File.ReadAllBytesAsync(filepath);
                //Response.Headers.Add("Content-Disposition", "inline; filename=test.pdf");
                return File(fileBytes, mimeType);
                //return new FileContentResult(fileBytes, mimeType)
                //{
                //    FileDownloadName = fileName
                //};
            }
            else
            {
                logger.LogWarning($"fileName : {fileName}, File Not Exists");
                return NotFound();
            }
        }

        [AllowAnonymous]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}/{trackId}")]
        public async Task<IActionResult> ContractSignatureCallback(int requestFacilityId, string trackId, CancellationToken cancellationToken)
        {
            logger.LogInformation($"trackId value in parameter : {trackId}");
            logger.LogInformation($"requestFacilityId value in parameter : {requestFacilityId}");
            var signFileUrl = string.Empty;
            var receiveTrackId = string.Empty;
            if (!string.IsNullOrEmpty(Request.Query["trackid"]))
            {
                receiveTrackId = Request.Query["trackid"];
                logger.LogInformation($"trackid value in querystring : {Request.Query["trackid"]}");
            }

            if (!string.IsNullOrEmpty(Request.Query["file"]))
            {
                signFileUrl = Request.Query["file"];
                logger.LogInformation($"file value in querystring : {Request.Query["file"]}");
            }

            if (await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer },
                WorkFlowFormEnum.WaitingToSignContractByUser, cancellationToken))
            {
                if (!string.IsNullOrEmpty(receiveTrackId) && !string.IsNullOrEmpty(signFileUrl))
                {
                    if (trackId != receiveTrackId)
                        return BadRequest($"The 'trackId' sent is different from the 'trackId' received. '{trackId}' <> '{receiveTrackId}'");

                    await neoZarinCallBackSignatureLogService.AddLog(new Services.Model.NeoZarinCallBackSignatureLogModel()
                    {
                        RequestFacilityId = requestFacilityId,
                        SendTrackId = trackId,
                        FileUrl = signFileUrl,
                        ReceiveTrackId = receiveTrackId
                    }, cancellationToken);

                    using (var client = new HttpClient())
                    {
                        using (var result = await client.GetAsync(signFileUrl))
                        {
                            if (result.IsSuccessStatusCode)
                            {
                                var signedContract = await result.Content.ReadAsByteArrayAsync();
                                await requestFacilityService.SaveSignedContractAndMoveOnWorkFlow(requestFacilityId, signedContract, cancellationToken);
                                return Ok(new { message = "NeoZarin : save file and move on work flow!" });
                            }
                            else
                            {
                                return BadRequest(new { message = "NeoZarin : error in access to file or file url is wrong" });
                            }
                        }
                    }
                }
            }

            return BadRequest(new { message = "NeoZarin : trackId or file is Null or Empty!" });
        }

        [AllowAnonymous]
        [HttpGet("[controller]/[action]/{requestFacilityId:int}/{trackId}")]
        public async Task<IActionResult> TestContractSignatureCallback(int requestFacilityId, string trackId, CancellationToken cancellationToken)
        {
            logger.LogInformation($"trackId value in parameter : {trackId}");
            logger.LogInformation($"requestFacilityId value in parameter : {requestFacilityId}");

            if (await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer },
                WorkFlowFormEnum.WaitingToSignContractByUser, cancellationToken))
            {
                if (true)
                {
                    await requestFacilityService.TestSaveSignedContractAndMoveOnWorkFlow(requestFacilityId, cancellationToken);
                    return Ok(new { message = "save file and move on work flow!" });
                }
            }

            return BadRequest(new { message = "trackId or file is Null or Empty!" });
        }

        /// <summary>
        /// callback for signing contract by user/customer
        /// </summary>
        /// <param name="requestFacilityId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("[controller]/[action]/{requestFacilityId}")]
        public async Task<IActionResult> AyandehSignPostCallback(int requestFacilityId, CancellationToken cancellationToken)
        {
            logger.LogInformation($"AyandehSignPostCallback : requestFacilityId value in parameter : {requestFacilityId}");
            logger.LogInformation($"AyandehSignPostCallback : GetDisplayUrl : {Request.GetDisplayUrl()}");
            if (!Request.Body.CanSeek)
            {
                // We only do this if the stream isn't *already* seekable,
                // as EnableBuffering will create a new stream instance
                // each time it's called
                Request.EnableBuffering();
            }
            Request.Body.Position = 0;
            var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var body = await reader.ReadToEndAsync().ConfigureAwait(false);
            var signingTokenFromBody = string.Empty;
            if (!string.IsNullOrEmpty(body))
            {
                var resultObject = JObject.Parse(body);
                if (resultObject != null && resultObject.ContainsKey("signingToken") && !string.IsNullOrEmpty(Convert.ToString(resultObject["signingToken"])))
                {
                    signingTokenFromBody = resultObject["signingToken"]!.ToString();
                }
            }
            Request.Body.Position = 0;
            logger.LogInformation($"AyandehSignPostCallback : Request.Body : {body}");
            if (await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId, new List<RoleEnum> { RoleEnum.Buyer },
             WorkFlowFormEnum.WaitingToSignContractByUser, cancellationToken))
            {
                var signingToken = await requestFacilityService.GetAyandehSigningToken(requestFacilityId, cancellationToken);
                if (!string.IsNullOrEmpty(signingToken) && signingTokenFromBody == signingToken)
                {
                    var signedData = await ayandehSignService.GetData(signingToken, cancellationToken);
                    if (signingToken != null && signedData.Succeeded && !string.IsNullOrEmpty(signedData.Signature)
                        && signedData.Metadata == requestFacilityId.ToString())
                    {
                        var decodeData = HttpUtility.UrlDecode(signedData.Signature);
                        var pdfArray = Convert.FromBase64String(decodeData);
                        await requestFacilityService.SaveSignedContractAndMoveOnWorkFlow(requestFacilityId, pdfArray, cancellationToken);
                        logger.LogInformation("AyandehSign : save file and move on work flow!");
                        return Ok(new { message = "AyandehSign : save file and move on work flow!" });
                    }
                    else
                    {
                        logger.LogError("AyandehSign : data from AyandehSign.GetData is null or is not valid.");
                        return BadRequest(new { message = "AyandehSign : data from AyandehSign.GetData is null or is not valid." });
                    }
                }
                else
                {
                    logger.LogError($"AyandehSign : signToken not found in DB. {signingToken}={signingTokenFromBody}");
                    return BadRequest(new { message = "AyandehSign : signToken not found in DB. signingToken" });
                }
            }
            logger.LogError($"AyandehSign : RequestFacility ID={requestFacilityId} is not in the WaitingToSignContractByUser step.");
            return Ok(new { message = $"AyandehSign : RequestFacility ID={requestFacilityId} is not in the WaitingToSignContractByUser step." });
        }

        /// <summary>
        /// callback for signing contract by bank/leasing admin
        /// </summary>
        /// <param name="requestFacilityId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("[controller]/[action]/{requestFacilityId:int}/{leasingId:int}")]
        public async Task<IActionResult> BankAdminAyandehSignPostCallback(int requestFacilityId, int leasingId, CancellationToken cancellationToken)
        {
            logger.LogInformation($"BankAdminAyandehSignPostCallback : requestFacilityId value in parameter : {requestFacilityId}");
            logger.LogInformation($"BankAdminAyandehSignPostCallback : GetDisplayUrl : {Request.GetDisplayUrl()}");
            if (!Request.Body.CanSeek)
            {
                // We only do this if the stream isn't *already* seekable,
                // as EnableBuffering will create a new stream instance
                // each time it's called
                Request.EnableBuffering();
            }
            Request.Body.Position = 0;
            var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var body = await reader.ReadToEndAsync().ConfigureAwait(false);
            var signingTokenFromBody = string.Empty;
            if (!string.IsNullOrEmpty(body))
            {
                var resultObject = JObject.Parse(body);
                if (resultObject != null && resultObject.ContainsKey("signingToken") && !string.IsNullOrEmpty(Convert.ToString(resultObject["signingToken"])))
                {
                    signingTokenFromBody = resultObject["signingToken"]!.ToString();
                }
            }
            Request.Body.Position = 0;
            logger.LogInformation($"BankAdminAyandehSignPostCallback : Request.Body : {body}");
            //if (await requestFacilityService.CheckRequestFacilityWaitingSpecifiedStepAndRole(requestFacilityId,
            //                                                                                 leasingId,
            //                                                                                 new List<RoleEnum> { RoleEnum.AdminBankLeasing },
            //                                                                                 WorkFlowFormEnum.AdminBankLeasingSignature,
            //                                                                                 cancellationToken))
            //{
            var signingToken = await requestFacilityService.GetAyandehSignSigningTokenForAdminBank(requestFacilityId, cancellationToken);

            logger.LogInformation($"{signingToken} x==x {signingTokenFromBody}");
            if (!string.IsNullOrEmpty(signingToken) && signingTokenFromBody == signingToken)
            {
                var signedData = await ayandehSignService.GetData(signingToken, cancellationToken);
                logger.LogInformation($"26842684 : {JsonConvert.SerializeObject(signedData)}");
                if (signingToken != null && signedData.Succeeded && !string.IsNullOrEmpty(signedData.Signature)
                    && signedData.Metadata == requestFacilityId.ToString())
                {
                    logger.LogInformation("123456");
                    var decodeData = HttpUtility.UrlDecode(signedData.Signature);
                    var pdfArray = Convert.FromBase64String(decodeData);
                    await requestFacilityService.SaveSignedContractByAdminAndMoveOnWorkFlow(requestFacilityId, leasingId, pdfArray, cancellationToken);
                    logger.LogInformation("AyandehSign by Bank Admin : save file and move on work flow!");
                    return Ok(new { message = "AyandehSign by Bank Admin : save file and move on work flow!" });
                }
                else
                {
                    logger.LogError("AyandehSign : data from AyandehSign.GetData is null or is not valid.");
                    return BadRequest(new { message = "AyandehSign : data from AyandehSign.GetData is null or is not valid." });
                }
            }
            else
            {
                logger.LogError($"AyandehSign : signToken not found in DB. {signingToken}={signingTokenFromBody}");
                return BadRequest(new { message = "AyandehSign : signToken not found in DB. signingToken" });
            }
            //}
            logger.LogError($"AyandehSign : RequestFacility ID={requestFacilityId} is not in the WaitingToSignContractByUser step.");
            return Ok(new { message = $"AyandehSign : RequestFacility ID={requestFacilityId} is not in the WaitingToSignContractByUser step." });
        }


        [AllowAnonymous]
        [HttpGet("[controller]/[action]/{requestFacilityId}")]
        public async Task<IActionResult> AyandehSignPostCallbackForPromissory(int requestFacilityId, CancellationToken cancellationToken)
        {
            logger.LogInformation($"AyandehSignPostCallbackForPromissory : requestFacilityId value in parameter : {requestFacilityId}");
            logger.LogInformation($"AyandehSignPostCallbackForPromissory : GetDisplayUrl : {Request.GetDisplayUrl()}");
            return Ok(new { message = $"AyandehSignPostCallbackForPromissory" });
        }
    }
}