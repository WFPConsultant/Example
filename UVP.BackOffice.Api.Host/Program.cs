using UVP.Shared.Micro.Host.Options.ApplicationInsights;
using UVP.Shared.Micro.Host.Options.AzureServiceBus;

namespace UVP.BackOffice.Api.Host
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using UVP.Shared.Micro.Host;

    public class Program
    {
        public static async Task Main(string[] args) => await BaseProgram.BaseMain<Startup>(args, async (host) =>
            {
                using (var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    scope.ServiceProvider.RegisterTraceLister();
                    await scope.ServiceProvider.InitializeAzureServiceBus();
                }
            });
    }
}
