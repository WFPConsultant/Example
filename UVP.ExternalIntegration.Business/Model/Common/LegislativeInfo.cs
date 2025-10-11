using Newtonsoft.Json;

namespace UVP.ExternalIntegration.Business.Model.Common
{
    /// <summary>
    /// LegislativeInfo class.
    /// </summary>
    public class LegislativeInfo
    {
        /// <summary>
        /// Gets or sets the LegislationCode.
        /// </summary>
        [JsonProperty("LegislationCode")]
        public string? LegislationCode { get; set; }

        /// <summary>
        /// Gets or sets the Gender.
        /// </summary>
        [JsonProperty("Gender")]
        public string? Gender { get; set; }

        /// <summary>
        /// Gets or sets the MaritalStatus.
        /// </summary>
        [JsonProperty("MaritalStatus")]
        public string? MaritalStatus { get; set; }
    }
}
