using Newtonsoft.Json;

namespace UVP.ExternalIntegration.Business.Model.Common
{
    /// <summary>
    /// Address class.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Gets or sets the AddressType.
        /// </summary>
        [JsonProperty("AddressType")]
        public string? AddressType { get; set; }

        /// <summary>
        /// Gets or sets the AddressLine1.
        /// </summary>
        [JsonProperty("AddressLine1")]
        public string? AddressLine1 { get; set; }

        /// <summary>
        /// Gets or sets the AddressLine2.
        /// </summary>
        [JsonProperty("AddressLine2")]
        public string? AddressLine2 { get; set; }

        /// <summary>
        /// Gets or sets the Country.
        /// </summary>
        [JsonProperty("Country")]
        public string? Country { get; set; }

        /// <summary>
        /// Gets or sets the PostalCode.
        /// </summary>
        [JsonProperty("PostalCode")]
        public string? PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the TownOrCity.
        /// </summary>
        [JsonProperty("TownOrCity")]
        public string? TownOrCity { get; set; }

        /// </summary>
        [JsonProperty("Region2")]
        public string? Region3 { get; set; }
    }
}
