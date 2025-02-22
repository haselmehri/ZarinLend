using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using Core.Data.Repositories;
using Core.Entities.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Services;
using System;
using WebFramework.Api;
using WebFramework.Filters;
using static WebFramework.Configuration.IdentityConfigurationExtensions;

namespace WebFramework.Configuration
{
    public static class AutofacConfigurationExtensions
    {
        public static IServiceProvider BuildAutofacServiseProvider(this IServiceCollection services)
        {
            var containerBuilder = new ContainerBuilder();
            //register services...
            services.AddTransient<CustomCookieAuthenticationEvents>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IAuthorizationHandler, LoggedIn>();
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();

            return new AutofacServiceProvider(container);
        }

        public static void AddServices(this ContainerBuilder containerBuilder)
        {
            //Registertype > As > LifeTime

            //services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IJwtService, JwtService>();

            containerBuilder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerLifetimeScope();
            containerBuilder.RegisterGeneric(typeof(ServiceRepository<,,,>)).As(typeof(IServiceRepository<,,,>)).InstancePerLifetimeScope();
            containerBuilder.RegisterGeneric(typeof(ServiceRepository<,,>)).As(typeof(IServiceRepository<,,>)).InstancePerLifetimeScope();
            containerBuilder.RegisterGeneric(typeof(ServiceRepository<,>)).As(typeof(IServiceRepository<,>)).InstancePerLifetimeScope();
            //containerBuilder.RegisterType<JwtService>().As<IJwtService>().InstancePerLifetimeScope();
            //containerBuilder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();

            //Scan Assembly & Register All Base Assignable Interface
            var commonAssembly = typeof(SiteSettings).Assembly;
            var entitiesAssembly = typeof(IEntity).Assembly;
            var dataAssembly = typeof(IBaseRepository<>).Assembly;
            var servicesAssembly = typeof(IJwtService).Assembly;
            var webFrameworkAssembly = typeof(ApiResult).Assembly;

            containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly, webFrameworkAssembly)
                .AssignableTo<IScopedDependency>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly, webFrameworkAssembly)
                .AssignableTo<ITransientDependency>()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly, webFrameworkAssembly)
                .AssignableTo<ISingletonDependency>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
