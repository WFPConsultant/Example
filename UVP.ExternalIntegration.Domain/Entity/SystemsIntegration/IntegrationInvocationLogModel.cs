namespace UVP.ExternalIntegration.Domain.Entity.Integration
{
    using System;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Database entity for logging integration invocation details.
    /// </summary>

    [Keyless]
    public class IntegrationInvocationLogModel
    {
        public long IntegrationInvocationLogId { get; set; }
        public long IntegrationInvocationId { get; set; }
        public string? RequestPayload { get; set; }
        public string? ResponsePayload { get; set; }
        public int? ResponseStatusCode { get; set; }
        public string IntegrationStatus { get; set; } = string.Empty;
        public DateTime? RequestSentOn { get; set; }
        public DateTime? ResponseReceivedOn { get; set; }
        public long? ResponseTimeMs { get; set; }
        public string? ErrorDetails { get; set; }
        public int LogSequence { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedUser { get; set; } = string.Empty;        
        public virtual IntegrationInvocationModel? IntegrationInvocation { get; set; }
    }
}
