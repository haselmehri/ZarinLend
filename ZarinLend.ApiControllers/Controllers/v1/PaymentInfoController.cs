using Asp.Versioning;
using Common;
using Common.Exceptions;
using Common.Utilities;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.X509;
using Services;
using Services.Model;
using Services.Model.GlobalSetting;
using Services.Model.Payment;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.Api;
using WebFramework.Filters;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class PaymentInfoController : BaseApiController
    {
        private readonly IPaymentInfoService paymentInfoService;
        private readonly IUserService userService;
        private readonly ISmsService smsService;
        private readonly IWalletTransactionService walletTransactionService;
        private readonly ILogger<PaymentInfoController> _logger;
        public readonly static string KEY_FOR_ENCRYPT_DECRYPT = "Z@r!nL3nd";

        public PaymentInfoController(IPaymentInfoService paymentInfoService,
                                     IUserService userService,
                                     ISmsService smsService,
                                     IWalletTransactionService walletTransactionService,
                                     ILogger<PaymentInfoController> logger)
        {
            this.paymentInfoService = paymentInfoService;
            this.userService = userService;
            this.smsService = smsService;
            this.walletTransactionService = walletTransactionService;
            _logger = logger;
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Seller, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> GeneratePaymentLink(PaymentInfoModel model, CancellationToken cancellationToken)
        {
            model.CreatorId = new Guid(User.Identity.GetUserId());
            model.SellerId = new Guid(User.Identity.GetUserId());
            model.ExpireDate = DateTime.Now.AddMinutes(15);
            model.IpgType = Core.Entities.Business.Payment.IpgType.ZarinpalIPG;

            var newId = await paymentInfoService.Add(model, cancellationToken);
            await smsService.SendPaymentLink(await userService.GetMobile(model.BuyerId),
                                             await SecurityHelper.EncryptAsStringAsync(newId.ToString(), KEY_FOR_ENCRYPT_DECRYPT),
                                             cancellationToken);
            return Ok();
        }

        [HttpPost("[action]/{buyerId:guid}/{cardNumber}")]
        [CustomAuthorize(RoleEnum.Seller, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public virtual async Task<ApiResult> SendVerificationCode(Guid buyerId,string cardNumber, CancellationToken cancellationToken)
        {
            var sellerId = new Guid(User.Identity.GetUserId());
            var organizationName = User.Identity.GetOrganizationName();
            var organizationId = User.Identity.GetSellerOrganizationId();
            var mobile = await userService.GetMobile(buyerId);
            var walletBalanceBaseCardNumber = await walletTransactionService.GetWalletBalanceBaseCard(cardNumber, buyerId, cancellationToken);
            if (walletBalanceBaseCardNumber.Balance == 0)
                throw new AppException("موجودی کارت صفر می باشد");

            Random rnd = new Random();
            var otp = string.Empty;
            for (int j = 0; j < 6; j++)
            {
                otp = $"{otp}{rnd.Next(0, 9)}";
            }
            var messageId = await smsService.SendVerificationSmsToTransferCreditAmount(sellerId, walletBalanceBaseCardNumber.Balance, mobile, organizationName, otp, cancellationToken);
            if (!messageId.HasValue)
                throw new AppException("خطا در ارسال پیامک تایید به خریدار!");

            var model = new PaymentInfoModel()
            {
                Amount = walletBalanceBaseCardNumber.Balance,
                CardNumber = cardNumber,
                BuyerId = buyerId,
                BuyerMobile = mobile,
                CreatorId = sellerId,
                ExpireDate = DateTime.Now.AddMinutes(15),
                IpgType = Core.Entities.Business.Payment.IpgType.SamanIPG,
                Otp = otp,
                SellerId = sellerId,
                OrganizationId = organizationId,
                MessageId = messageId.Value
            };

            await paymentInfoService.Add(model, cancellationToken);
            return Ok();
        }

        [HttpPost("[action]")]
        [CustomAuthorize(RoleEnum.Seller, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ApiResult> VerifyVerificationCode(VerifyOtpForTransferWalletBallanceModel model, CancellationToken cancellationToken)
        {
            var walletBalanceBaseCardNumber = await walletTransactionService.GetWalletBalanceBaseCard(model.CardNumber, model.BuyerId, cancellationToken);
            if (model.Amount > walletBalanceBaseCardNumber.Balance)
                throw new AppException("مبلغ بیشتر از موجودی کارت می باشد");

            model.SellerId = new Guid(User.Identity.GetUserId());
            model.OrganizationId = User.Identity.GetSellerOrganizationId();
            model.BuyerMobile = await userService.GetMobile(model.BuyerId);

            var paymentInfoId = await paymentInfoService.OtpIsVerify(model, cancellationToken);
            var encryptPaymentInfoId = WebUtility.UrlEncode(await SecurityHelper.EncryptAsStringAsync(paymentInfoId.ToString(), KEY_FOR_ENCRYPT_DECRYPT));
            return new ApiResult<dynamic>(true, ApiResultStatusCode.Success, encryptPaymentInfoId);
        }
    }
}
