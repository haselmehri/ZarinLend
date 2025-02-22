using Core.Entities;
using Services.Model;
using System.Threading.Tasks;

namespace Services
{
    public interface IJwtService
    {
        Task<TokenModel> GenerateAsync(User user);
    }
}