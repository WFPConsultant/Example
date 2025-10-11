namespace UVP.ExternalIntegration.Business.Model.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// WorkerProperty class.
    /// </summary>
    public class WorkerProperty
    {
        /// <summary>
        /// Gets or sets the ChangeIndicator.
        /// </summary>
        [JsonProperty("changeIndicator")]
        public string? ChangeIndicator { get; set; }
    }
}
