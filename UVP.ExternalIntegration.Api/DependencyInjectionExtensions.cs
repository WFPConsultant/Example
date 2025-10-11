namespace UVP.ExternalIntegration.Api
{
    using System;
    using System.Net.Http.Headers;
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using UVP.ExternalIntegration.Business.Services;
    using UVP.ExternalIntegration.Business.Services.Contracts;
    using UVP.ExternalIntegration.Domain;
    using UVP.ExternalIntegration.Domain.Entity.Doa;
    using UVP.ExternalIntegration.Domain.Entity.Users;
    using UVP.ExternalIntegration.Domain.Repository;
    using UVP.ExternalIntegration.Domain.Repository.Doa;
    using UVP.ExternalIntegration.Domain.Repository.Users;
    using UVP.ExternalIntegration.ErrorValidationFramework.Repository;

    public static class DependencyInjectionExtensions
    {
        public static void AddExternalIntegrationApi(this IServiceCollection services)
        {
            services.AddOptions<ExternalIntegrationOptions>().Configure<IConfiguration>((settings, config) => config.GetSection(ExternalIntegrationOptions.Section).Bind(settings));

            services.AddHttpClient<IExternalIntegrationService, ExternalIntegrationService>(ExternalIntegrationOptions.LogicalName, (provider, c) =>
            {
                var options = provider.GetRequiredService<IOptions<ExternalIntegrationOptions>>().Value;
                var authenticationString = $"{options.Username}:{options.Password}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

                // c.DefaultRequestHeaders.Add("Accept", "*/*");
                // c.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate,br");
                // c.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                // c.DefaultRequestHeaders.Add("Effective-Of", "RangeStartDate=2022-02-26");
                c.Timeout = TimeSpan.FromSeconds(300);
                c.DefaultRequestHeaders.Add("REST-Framework-Version", "4");
                c.BaseAddress = new Uri(options.Url);
            });

            services.AddDbContext<DataUserContext>();
            services.AddDbContext<DataDoaContext>();
            services.AddDbContext<ErrorDBContext>();
            services.AddScoped<ICandidateRepository, CandidateRepository>();
            services.AddScoped<IAssignmentRepository, AssignmentRepository>();
            services.AddScoped<IDBLoggerRepository, DBLoggerRepository>();
            services.AddScoped<DapperContext>();
        }
    }
}
