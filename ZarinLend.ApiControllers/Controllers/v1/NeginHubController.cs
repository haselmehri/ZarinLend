using Asp.Versioning;
using Common;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Model.NeginHub;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;
using ZarinLend.Services.Model.Ayandeh.BankAccount;
using ZarinLend.Services.Model.NeginHub;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class NeginHubController : BaseApiController
    {
        private readonly INeginHubService neginHubService;
        private readonly IUserService userService;
        private readonly IPersonService personService;

        public NeginHubController(INeginHubService neginHubService, IUserService userService, IPersonService personService)
        {
            this.neginHubService = neginHubService;
            this.userService = userService;
            this.personService = personService;
        }

        [HttpPost("[action]")]
        //[CustomAuthorize(RoleEnum.Buyer)]
        [CustomAuthorize(RoleEnum.ZarinLendExpert, RoleEnum.Admin,RoleEnum.SuperAdmin, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult<GetCivilRegistryDataResultModel>> GetCivilRegistryData(GetCivilRegistryDataInputModel model, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity.GetUserId());

            var result = await neginHubService.GetCivilRegistryDataV4(model, userId,  cancellationToken);

            if (result.IsSuccess) { 
                await personService.UpdateAsync(new() 
                {
                    NationalCode = result.NationalCode,
                    FirstName = result.FirstName,
                    LastName = result.LastName,
                    FatherName = result.FatherName,
                    PlaceOfBirth = result.PlaceOfBirth,
                    SSID = result.IdentityId != "0" ? result.IdentityId : "0",
                    Gender = result.Gender == "Male" ? GenderEnum.Male : GenderEnum.Female
                });

                return new ApiResult<GetCivilRegistryDataResultModel>(result != null, ApiResultStatusCode.Success, result);
            }

            return new ApiResult<GetCivilRegistryDataResultModel>(
                false,
                ApiResultStatusCode.BadRequest,
                null,
                "Failed."
            );
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.Buyer, RoleEnum.ZarinLendExpert, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult<PostalCodeInquieryDataResultModel>> GetAddressInquiry([FromBody] PostalCodeInquieryDataInputModel model, CancellationToken cancellationToken) 
        {
            var userId = new Guid(User.Identity.GetUserId());
            var postalCodeResult = await neginHubService.PostalCodeInquiry(model, userId, cancellationToken);

            if (postalCodeResult != null && postalCodeResult.IsSuccess)
            {
                return new ApiResult<PostalCodeInquieryDataResultModel>(
                    true,
                    ApiResultStatusCode.Success,
                    postalCodeResult
                );
            }

            return new ApiResult<PostalCodeInquieryDataResultModel>(
                false,
                ApiResultStatusCode.BadRequest,
                null,
                "Failed."
            );
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.Buyer, RoleEnum.ZarinLendExpert, RoleEnum.BankLeasing, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult<PostalCodeInquieryDataResultModel>> ValidateValidatedAddress([FromBody] ValidateAddressInputModel model, CancellationToken cancellationToken)
        {
            await personService.ValidateValidatedAddressAsync(model, cancellationToken);

            return Ok();
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.ZarinLendExpert, RoleEnum.BankLeasing, RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ClientIdAndAccountNumbersAyandehResultModel>> GetClientIdAndAccountNumbersAyandeh([FromBody] ClientIdAndAccountNumbersAyandehInputModel model, CancellationToken cancellationToken)
        {
            var userId = new Guid(User.Identity.GetUserId());
            var result = await neginHubService.GetClientIdAndAccountNumbersAyandeh(model, userId, cancellationToken);

            return result;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.SuperAdmin, RoleEnum.Admin, RoleEnum.AdminBankLeasing, RoleEnum.SupervisorLeasing, RoleEnum.ZarinLendExpert, RoleEnum.BankLeasing, RoleEnum.Buyer, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<DepositToIbanResponseModel>> DepositToIban(DepositToIbanInputModel model, CancellationToken cancellationToken) 
        {
            var userId = new Guid(User.Identity.GetUserId());
            var result = await neginHubService.DepositToIbanAsync(model, userId, cancellationToken);

            return result;
        }
    }
}