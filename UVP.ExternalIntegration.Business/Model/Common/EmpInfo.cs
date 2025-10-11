using Newtonsoft.Json;

namespace UVP.ExternalIntegration.Business.Model.Common
{
    /// <summary>
    /// EmpInfo class.
    /// </summary>
    public class EmpInfo
    {
        /// <summary>
        /// Gets or sets the FirstName.
        /// </summary>
        [JsonProperty("FirstName")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the LastName.
        /// </summary>
        [JsonProperty("LastName")]
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the LegislationCode.
        /// </summary>
        [JsonProperty("LegislationCode")]
        public string? LegislationCode { get; set; }
    }
}
