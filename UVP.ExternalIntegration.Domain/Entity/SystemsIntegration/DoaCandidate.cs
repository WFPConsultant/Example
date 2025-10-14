namespace UVP.ExternalIntegration.Domain.Entity.SystemsIntegration
{
    using System;
    using Microsoft.EntityFrameworkCore;

    [Keyless]
    public class DoaCandidate
    {
        public long Id { get; set; }
        public long DoaId { get; set; }
        public long CandidateId { get; set; }
        public string? Department { get; set; }
        public string? RequestorName { get; set; }
        public string? RequestorEmail { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
