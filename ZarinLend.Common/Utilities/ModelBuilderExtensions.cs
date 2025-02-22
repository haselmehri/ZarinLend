using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Pluralize.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Common.Utilities
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Singularizin table name like Posts to Post or People to Person
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void AddSingularizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            Pluralizer pluralizer = new Pluralizer();
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                //string tableName = entityType.Relational().TableName;
                //entityType.Relational().TableName = pluralizer.Singularize(tableName);
                string tableName = entityType.GetTableName();
                entityType.SetTableName(pluralizer.Singularize(tableName));
            }
        }

        /// <summary>
        /// Pluralizing table name like Post to Posts or Person to People
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void AddPluralizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            Pluralizer pluralizer = new Pluralizer();
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes().Where(p => !p.IsAbstract()))
            {
                //string tableName = entityType.Relational().TableName;
                //entityType.Relational().TableName = pluralizer.Pluralize(tableName);
                string tableName = entityType.GetTableName();
                entityType.SetTableName(pluralizer.Pluralize(tableName));
            }
        }

        public enum DatabaseType
        {
            MicrosoftSqlServer,
            PostgreSQL
        }
        /// <summary>
        /// Set NEWSEQUENTIALID() sql function for all columns named "Id"(for Sql Server)
        /// Set NEWSEQUENTIALID() sql function for all columns named "Id"(for PostgreSQL)
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="mustBeIdentity">Set to true if you want only "Identity" guid fields that named "Id"</param>
        public static void AddSequentialGuidForIdConvention(this ModelBuilder modelBuilder, DatabaseType databaseType)
        {
            if (databaseType == DatabaseType.MicrosoftSqlServer)
            {
                #region FOR SQL SERVER
                //modelBuilder.AddDefaultValueSqlConvention("Id", typeof(Guid), "NEWSEQUENTIALID()");
                #endregion
            }
            else if (databaseType == DatabaseType.PostgreSQL)
            {
                #region FOR PostgreSQL
                modelBuilder.AddDefaultValueSqlConvention("Id", typeof(Guid), "uuid_generate_v4()");
                #endregion
            }
        }

        /// <summary>
        /// Set DefaultValueSql for sepecific property name and type
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="propertyName">Name of property wants to set DefaultValueSql for</param>
        /// <param name="propertyType">Type of property wants to set DefaultValueSql for </param>
        /// <param name="defaultValueSql">DefaultValueSql like "NEWSEQUENTIALID()"</param>
        public static void AddDefaultValueSqlConvention(this ModelBuilder modelBuilder, string propertyName, Type propertyType, string defaultValueSql)
        {
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                //IMutableProperty property = entityType.GetProperties().SingleOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
                //if (property != null && property.ClrType == propertyType)
                //    property.Relational().DefaultValueSql = defaultValueSql;

                #region My Method
                //var property = entityType.FindPrimaryKey().Properties.SingleOrDefault();
                //if (property != null && property.ClrType == propertyType)
                //    property.SetDefaultValueSql(defaultValueSql);
                #endregion My Method

                IMutableProperty property = entityType.GetProperties().SingleOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
                if (property != null && property.ClrType == propertyType)
                    property.SetDefaultValueSql(defaultValueSql);
            }
        }

        /// <summary>
        /// Just to PostgreSQL
        /// change string property to 'citext' PostgreSql column type to must be Case insensitive 
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void ChangeStringPropertyColumnType(this ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.ClrType == typeof(string))
                    {
                        //property.Relational().ColumnType = "citext";
                        property.SetColumnType("citext");
                    }
                }
            }
        }

        /// <summary>
        /// Set DeleteBehavior.Restrict by default for relations
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void AddRestrictDeleteBehaviorConvention(this ModelBuilder modelBuilder)
        {
            IEnumerable<IMutableForeignKey> cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && (fk.DeleteBehavior == DeleteBehavior.Cascade || fk.DeleteBehavior == DeleteBehavior.NoAction || fk.DeleteBehavior == DeleteBehavior.SetNull));

            //List<IMutableForeignKey> cascadeFKs1 = modelBuilder.Model.GetEntityTypes()
            //    .SelectMany(t => t.GetForeignKeys())
            //    .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade).ToList();

            //List<IMutableForeignKey> cascadeFKs12 = modelBuilder.Model.GetEntityTypes()
            //    .SelectMany(t => t.GetForeignKeys())
            //    .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade).ToList();

            //List<IMutableForeignKey> cascadeFKs112 = modelBuilder.Model.GetEntityTypes()
            //    .SelectMany(t => t.GetForeignKeys()).ToList();

            foreach (IMutableForeignKey fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;
        }

        /// <summary>
        /// Dynamicaly load all IEntityTypeConfiguration with Reflection
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="assemblies">Assemblies contains Entities</param>
        public static void RegisterEntityTypeConfiguration(this ModelBuilder modelBuilder, params Assembly[] assemblies)
        {
            //get ApplyConfiguration
            MethodInfo applyGenericMethod = typeof(ModelBuilder).GetMethods().First(m => m.Name == nameof(ModelBuilder.ApplyConfiguration));

            //IEnumerable<Type> types = assemblies.SelectMany(a => a.GetExportedTypes())
            //    .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic);

            IEnumerable<Type> types = assemblies.SelectMany(a => a.GetExportedTypes())
                .Where(c => c.IsClass &&
                            !c.IsAbstract &&
                            c.IsPublic &&
                            c.GetInterfaces().Any() &&
                            c.GetInterfaces().Any(x => x.IsConstructedGenericType &&
                                                       x.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

            foreach (Type type in types)
            {
                foreach (Type iface in type.GetInterfaces())
                {
                    if (iface.IsConstructedGenericType && iface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                    {
                        MethodInfo applyConcreteMethod = applyGenericMethod.MakeGenericMethod(iface.GenericTypeArguments[0]);
                        applyConcreteMethod.Invoke(modelBuilder, new object[] { Activator.CreateInstance(type) });
                    }
                }
            }
        }

        /// <summary>
        /// Dynamicaly register all Entities that inherit from specific BaseType
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="baseType">Base type that Entities inherit from this</param>
        /// <param name="assemblies">Assemblies contains Entities</param>
        public static void RegisterAllEntities<BaseType>(this ModelBuilder modelBuilder, params Assembly[] assemblies)
        {
            IEnumerable<Type> types = assemblies.SelectMany(a => a.GetExportedTypes())
                .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic && typeof(BaseType).IsAssignableFrom(c));

            foreach (Type type in types)
                modelBuilder.Entity(type);
        }
    }
}
