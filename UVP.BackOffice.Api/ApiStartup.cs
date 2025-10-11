using UVP.ExternalIntegration.Api;
using UVP.Integration.Api;
using UVP.Shared.Micro.Api;
using UVP.Shared.Micro.Api.Services;
using UVP.Shared.Micro.Domain.Configuration;
using UVP.Shared.Micro.Host.Extensions;

namespace UVP.BackOffice.Api
{
    using System;
    using System.Reflection;
    using Koa.Platform.Injection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Shared.Micro.Business.Sql;
    using Shared.Micro.Entities.Sql;
    using UVP.Core.EventGrid.Extensions;
    using UVP.Doa.Domain.Authorization;
    using UVP.Profile.Business.Repository;
    using UVP.Shared.Http.Integration;
    using UVP.Shared.Micro.Api.Middleware.ExceptionMiddleware;
    using UVP.TaskProcessor.Domain;

    /// <summary>
    /// Api startUp with the Api services injection.
    /// </summary>
    public static class ApiStartup
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
            services.AddSharedApi<ProfileMiddlewareService>(config, typeof(ApiStartup), assemblyToAdd:
            [
                typeof(ApiStartupExternalIntegration).Assembly,
                typeof(ApiStartupIntegration).Assembly
            ]);

            // Add Koa servicese, This pointer is needed to load the current assembly because of .Net load the libraries dinamically.
            var businessPointerType = typeof(Business.BusinessPointer);
            var businessAssembly = businessPointerType.GetTypeInfo().Assembly;
            var infraAssembly = typeof(Shared.Micro.Repositories.BusinessPointer).GetTypeInfo().Assembly;
            var domainServicesPointer = typeof(Shared.Micro.Domain.Services.BusinessPointer).GetTypeInfo().Assembly;            
            var assemblyDocument = typeof(UVP.Shared.Micro.Domain.Documents.BusinessPointer).GetTypeInfo().Assembly;
            services.AddObjectMapper(ServiceLifetime.Scoped);
            services.AddAutoMapperToRegister(businessAssembly, infraAssembly, domainServicesPointer);
            services.AddKoaMapperRepositories(infraAssembly);
            services.AddKoaEntityFramework<DataContext, EfUnitOfWorkParent>(configureDataContext);
            services.EfUnitOfWorkParentOverride();
            // Add Internal services using [Injectable(....)]
            services.ScanInjections(new[] { businessAssembly, infraAssembly, domainServicesPointer });
            services.ScanCQRSHandlers(new[] { businessAssembly, infraAssembly, domainServicesPointer });

            services.AddUVPHttpMasterTable().AddHeaderPropagation();            
            services.AddUVPHttpIdentity().AddHeaderPropagation();

            //Service bus.
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IPortfolioRepository, PortfolioRepository>();

            services.AddExceptionMiddleware();
            services.AddUVPHttpAzureCognitiveSearch();
            services.AddUVPHttpAIRecruitmentFiltering(config);
            services.AddTaskProcessorClient(config);
            services.AddDependencies(config, businessAssembly, infraAssembly, domainServicesPointer, assemblyDocument);
            
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="configuration">Represents a set of key/value application configuration properties.</param>
        /// <param name="configureEndpoints">Endpoint route builder.</param>
        public static void UseApi(this IApplicationBuilder app, IConfiguration configuration, Action<IEndpointRouteBuilder> configureEndpoints = default) => app.UseSharedApi(configuration, configureEndpoints);
    }
}
