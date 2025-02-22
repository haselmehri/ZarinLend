using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PostgreSqlAndEFCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostgreSqlAndEFCore
{
    public class MyDbContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole,
        IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //var entitiesAssembly = System.Reflection.Assembly.GetAssembly(typeof(IEntity));
            builder.HasPostgresExtension("uuid-ossp");
            var entitiesAssembly = typeof(IEntity).Assembly;            
            builder.RegisterAllEntities<IEntity>(entitiesAssembly);
            builder.HasPostgresExtension("uuid-ossp");
            builder.AddRestrictDeleteBehaviorConvention();
            builder.AddSequentialGuidForIdConvention();
            builder.AddDefaultValueSqlConvention("CreatedDate", typeof(DateTime), "now()");
            builder.AddPluralizingTableNameConvention();
            builder.RegisterEntityTypeConfiguration(entitiesAssembly);
            //OR
            //modelBuilder.ApplyConfiguration<Post>(new PostConfiguration());
            //...
        }

    }

}