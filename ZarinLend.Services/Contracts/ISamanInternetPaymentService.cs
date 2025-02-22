using Core.Entities;
using Core.Entities.Business.Payment;
using Microsoft.AspNetCore.Http;
using Services.Dto;
using Services.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface ISamanInternetPaymentService
    {
        Task<long> GetTotalWithdrawBaseCard(string cardNumber, Guid userId, CancellationToken cancellationToken);
        Task<PagingDto<SamanIntenetBankPaymentExportModel>> SearchForExport(PagingFilterDto filter, CancellationToken cancellationToken);
        Task<PagingDto<SamanIntenetBankPaymentModel>> Search(PagingFilterDto filter, CancellationToken cancellationToken);
        Task<GetTokenResultModel> GetToken(string resNum, long amount, string mobile, string redirectUrl, CancellationToken cancellationToken);
        Task<GetTokenResultModel> GetToken(string resNum, long amount, string mobile, string redirectUrl, string defaultCardNumber, CancellationToken cancellationToken);
        Task<string> InitilizeInternetPayment(Guid userId, int? requestFacilityId, long amount, PaymentType paymentType, string description,
            CancellationToken cancellationToken);
        Task<string> InitilizeInternetPayment(Guid userId, int requestFacilityId, int requestFacilityInstallmentId, long amount, string description, CancellationToken cancellationToken);
        Task<string> InitilizeInternetPaymentByGuarantor(Guid userId, int requestFacilityGuarantorId, int requestFacilityId, long amount, PaymentType paymentType, string description,
            CancellationToken cancellationToken);
        Task<string> InitilizeInternetPaymentBySeller(Guid userId, Guid paymentInfoId, long amount, string cardNumber, string description, CancellationToken cancellationToken);
        Task<SamanInternetPaymentCallBackModel> ValidatePaymentResponse(HttpContext context, CancellationToken cancellationToken);
        Task<string> RegisterCard(string cardNumber, Guid? creator, CancellationToken cancellationToken);
        Task ApprovedPaymentSalesCommissioStep(Guid userId, int requestFacilityId, CancellationToken cancellationToken);
        //Task<bool> ExistSuccessfulPayment(int requestFacilityId, PaymentType paymentType, CancellationToken cancellationToken);
        //Task<bool> ExistSuccessfulPayment(Guid userId, PaymentType paymentType, CancellationToken cancellationToken);
    }
}