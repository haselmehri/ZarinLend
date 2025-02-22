using Common;
using Common.Exceptions;
using Common.Utilities;
using Core.Entities.Business.RequestFacility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public class RequestFacilityRepository : BaseRepository<RequestFacility>, IRequestFacilityRepository, IScopedDependency
    {
        public RequestFacilityRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }        
    }
}
