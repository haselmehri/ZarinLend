using Services.Model.ZarinPal;
using System.Threading.Tasks;

namespace Services
{
    public interface IZarinpalService
    {
        Task<string> GetAccessToken(string code, string codeVerifier);
        Task<ZarinpalUserData> GetUserData(string accessToken);
        string MakeCodeVerifier();
        string MakeCodeChallenge(string verifier);
        string GenerateAuthorizationUrl(string challengeCode);
    }
}