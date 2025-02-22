using Common.CustomAttribute;
using Services.Model.NeginHub;
using System;
using System.Threading;
using System.Threading.Tasks;
using ZarinLend.Services.Model.Ayandeh.BankAccount;
using ZarinLend.Services.Model.NeginHub;

namespace Services
{
    public interface INeginHubService
    {
        Task<bool?> CheckShahkar(CheckShahkarInputModel model, Guid? creator, CancellationToken cancellationToken);
        Task<GetCivilRegistryDataResultModel?> GetCivilRegistryDataIncludePersonPhotoV4(GetCivilRegistryDataInputModel model, Guid? creator, CancellationToken cancellationToken);
        Task<GetCivilRegistryDataResultModel?> GetCivilRegistryDataV4(GetCivilRegistryDataInputModel model, Guid? creator, CancellationToken cancellationToken);
        Task<GetCivilRegistryDataResultModel?> GetCivilRegistryDataV1(GetCivilRegistryDataInputModel model, Guid? creator, CancellationToken cancellationToken);
        Task<SanaInquieryDataResultModel?> SanaInquiryV5(SanaInquieryDataInputModel model, Guid? creator, CancellationToken cancellationToken);
        Task<PostalCodeInquieryDataResultModel?> PostalCodeInquiry(PostalCodeInquieryDataInputModel model, Guid? creator, CancellationToken cancellationToken);
        Task<bool?> NationalCodeAndCardVerification(string cardNumber, [NationalCode] string nationalCode, string shamsiBirthdate, Guid? creator, CancellationToken cancellationToken);
        Task<CardToIbanRasult?> CardToIBAN(string cardNumber, Guid? creator, CancellationToken cancellationToken);
        Task<ClientIdAndAccountNumbersAyandehResultModel> GetClientIdAndAccountNumbersAyandeh(ClientIdAndAccountNumbersAyandehInputModel model, Guid? creator, CancellationToken cancellationToken);
        Task<DepositToIbanResponseModel> DepositToIbanAsync(DepositToIbanInputModel model, Guid operatorId, CancellationToken cancellationToken);
    }
}