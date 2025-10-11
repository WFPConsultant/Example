namespace UVP.ExternalIntegration.Business.Model.Common
{
    using Newtonsoft.Json;

    public class Versions
    {
        [JsonProperty("items")]
        public VersionItems[] Items { get; set; }

        [JsonProperty("links")]
        public WorkerLink[] Links { get; set; }

    }
}
