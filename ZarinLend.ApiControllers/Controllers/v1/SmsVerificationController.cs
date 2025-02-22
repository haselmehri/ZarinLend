using Asp.Versioning;
using Microsoft.Extensions.Logging;
using Services;
using WebFramework.Api;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class SmsVerificationController : BaseApiController
    {
        private readonly ISmsService smsService;
        private readonly ILogger<SmsVerificationController> logger;

        public SmsVerificationController(ISmsService smsService, ILogger<SmsVerificationController> logger)
        {
            this.smsService = smsService;
            this.logger = logger;
        }

        //[HttpPost("[action]")]
        //[CustomAuthorize(RoleEnum.Seller, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public virtual async Task<long?> SendVerificationCode(SendSmsVerificationModel model, CancellationToken cancellationToken)
        //{
        //    model.OrganizationName = User.Identity.GetOrganizationName();
        //    return await smsService.SendVerificationSmsToTransferCreditAmount(model.BuyerId, model.SellerId,
        //        Convert.ToInt64(model.AmountThousandSeparator.Replace(",", string.Empty)), model.OrganizationName, cancellationToken);
        //}
    }
}
