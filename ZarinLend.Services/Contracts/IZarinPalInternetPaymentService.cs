using Core.Entities;
using Core.Entities.Business.Payment;
using Microsoft.AspNetCore.Http;
using Services.Model;
using Services.Model.Payment;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IZarinPalInternetPaymentService
    {
        /// <summary>
        /// initilize 'Zarinpal Internet payment' and get 'authority'
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="requestFacilityId"></param>
        /// <param name="amount"></param>
        /// <param name="paymentType"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="mobile"></param>
        /// <param name="description"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> InitilizeInternetPayment(Guid userId, int? requestFacilityId, long amount, PaymentType paymentType, string callbackUrl, string mobile,
            string description, CancellationToken cancellationToken);

        /// <summary>
        /// initilize 'Zarinpal Internet payment' and get 'authority'
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="requestFacilityGuarantorId"></param>
        /// <param name="requestFacilityId"></param>
        /// <param name="amount"></param>
        /// <param name="paymentType"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="mobile"></param>
        /// <param name="description"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> InitilizeInternetPaymentByGuarantor(Guid userId, int requestFacilityGuarantorId, int requestFacilityId, long amount, PaymentType paymentType, string callbackUrl,
            string mobile, string description, CancellationToken cancellationToken);

        /// <summary>
        /// initilize 'Zarinpal Internet payment' and get 'authority'
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="requestFacilityId"></param>
        /// <param name="requestFacilityInstallmentId"></param>
        /// <param name="amount"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="mobile"></param>
        /// <param name="description"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> InitilizeInternetPayment(Guid userId, int requestFacilityId, int requestFacilityInstallmentId, long amount, string callbackUrl, string mobile,
            string description, CancellationToken cancellationToken);

        Task<string> InitilizeInternetPaymentBySeller(Guid userId, Guid paymentInfoId, long amount, string mobile, string description,
            string callbackUrl, CancellationToken cancellationToken);

        Task<InternetPaymentResponseModel> ValidatePaymentResponse(HttpContext context, CancellationToken cancellationToken);
        Task ValidationStatuslessPayments(Guid userId, CancellationToken cancellationToken);
        //Task ApprovedVerifyPaymentStepByGuarantor(Guid userId, int requestFacilityGuarantorId, CancellationToken cancellationToken);
        Task ApprovedPaymentVerifyShahkarAndSamatServiceStep(Guid userId, int requestFacilityId, CancellationToken cancellationToken);
        Task ApprovedVerifyShahkarAndSamatServiceStep(Guid userId, Guid creatorId, int requestFacilityId, CancellationToken cancellationToken);
        Task ApprovedPaymentSalesCommissioStep(Guid userId, int requestFacilityId, CancellationToken cancellationToken);
        //Task<bool> ExistSuccessfulPayment(int requestFacilityId, PaymentType paymentType, CancellationToken cancellationToken);
        //Task<bool> ExistSuccessfulPayment(Guid userId, PaymentType paymentType, CancellationToken cancellationToken);
    }
}