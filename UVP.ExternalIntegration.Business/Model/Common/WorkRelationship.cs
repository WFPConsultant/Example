using Newtonsoft.Json;

namespace UVP.ExternalIntegration.Business.Model.Common
{
    /// <summary>
    /// WorkRelationship class.
    /// </summary>
    public class WorkRelationship
    {
        /// <summary>
        /// Gets or sets the WorkerType.
        /// </summary>
        [JsonProperty("WorkerType")]
        public string? WorkerType { get; set; }

        /// <summary>
        /// Gets or sets the LegalEntityId.
        /// </summary>
        [JsonProperty("LegalEntityId")]
        public string? LegalEntityId { get; set; }

        /// <summary>
        /// Gets or sets the Assignments.
        /// </summary>

        [JsonProperty("assignments")]
        public Assignment[]? Assignments { get; set; }

        /// <summary>
        /// Gets or sets the Contracts.
        /// </summary>
        [JsonProperty("contracts")]
        public Contract[]? Contracts { get; set; }
    }
}
