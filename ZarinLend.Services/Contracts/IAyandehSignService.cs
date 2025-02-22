using System;
using System.Threading;
using System.Threading.Tasks;
using ZarinLend.Services.Model.AyandehSign;
using static Services.AyandehSignService;

namespace Services
{
    public interface IAyandehSignService
    {
        Task<GetSigningTokenResult> GetSigningToken(int requestFacilityId, Guid userId, string pdfToSign, string nationalCode, string mobile, string title, string hint, string callBackUrl, CancellationToken cancellationToken);
        Task<AyandehSignGetDataModel> GetData(string signingToken, CancellationToken cancellationToken);
        Task<AyandehSignContentStatus?> CheckStatus(string signingToken, CancellationToken cancellationToken);
    }
}