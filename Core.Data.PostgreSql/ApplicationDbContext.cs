using Common.Utilities;
using Core.Entities;
using Core.Entities.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole,
      IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>  //DbContext
    {
        public ApplicationDbContext([NotNull] DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //var entitiesAssembly = System.Reflection.Assembly.GetAssembly(typeof(IEntity));            
            var entitiesAssembly = typeof(IEntity).Assembly;
            modelBuilder.HasPostgresExtension("uuid-ossp");
            modelBuilder.HasPostgresExtension("citext");
            modelBuilder.RegisterAllEntities<IEntity>(entitiesAssembly);
            modelBuilder.AddRestrictDeleteBehaviorConvention();
            modelBuilder.AddSequentialGuidForIdConvention(ModelBuilderExtensions.DatabaseType.PostgreSQL);
            modelBuilder.AddDefaultValueSqlConvention("CreatedDate", typeof(DateTime), "now()");
            modelBuilder.ChangeStringPropertyColumnType();
            modelBuilder.AddPluralizingTableNameConvention();
            modelBuilder.RegisterEntityTypeConfiguration(entitiesAssembly);
            //OR
            //modelBuilder.ApplyConfiguration<Post>(new PostConfiguration());
            //...
        }

        public override int SaveChanges()
        {
            _manipulationPropertiesBeforeSave();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            _manipulationPropertiesBeforeSave();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            _manipulationPropertiesBeforeSave();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _manipulationPropertiesBeforeSave();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void _manipulationPropertiesBeforeSave()
        {
            var changedEntities = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
            foreach (var item in changedEntities)
            {
                if (item.Entity == null)
                    continue;

                var stringProperties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

                foreach (var property in stringProperties)
                {
                    var propName = property.Name;
                    var val = (string)property.GetValue(item.Entity, null);

                    if (val.HasValue())
                    {
                        var newVal = val.Fa2En().FixPersianChars();
                        if (newVal == val)
                            continue;
                        property.SetValue(item.Entity, newVal, null);
                    }
                }

                #region fill 'UpdateDate' & 'CreateDate' properties
                var datetimeProperties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                 .Where(p => p.CanRead && p.CanWrite && (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?)));

                foreach (var property in datetimeProperties)
                {                  
                    //https://www.entityframeworktutorial.net/faq/set-created-and-modified-date-in-efcore.aspx
                    var currentDateTime = DateTime.Now;
                    if (item.State == EntityState.Added && property.Name == "CreatedDate" &&
                        (property.GetValue(item.Entity, null) == null || Convert.ToDateTime(property.GetValue(item.Entity, null)) == default))
                        property.SetValue(item.Entity, currentDateTime);

                    if (property.Name == "UpdateDate" &&
                        (property.GetValue(item.Entity, null) == null || Convert.ToDateTime(property.GetValue(item.Entity, null)) == default))
                        property.SetValue(item.Entity, currentDateTime);                    
                }
                #endregion
            }
        }
    }
}
