namespace UVP.ExternalIntegration.Business.Model.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// APIStatusDisplay of query class.
    /// </summary>
    public class APIStatusDisplayModel
    {
        /// <summary>
        /// Gets or sets APIName.
        /// </summary>
        [JsonProperty("APIName")]
        public string? APIName { get; set; }

        /// <summary>
        /// Gets or sets APICode.
        /// </summary>
        [JsonProperty("APICode")]
        public string? APICode { get; set; }

        /// <summary>
        /// Gets or sets APIStatus.
        /// </summary>
        [JsonProperty("APIStatus")]
        public string? APIStatus { get; set; }
    }
}
