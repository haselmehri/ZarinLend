using Newtonsoft.Json.Linq;
using Services.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public interface ISamatService
    {
        Task<bool> InquiryDone(int requestFacilityId, CancellationToken cancellationToken = default);

        Task<SamatFacilityHeaderModel> GetUserFacilitiesFromDB(int requestFacilityId, CancellationToken cancellationToken);

        Task<JObject> FacilityInquiry(string nationalCode, CancellationToken cancellationToken);

        /// <summary>
        /// get user Facilities from Central Bank and Save in database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="creatorId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> GetUserFacilitiesFromCentralBank(int requestFacilityId, Guid requestFacilityUserId, Guid creatorId, CancellationToken cancellationToken);
        Task<SamatBackChequeHeaderModel> GetUserBackChequesFromDB(int requestFacilityId, CancellationToken cancellationToken);

        Task<JObject> BackCheques(string nationalCode, CancellationToken cancellationToken);

        /// <summary>
        /// get user Back Cheque from Central Bank and Save in database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="creatorId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> GetUserBackChequesFromCentralBank(int requestFacilityId, Guid requestFacilityUserId, Guid creatorId, CancellationToken cancellationToken);
    }
}