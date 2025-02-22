using Newtonsoft.Json.Linq;
using Services.Model.IranCreditScoring;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface _OldIIranCreditScoringService
    {
        Task ApprovedVerifyIranCreditScoreStep(Guid userId, int requestFacilityId, CancellationToken cancellationToken);
        Task<IranCreditScoringModel> GetVerifyResult(int requestFacilityId, CancellationToken cancellationToken = default);
        Task<IranCreditScoringModel> GetVerifyResult(Guid userId, CancellationToken cancellationToken = default);
        Task<IranCreditScoringModel> SaveVerify(Guid userId, IranCreditScoringModel model, CancellationToken cancellationToken = default);
        Task<string> Request(RequestModel model, TokenModel token, CancellationToken cancellationToken);
        Task<Tuple<string, JObject>> Validate(string hashCode, string otp, TokenModel token, CancellationToken cancellationToken);
        Task<StatusResultModel> Status(string hashCode, TokenModel token, CancellationToken cancellationToken);
        Task<string> PdfReport(string reportCode, TokenModel token, CancellationToken cancellationToken);
        Task<string> Xml(string reportCode, TokenModel token, CancellationToken cancellationToken);
        Task<string> Json(string reportCode, TokenModel token, CancellationToken cancellationToken);
        Task<TokenModel> GetToken(CancellationToken cancellationToken = default);
        //Task<List<FinotechTransactionCodeModel>> GetAllProcessCode(CancellationToken cancellationToken);
        //Task<long> Add(CardTransactionModel model, CancellationToken cancellationToken);
        //Task<List<CardStatementResultModel>> GetCardStatement(CardStatementInput model, CancellationToken cancellationToken);
        //Task UpdateSendStatusMessage(long id, string mobile, string message, CancellationToken cancellationToken);
        //Task<CardTransactionModel> CheckLog(long cardNumber, string refNum, CancellationToken cancellationToken);
        //Task<DepositBalanceResult> GetAccountBalance(string deposit, CancellationToken cancellationToken);
        //Task<long?> GetCardBalance(string cardNumber, CancellationToken cancellationToken);
        //Task<Tuple<bool, string>> CardCharge(string cardNumber, long amount, CancellationToken cancellationToken);
    }
}