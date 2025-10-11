namespace UVP.ExternalIntegration.Business.Model.Request
{
    using Newtonsoft.Json;

    public class NationalityRequestModel
    {
        [JsonProperty("PeiInformationCategory")]
        public string? PeiInformationCategory { get; set; }

        [JsonProperty("PeiAttributeCategory")]
        public string? PeiAttributeCategory { get; set; }

        [JsonProperty("nationalityType")]
        public string? nationalityType { get; set; }

        [JsonProperty("effectiveDate")]
        public string? effectiveDate { get; set; }

        [JsonProperty("reason")]
        public string? reason { get; set; }

        [JsonProperty("nationality")]
        public string? nationality { get; set; }
    }
}
