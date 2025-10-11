namespace UVP.ExternalIntegration.Domain.Repository.Integration
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UVP.ExternalIntegration.Domain.Entity.Integration;

    public interface IIntegrationEndpointRepository
    {
        Task<IntegrationEndpointConfiguration?> GetActiveEndpointAsync(
            string integrationType,
            string operation);

        Task<IEnumerable<IntegrationEndpointConfiguration>> GetAllActiveEndpointsAsync();

        Task<IntegrationEndpointConfiguration> AddAsync(
            IntegrationEndpointConfiguration endpoint);

        Task UpdateAsync(IntegrationEndpointConfiguration endpoint);
    }
}
