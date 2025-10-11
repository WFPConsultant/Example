namespace UVP.ExternalIntegration.Business.Model.Common
{
    using System.Net;
    using Newtonsoft.Json;

    /// <summary>
    /// ReturnObject class.
    /// </summary>
    public class ReturnObjectPayrollOTE
    {
        [JsonProperty("responsecode")]
        public HttpStatusCode ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets Message.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets Response.
        /// </summary>
        [JsonIgnore]
        public dynamic Response { get; set; }
    }
}
