namespace UVP.ExternalIntegration.Domain.Integration.DTOs
{
    /// <summary>
    /// Request DTO for initiating integration operations.
    /// </summary>
    public class IntegrationRequestDto
    {
        public long DoaCandidateId { get; set; }
        public long CandidateId { get; set; }
        public string IntegrationType { get; set; } = string.Empty;
        public string? IntegrationOperation { get; set; }
    }
}
