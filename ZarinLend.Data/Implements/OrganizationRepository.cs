using Common;
using Common.Exceptions;
using Common.Utilities;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data.Repositories
{
    public class OrganizationRepository : BaseRepository<Organization>, IOrganizationRepository, IScopedDependency
    {
        public OrganizationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
