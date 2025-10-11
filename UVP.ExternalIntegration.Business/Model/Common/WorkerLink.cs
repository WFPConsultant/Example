namespace UVP.ExternalIntegration.Business.Model.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// WorkerLink class.
    /// </summary>
    public class WorkerLink
    {
        /// <summary>
        /// Gets or sets the Rel.
        /// </summary>
        [JsonProperty("rel")]
        public string? Rel { get; set; }

        /// <summary>
        /// Gets or sets the Href.
        /// </summary>
        [JsonProperty("href")]
        public string? Href { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the Kind.
        /// </summary>
        [JsonProperty("kind")]
        public string? Kind { get; set; }

        /// <summary>
        /// Gets or sets the Properties.
        /// </summary>
        [JsonProperty("properties")]
        public WorkerProperty Properties { get; set; }
    }
}
