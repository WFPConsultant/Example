namespace UVP.ExternalIntegration.Business.Interfaces
{
    using System.Threading.Tasks;
    using UVP.ExternalIntegration.Domain.Entity.Integration;

    public interface IResultMapperService
    {
        /// <summary>
        /// Map an external response body back into UVP domain state for the given invocation.
        /// Should not rely on DoaCandidateId/ReferenceId stored on the invocation row.
        /// </summary>
        Task ProcessResponseAsync(IntegrationInvocation invocation, string responseBody, string integrationType);
    }
}
