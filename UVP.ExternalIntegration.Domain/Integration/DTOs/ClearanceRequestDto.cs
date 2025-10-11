namespace UVP.ExternalIntegration.Domain.Integration.DTOs
{
    using Newtonsoft.Json;

    public class ClearanceRequestDto
    {
        [JsonProperty("ClearanceRequest")]
        public ClearanceRequest? ClearanceRequest { get; set; }
    }

    public class ClearanceRequest
    {
        [JsonProperty("externalRequestId")]
        public string? ExternalRequestId { get; set; }

        [JsonProperty("externalBatchId")]
        public string? ExternalBatchId { get; set; }

        [JsonProperty("lastName")]
        public string? LastName { get; set; }

        [JsonProperty("middleName")]
        public string? MiddleName { get; set; }

        [JsonProperty("firstName")]
        public string? FirstName { get; set; }

        [JsonProperty("indexNo")]
        public string? IndexNo { get; set; }

        [JsonProperty("gender")]
        public string? Gender { get; set; }

        [JsonProperty("dateOfBirth")]
        public string? DateOfBirth { get; set; }

        // ... other fields
    }
}
