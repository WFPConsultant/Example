using Newtonsoft.Json;

namespace UVP.ExternalIntegration.Business.Model.Common
{
    /// <summary>
    /// Email class.
    /// </summary>
    public class Email
    {
        /// <summary>
        /// Gets or sets the EmailAddress.
        /// </summary>
        [JsonProperty("EmailAddress")]
        public string? EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the EmailType.
        /// </summary>
        [JsonProperty("EmailType")]
        public string? EmailType { get; set; }
    }
}
