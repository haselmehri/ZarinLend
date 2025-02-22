using Services.Model;
using Services.Model.AccountStatement;
using System;
using System.Threading;
using System.Threading.Tasks;
using ZarinLend.Services.Model.Promissory;

namespace Services
{
    public interface IFinnotechService
    {
        string GenerateUrlToGetAuthorizationCode(string redirectUrl, string scope, string orderId);
        Task<AccountStatementModel?> GetAccountStatement(AccountStatementInput filter, string code, string redirectUrl, Guid? creator, CancellationToken cancellationToken);
        //Task<CardToIbanRasult> CardToIBAN(string cardNumber, Guid? creator, CancellationToken cancellationToken);
        Task<string> CifInquiry(string nationalCode, Guid? creator, CancellationToken cancellationToken);
        Task<Tuple<bool, string>> ChargeCard(int requestFacilityId, string cardNumber, long amount, Guid creator, CancellationToken cancellationToken);

        #region promissory methods

        Task<FinotechBaseResult<PromissoryPublishResponseModel>> PromissoryPublishRequest(PromissoryPublishRequestModel model, string trackId, Guid? creator, CancellationToken cancellationToken);
        Task<FinotechBaseResult<PromissorySignRequestResponseModel>> PromissorySignRequest(PromissorySignRequestModel model, Guid? creator, CancellationToken cancellationToken);
        Task<FinotechBaseResult<PromissoryFinalizeResponseModel>> PromissoryFinalize(string registrationId, Guid? creator, CancellationToken cancellationToken);
        Task<FinotechBaseResult<PromissoryStatusInquiryResponseModel>> PromissoryStatusInquiry(string registrationId, Guid? creator, CancellationToken cancellationToken);
        Task<FinotechBaseResult<PromissoryStatusInquiryResponseModel>> PromissorySignedDocument(string signingTrackId, Guid? creator, CancellationToken cancellationToken);
        Task<string?> PromissoryPublishRequestInquiry(string nationalCode, Guid? creator, CancellationToken cancellationToken);
        Task<bool?> PromissoryDelete(string requestId, Guid? creator, CancellationToken cancellationToken);

        #endregion promissory methods

    }
}