namespace UVP.ExternalIntegration.Business.Model.Response
{
    using Newtonsoft.Json;

    public class ErrorModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("o:errorDetails")]
        public OErrorDetail[] OErrorDetails { get; set; }
    }

    public class OErrorDetail
    {
        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("o:errorCode")]
        public string OErrorCode { get; set; }
    }
}
