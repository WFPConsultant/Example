namespace UVP.ExternalIntegration.Domain.Integration.DTOs
{
    using System;
    using Newtonsoft.Json;

    public class ClearanceResponseDto
    {
        [JsonProperty("clearanceRequestId")]
        public string? ClearanceRequestId { get; set; }

        [JsonProperty("clearanceResponseId")]
        public string? ClearanceResponseId { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("statusDate")]
        public DateTime? StatusDate { get; set; }
    }
}
