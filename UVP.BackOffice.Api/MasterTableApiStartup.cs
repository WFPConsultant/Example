using UVP.Shared.Micro.Api;
using UVP.Shared.Micro.Api.Services;
using UVP.Shared.Micro.Domain.Configuration;
using UVP.Shared.Micro.Host.Extensions;

namespace UVP.MasterTables.Api
{
    using System;
    using System.Reflection;
    using Koa.Platform.Injection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Shared.Micro.Entities.Sql;
    using UVP.Core.EventGrid.Extensions;
    using UVP.Profile.Business.Repository;
    using UVP.Shared.Micro.Business.Sql;

    public static class MasterTableApiStartup
    {
        //public static void AddApi
        // This method gets called by the runtime. Use this method to add services to the container.
        public static void AddApi(this IServiceCollection services, IConfiguration config, Action<DbContextOptionsBuilder> configureDataContext)
        {
            //Add Shared Api
            services.AddSharedApi<ProfileMiddlewareService>(config, typeof(MasterTableApiStartup), addHealthCheck: false);

            // Add Koa servicese, This pointer is needed to load the current assembly because of .Net load the libraries dinamically.
            var businessPointerType = typeof(Business.BusinessPointer);
            var businessAssembly = businessPointerType.GetTypeInfo().Assembly;
            var infraAssembly = typeof(Shared.Micro.Repositories.BusinessPointer).GetTypeInfo().Assembly;
            services.AddKoaEntityFramework<DataContext, EfUnitOfWorkParent>(configureDataContext);
            services.EfUnitOfWorkParentOverride();
            services.AddObjectMapper(ServiceLifetime.Scoped);
            services.AddAutoMapperToRegister(businessAssembly, infraAssembly);
            services.AddKoaMapperRepositories(businessAssembly);
            services.AddKoaMapperRepositories(infraAssembly);

            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IPortfolioRepository, PortfolioRepository>();

            // Add Internal services using [Injectable(....)]
            services.ScanInjections(new[] { businessAssembly, infraAssembly });
            services.ScanCQRSHandlers(new[] { businessAssembly });
            services.AddUVPHttpAzureCognitiveSearch();
        }
    }
}
