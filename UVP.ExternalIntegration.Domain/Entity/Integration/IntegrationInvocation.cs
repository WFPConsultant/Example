namespace UVP.ExternalIntegration.Domain.Entity.Integration
{
    using System;

    /// <summary>
    /// Database entity for tracking integration invocations.
    /// </summary>
    public class IntegrationInvocation
    {
        public long IntegrationInvocationId { get; set; }
        public string IntegrationType { get; set; } = string.Empty;
        public string IntegrationOperation { get; set; } = string.Empty;
        public string IntegrationStatus { get; set; } = string.Empty;
        public int AttemptCount { get; set; }
        public DateTime? NextRetryTime { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedUser { get; set; } = string.Empty;
        public DateTime? UpdatedOn { get; set; }
        public string? UpdatedUser { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
