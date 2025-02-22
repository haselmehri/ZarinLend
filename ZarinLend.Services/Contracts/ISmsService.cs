using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface ISmsService
    {
        Task<bool> Send(string mobile, string message, CancellationToken cancellationToken);
        Task<long> SendPaymentLink(string mobile, string paymentInfoId, CancellationToken cancellationToken);
        Task<long?> SendVerificationSmsToTransferCreditAmount(Guid sellerId, long amount, [NotNull] string mobile, [NotNull] string shopName,
            string verficationCode, CancellationToken cancellationToken);
    }
}
