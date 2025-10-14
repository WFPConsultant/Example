namespace UVP.ExternalIntegration.Domain.Repository.Interfaces
{
    using System.Threading.Tasks;
    using UVP.ExternalIntegration.Domain.Entity.Integration;

    public interface IIntegrationEndpointRepository : IGenericRepository<IntegrationEndpointConfiguration>
    {
        Task<IntegrationEndpointConfiguration?> GetActiveEndpointAsync(string integrationType, string operation);
    }
}
