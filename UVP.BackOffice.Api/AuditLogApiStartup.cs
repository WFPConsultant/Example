using Koa.Integration.PubSub.DependencyInjection;
using UVP.Shared.Micro.Api;
using UVP.Shared.Http.Integration;
using UVP.Shared.Micro.Api.Services;
using UVP.Shared.Micro.Domain.Configuration;
using UVP.Shared.Micro.Host.Extensions;

namespace UVP.AuditLog.Api
{
    using System;
    using System.Reflection;
    using Koa.Platform.Injection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Shared.Micro.Entities.Sql;
    using UVP.AuditLog.Business.Bus.Event.AuditLog;
    using UVP.Core.EventGrid.Extensions;
    using UVP.Profile.Business.Repository;
    using UVP.Shared.Event.AuditLog;
    using UVP.Shared.Micro.Business.Sql;

    /// <summary>
    /// Api startUp with the Api services injection.
    /// </summary>
    public static class AuditLogApiStartup
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <param name="config">Configuration.</param>
        /// <param name="configureDataContext">Options builder for DbContext.</param>
        public static void AddApi(this IServiceCollection services, IConfiguration config, Action<DbContextOptionsBuilder> configureDataContext)
        {
            // Add Shared Api
            services.AddSharedApi<ProfileMiddlewareService>(config, typeof(AuditLogApiStartup), addHealthCheck: false);

            // Add Koa servicese, This pointer is needed to load the current assembly because of .Net load the libraries dinamically.
            var businessPointerType = typeof(Business.BusinessPointer);
            var businessAssembly = businessPointerType.GetTypeInfo().Assembly;
            var infraAssembly = typeof(Shared.Micro.Repositories.BusinessPointer).GetTypeInfo().Assembly;
            var domainServicesPointer = typeof(Shared.Micro.Domain.Services.BusinessPointer).GetTypeInfo().Assembly;
            var assembly = typeof(UVP.Shared.Micro.Domain.Services.Contracts.BusinessPointer).GetTypeInfo().Assembly;

            services.AddKoaEntityFramework<DataContext, EfUnitOfWorkParent>(configureDataContext);
            services.EfUnitOfWorkParentOverride();
            services.AddObjectMapper(ServiceLifetime.Scoped);
            services.AddAutoMapperToRegister(businessAssembly, infraAssembly);
            services.AddKoaMapperRepositories(businessAssembly);
            services.AddKoaMapperRepositories(infraAssembly);
            services.AddKoaMapperRepositories(domainServicesPointer);

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IPortfolioRepository, PortfolioRepository>();
            // Add Internal services using [Injectable(....)]
            services.ScanInjections(new[] { businessAssembly, infraAssembly, domainServicesPointer, assembly });
            services.ScanCQRSHandlers(new[] { businessAssembly, infraAssembly, domainServicesPointer, assembly });

            // Add Service Bus Events
            services.AddEventHandler<AuditableEventHandler, AuditableEvent>();
            services.AddEventHandler<AuditLogActivityEventHandler, AuditLogActivityEvent>();


            services.AddUVPHttpIntegration().AddHeaderPropagation();
            services.AddUVPHttpAzureCognitiveSearch();
        }        
    }
}
