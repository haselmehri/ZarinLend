using Common;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services;
using Services.Model.IranCreditScoring;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.ApiControllers.Controllers.v1
{

    [ApiVersion("1")]
    public class _OldIranCreditScoringController : BaseApiController
    {
        private readonly _OldIIranCreditScoringService iranCreditScoringService;
        private readonly IUserService userService;

        public _OldIranCreditScoringController(_OldIIranCreditScoringService iranCreditScoringService, IUserService userService)
        {
            this.iranCreditScoringService = iranCreditScoringService;
            this.userService = userService;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer)]
        public virtual async Task<object> SendRequest(IranCreditScoringInputModel model, CancellationToken cancellationToken)
        {
            //=============Test============
            //return new
            //{
            //    AccessToken = "AAAAAAAAAAAA",
            //    ApiKey = "BBBBBBBBBBBBBBB",
            //    HashCode = "CCCCCCCCCCCCCC",
            //};

            var userId = new Guid(User.Identity.GetUserId());
            var token = await iranCreditScoringService.GetToken(cancellationToken);
            var hashCode = await iranCreditScoringService.Request(new RequestModel()
            {
                //MobileNumber = model.Mobile,
                //RealPersonNationalCode = model.NationalCode
                RealPersonNationalCode = User.Identity.GetUserName(),
                MobileNumber = await userService.GetMobile(userId)
            }, token, cancellationToken);

            return new
            {
                token.AccessToken,
                token.ApiKey,
                HashCode = hashCode
            };
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Buyer)]
        public virtual async Task<ApiResult<dynamic>> Download(ValidateInput model, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity.GetUserId());
            //===============Test=============            
            var res = new IranCreditScoringResult()
            {
                Score = new IranCreditScoringModel()
                {
                    PdfUrl = "TestUrl",
                    Score = 650,
                    Description = "کم ریسک",
                    Risk = "A1",
                    RequestFacilityId = model.RequestFacilityId,
                }
            };
            await iranCreditScoringService.SaveVerify(userId, res.Score, cancellationToken);
            return res;
            //===============Test=============
            var tokenModel = new TokenModel()
            {
                AccessToken = model.AccessToken,
                ApiKey = model.ApiKey
            };

            var validateResult = await iranCreditScoringService.Validate(model.HashCode, model.Otp, tokenModel, cancellationToken);
            if (validateResult != null && !string.IsNullOrEmpty(validateResult.Item1))
            {
                var statusResult = await iranCreditScoringService.Status(model.HashCode, tokenModel, cancellationToken);
                while (statusResult == null || statusResult.ReportLink == null)
                {
                    Thread.Sleep(1000);
                    statusResult = await iranCreditScoringService.Status(model.HashCode, tokenModel, cancellationToken);
                }
                if (statusResult != null /*&& statusResult.StatusTitle == "ساخت گزارش"*/)
                {
                    var reportCode = statusResult.ReportLink.Split('/')[statusResult.ReportLink.Split('/').Length - 1];
                    if (!string.IsNullOrEmpty(reportCode))
                    {
                        var pdfBase64 = await iranCreditScoringService.PdfReport(reportCode, tokenModel, cancellationToken);
                        var xml = await iranCreditScoringService.Xml(reportCode, tokenModel, cancellationToken);
                        var json = await iranCreditScoringService.Json(reportCode, tokenModel, cancellationToken);

                        var result = new IranCreditScoringResult();
                        if (!string.IsNullOrEmpty(json))
                            result = JsonConvert.DeserializeObject<IranCreditScoringResult>(json);

                        result.Score.Xml = xml;
                        result.Score.Json = json;
                        result.Score.PdfBase64String = pdfBase64;
                        result.Score.RequestFacilityId = model.RequestFacilityId;

                        await iranCreditScoringService.SaveVerify(userId, result.Score, cancellationToken);

                        return result;
                    }
                }
            }
            if (validateResult != null && validateResult.Item2 != null)
                return new ApiResult<dynamic>(false, ApiResultStatusCode.ServerError, new { HasError = true, ErrorResult = validateResult.Item2 });

            return new ApiResult<dynamic>(false, ApiResultStatusCode.ServerError, null, message: "ارتباط با سرور جهت دریافت نتیحه اعتبار سنجی برقرار نشد!");
        }

        //[HttpPost("[action]")]
        //[CustomAuthorize(RoleEnum.Buyer)]
        //public virtual async Task<object> Download(ValidateInput model, CancellationToken cancellationToken)
        //{
        //    var userId = new Guid(User.Identity.GetUserId());
        //    var tokenModel = new TokenModel()
        //    {
        //        AccessToken = model.AccessToken,
        //        ApiKey = model.ApiKey
        //    };
        //    var validateResult = await iranCreditScoringService.Validate(model.HashCode, model.Otp, tokenModel, cancellationToken);
        //    if (!string.IsNullOrEmpty(validateResult))
        //    {
        //        var statusResult = await iranCreditScoringService.Status(model.HashCode, tokenModel, cancellationToken);
        //        while(statusResult == null || statusResult.ReportLink == null)
        //        {
        //            Thread.Sleep(1000);
        //            statusResult = await iranCreditScoringService.Status(model.HashCode, tokenModel, cancellationToken);
        //        }
        //        if (statusResult != null /*&& statusResult.StatusTitle == "ساخت گزارش"*/)
        //        {
        //            var reportCode = statusResult.ReportLink.Split('/')[statusResult.ReportLink.Split('/').Length - 1];
        //            if (!string.IsNullOrEmpty(reportCode))
        //            {
        //                var pdfBase64 = await iranCreditScoringService.PdfReport(reportCode, tokenModel, cancellationToken);
        //                var xml = await iranCreditScoringService.Xml(reportCode, tokenModel, cancellationToken);
        //                if (!string.IsNullOrEmpty(xml))
        //                {
        //                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(xml);
        //                    xml = Convert.ToBase64String(plainTextBytes);
        //                }
        //                var json = await iranCreditScoringService.Json(reportCode, tokenModel, cancellationToken);
        //                var result = new IranCreditScoringResult();
        //                if (!string.IsNullOrEmpty(json))
        //                {
        //                    result = JsonConvert.DeserializeObject<IranCreditScoringResult>(json);
        //                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(json);
        //                    json = Convert.ToBase64String(plainTextBytes);
        //                }

        //                //TODO IranCreditScoring
        //                await iranCreditScoringService.ApprovedVerifyIranCreditScoreStep(userId, model.RequestFacilityId, cancellationToken);
        //                return new
        //                {
        //                    Result = result,
        //                    PdfBase64 = pdfBase64,
        //                    Xml = xml,
        //                    Json = json
        //                };
        //            }
        //        }
        //    }
        //    return null;
        //}
    }
}