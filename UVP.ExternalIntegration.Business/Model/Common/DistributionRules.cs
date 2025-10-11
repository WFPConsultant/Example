namespace UVP.ExternalIntegration.Business.Model.Common
{
    using Newtonsoft.Json;

    public class DistributionRules
    {

        [JsonProperty("items")]
        public DistributionItems[] Items { get; set; }

        [JsonProperty("links")]
        public WorkerLink[] Links { get; set; }
    }
}
