using UVP.AuditLog.Api;
using UVP.ExternalIntegration.Api;
using UVP.Integration.Api;
using UVP.Integration.Business;
using UVP.MasterTables.Api;

namespace UVP.BackOffice.Api.Host
{
    using Hangfire;
    using Hangfire.SqlServer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using UVP.Shared.Micro.Entities.Options;
    using UVP.Shared.Micro.Host;

    public class Startup : BaseStartup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
            : base(env, configuration)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);
            app.UseApi(this.Configuration);
            app.UseIronPdfModule(this.Configuration, "IronPdf.License.LicenseKey");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddApi(this.Configuration, opt =>
            {
                opt.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"))
                    .ConfigureWarnings(e => e.Ignore(CoreEventId.NavigationBaseIncludeIgnored));
            });
            
            ApiStartupExternalIntegration.AddApi(services, this.Configuration, opt =>
            {
                opt.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"))
                    .ConfigureWarnings(e => e.Ignore(CoreEventId.NavigationBaseIncludeIgnored));
            }, addHealthCheck:false);
            
            ApiStartupIntegration.AddApi(services, this.Configuration, opt =>
            {
                opt.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"))
                    .ConfigureWarnings(e => e.Ignore(CoreEventId.NavigationBaseIncludeIgnored));
            }, ServiceLifetime.Transient, addHealthCheck:false);
            
            MasterTableApiStartup.AddApi(services, this.Configuration, opt =>
            {
                opt.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"))
                    .ConfigureWarnings(e => e.Ignore(CoreEventId.NavigationBaseIncludeIgnored));
            });
            
            AuditLogApiStartup.AddApi(services, this.Configuration, opt =>
            {
                opt.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"))
                    .ConfigureWarnings(e => e.Ignore(CoreEventId.NavigationBaseIncludeIgnored));
            });
            
            base.ConfigureServices(services);

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(() => new SqlConnection(this.Configuration.GetConnectionString("HangFireConnection")), new SqlServerStorageOptions
                {
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true,
                    TryAutoDetectSchemaDependentOptions = false, // Defaults to `true`
                }));

            // Configures ExperienceOptions from the configuration section.
            services.AddOptions<ExperienceOptions>().Configure<IConfiguration>((settings, config) => config.GetSection(ExperienceOptions.Section).Bind(settings));
            services.AddOptions<OnBehalfHiddenTasksOptions>().Configure<IConfiguration>((settings, config) => config.GetSection(OnBehalfHiddenTasksOptions.Section).Bind(settings));
            services.AddOptions<BeneficaryInformationConfigOptions>().Configure<IConfiguration>((settings, config) => config.GetSection(BeneficaryInformationConfigOptions.Section).Bind(settings));

        }
    }
}
