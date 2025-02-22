using Services.Model;
using Services.Model.Payment;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IPaymentInfoService
    {
        Task<Guid> Add(PaymentInfoModel model, CancellationToken cancellationToken);
        Task<PaymentInfoModel> Get(Guid id, Guid sellerId, CancellationToken cancellationToken);
        Task<Guid> OtpIsVerify(VerifyOtpForTransferWalletBallanceModel model, CancellationToken cancellationToken);
    }
}