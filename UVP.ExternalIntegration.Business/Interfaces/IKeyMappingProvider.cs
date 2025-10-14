namespace UVP.ExternalIntegration.Business.Interfaces
{
    using System.Collections.Generic;
    using UVP.ExternalIntegration.Domain.Entity.Integration;

    /// <summary>
    /// Provides a mapping from external payload keys to internal model keys (e.g., externalRequestId -> CandidateId, IndexNumber -> CandidateId).
    /// The mapping can be endpoint-specific (pulled from IntegrationEndpointConfiguration).
    /// </summary>
    public interface IKeyMappingProvider
    {
        /// <summary>
        /// Returns a dictionary mapping external keys to internal keys for the given endpoint.
        /// </summary>
        IDictionary<string, string> GetKeyMap(IntegrationEndpointConfiguration endpoint);
    }
}
