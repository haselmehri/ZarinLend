using Core.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByUserAndPass(string username, string password, CancellationToken cancellationToken);
        Task AddAsync(User user, string password, CancellationToken cancellationToken);
        Task UpdateSecurityStampAsync(User user, CancellationToken cancellationToken);
        Task UpdateLastLoginDateAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> DeleteUserByNationalCode(string nationalCode, CancellationToken cancellationToken);
    }
}