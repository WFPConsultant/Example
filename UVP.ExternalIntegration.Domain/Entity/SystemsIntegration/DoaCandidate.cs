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
        public long AssignmentId { get; set; }
        public DateTime? TentativeTravelDate { get; set; }
        public DateTime? ContractCalculatedEndDate { get; set; }
    }
}
