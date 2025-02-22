using Services.Model.IranCreditScoring;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface IIranCreditScoringService
    {
        Task<bool> GoToNextStepFromVerifyPaymentToZarinLend(Guid userId, int requestFacilityId, CancellationToken cancellationToken = default);
        Task<bool> ExistVerifyResult(int requestFacilityId, int expireAfterFewDays = 30, CancellationToken cancellationToken = default);
        Task<bool> ExistVerifyResult(Guid userId, int expireAfterFewDays = 30, CancellationToken cancellationToken = default);
        Task<IranCreditScoringModel> GetVerifyResult(int requestFacilityId, CancellationToken cancellationToken = default);   
        Task<IranCreditScoringModel> GetVerifyResultByRequestFacilityGaurantor(int requestFacilityGuarantorId, CancellationToken cancellationToken = default);
        Task<IranCreditScoringModel> GetVerifyResult(Guid userId, CancellationToken cancellationToken = default);
        Task<IranCreditScoringModel> SaveVerify(Guid userId, IranCreditScoringModel model, CancellationToken cancellationToken = default);
        Task<string?> Request(RequestModel model, Guid? creator, CancellationToken cancellationToken);
        Task<string?> Validate(string hashCode, string otp, Guid? creator, CancellationToken cancellationToken);
        Task<StatusResultModel?> Status(string hashCode, Guid? creator, CancellationToken cancellationToken);
        Task<string?> PdfReport(string reportCode,Guid? creator, CancellationToken cancellationToken);
        Task<string?> Json(string reportCode,Guid? creator, CancellationToken cancellationToken);
    }
}