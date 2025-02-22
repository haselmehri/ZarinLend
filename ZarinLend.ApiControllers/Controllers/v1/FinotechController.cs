using Asp.Versioning;
using Common;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Services;
using Services.Model;
using Services.Model.AccountStatement;
using Services.Model.NeginHub;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Configuration;
using WebFramework.Filters;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class FinotechController(INeginHubService neginHubService, IFinnotechService finnotechService, ISamatService samatService, IOptionsSnapshot<SiteSettings> siteSettings) : BaseApiController
    {
        private readonly SiteSettings siteSettings = siteSettings.Value;

        [HttpPost("[action]/{cardNumber}")]
        [AllowAnonymous]
        public virtual async Task<CardToIbanRasult> CardToIBAN(string cardNumber, CancellationToken cancellationToken)
        {
            var result = await neginHubService.CardToIBAN(cardNumber, null, cancellationToken);

            return result;
        }

        [HttpPost("[action]/{nationalCode}")]
        [AllowAnonymous]
        public virtual async Task<string> CifInquiry(string nationalCode, CancellationToken cancellationToken)
        {
            return await finnotechService.CifInquiry(nationalCode, null, cancellationToken);
        }


        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("[action]")]
        public virtual string GenerateUrlToGetAuthorizationCode(AccountStatementInput filter)
        {
            var orderId = $"{Guid.NewGuid()};{filter.deposit};{filter.fromDate.Replace("/", string.Empty)};{filter.toDate.Replace("/", string.Empty)}";

            CookieManager.Set(HttpContext, CookieManager.CookieKeys.StatementOrderId, orderId, SameSiteMode.Unspecified, false, false, false, DateTime.Now.AddMinutes(15));
            //var callBackUrl = finnotechService.GenerateUrlToGetAuthorizationCode($"{BaseUrl()}Finotech/GetAccountStatementCallBack", FinnotechService.ACCOUNT_STATEMENT_SCOPE, orderId);
            var callBackUrl = finnotechService.GenerateUrlToGetAuthorizationCode(siteSettings.FinotechAuthorizationCodeRedirectUrl, FinnotechService.ACCOUNT_STATEMENT_SCOPE, orderId);

            return callBackUrl;
        }

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("[action]/{nationalCode}")]
        public virtual async Task<JObject> FacilityInquiry(string nationalCode, CancellationToken cancellationToken)
        {
            var result = await samatService.FacilityInquiry(nationalCode, cancellationToken);

            return result;
        }

        [CustomAuthorize(RoleEnum.Admin, RoleEnum.SuperAdmin, RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("[action]/{nationalCode}")]
        public virtual async Task<JObject> BackCheques(string nationalCode, CancellationToken cancellationToken)
        {
            var result = await samatService.BackCheques(nationalCode, cancellationToken);

            return result;
        }
    }
}
