namespace UVP.ExternalIntegration.Domain.Repository.Interfaces
{
    using System.Threading.Tasks;
    using UVP.ExternalIntegration.Domain.Entity.Integration;

    public interface IIntegrationEndpointRepository : IGenericRepository<IntegrationEndpointConfigurationModel>
    {
        Task<IntegrationEndpointConfigurationModel?> GetActiveEndpointAsync(string integrationType, string operation);
    }
}
