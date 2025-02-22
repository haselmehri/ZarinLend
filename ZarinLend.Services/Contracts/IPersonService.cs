using Services.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using ZarinLend.Services.Model.NeginHub;

namespace Services
{
    public interface IPersonService
    {
        Task<PersonZarinpalTransactionsInfoModel> GetZarinpalTransactionInfo(string hashCardNumber, CancellationToken cancellationToken);
        Task<PersonModel> GetPerson(int id, CancellationToken cancellationToken);
        Task<PersonCompleteInfoModel> GetPersonInfo(int id, CancellationToken cancellationToken);
        Task<bool> UpdateJobInfo(PersonJobEditModel model, CancellationToken cancellationToken = default);
        Task<PersonJobViewModel> GetJobInfo(int personId, CancellationToken cancellationToken = default);
        Task<bool> ValidateValidatedAddressAsync(ValidateAddressInputModel model, CancellationToken cancellationToken = default);
        Task<bool> SetSanaTrackingCodeAsync(string nationalCode, string trackingCode, CancellationToken cancellationToken);
        Task UpdateAsync(UpdatePersonInfoInputModel model, CancellationToken cancellationToken = default);
    }
}