using Core.Entities;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public interface IPersonRepository : IBaseRepository<Person>
    {
        Task<DataTable> GetZarinpalTransactionInfo(string hashCardString, CancellationToken cancellationToken);
    }
}