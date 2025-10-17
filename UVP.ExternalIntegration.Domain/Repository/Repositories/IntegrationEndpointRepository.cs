namespace UVP.ExternalIntegration.Domain.Repository.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using UVP.ExternalIntegration.Domain.Entity.Integration;
    using UVP.ExternalIntegration.Domain.Entity.SystemsIntegration;
    using UVP.ExternalIntegration.Domain.Repository.Interfaces;

    public class IntegrationEndpointRepository : GenericRepository<IntegrationEndpointConfiguration>, IIntegrationEndpointRepository
    {
        public IntegrationEndpointRepository(DataSytemsIntegrationContext context) : base(context)
        {
        }

        public async Task<IntegrationEndpointConfiguration?> GetActiveEndpointAsync(string integrationType, string operation)
        {
            return await _dbSet
                .Where(e => e.IntegrationType == integrationType
                    && e.IntegrationOperation == operation
                    && e.IsActive)
                .FirstOrDefaultAsync();
        }
    }
}
