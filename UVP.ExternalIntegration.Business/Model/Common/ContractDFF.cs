using Newtonsoft.Json;

namespace UVP.ExternalIntegration.Business.Model.Common
{
    /// <summary>
    /// ContractDFF class.
    /// </summary>
    public class ContractDFF
    {
        /// <summary>
        /// Gets or sets the __FLEX_Context.
        /// </summary>
        [JsonProperty("__FLEX_Context")]
        public string? __FLEX_Context { get; set; }

        /// <summary>
        /// Gets or sets the ContractClause.
        /// </summary>
        [JsonProperty("contractClause")]
        public string? ContractClause { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        [JsonProperty("status")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the EffectiveEndDate1.
        /// </summary>
        [JsonProperty("effectiveEndDate1")]
        public string? EffectiveEndDate1 { get; set; }

        /// <summary>
        /// Gets or sets the DonorCountryEligibility.
        /// </summary>
        [JsonProperty("donorCountryEligibility")]
        public string? DonorCountryEligibility { get; set; }

        /// <summary>
        /// Gets or sets the EntitledToInternationalEntitle.
        /// </summary>
        [JsonProperty("entitledToInternationalEntitle")]
        public string? EntitledToInternationalEntitle { get; set; }
    }
}
