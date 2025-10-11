using Newtonsoft.Json;

namespace UVP.ExternalIntegration.Business.Model.Common
{
    /// <summary>
    /// Contract class.
    /// </summary>
    public class Contract
    {
        /// <summary>
        /// Gets or sets the ContractType.
        /// </summary>
        [JsonProperty("ContractType")]
        public string? ContractType { get; set; }

        /// <summary>
        /// Gets or sets the InitialDuration.
        /// </summary>
        [JsonProperty("InitialDuration")]
        public string? InitialDuration { get; set; }

        /// <summary>
        /// Gets or sets the InitialDurationUnits.
        /// </summary>
        [JsonProperty("InitialDurationUnits")]
        public string? InitialDurationUnits { get; set; }

        /// <summary>
        /// Gets or sets the ContractsDFF.
        /// </summary>
        [JsonProperty("contractsDFF")]
        public ContractDFF[] ContractsDFF { get; set; }
    }
}
