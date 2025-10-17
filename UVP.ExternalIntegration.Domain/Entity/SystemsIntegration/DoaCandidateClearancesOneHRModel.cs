namespace UVP.ExternalIntegration.Domain.Entity.Integration
{
    using System;
    using UVP.ExternalIntegration.Domain.Entity.SystemsIntegration;

    /// <summary>
    /// Database entity for tracking OneHR clearance details.
    /// </summary>
    public class DoaCandidateClearancesOneHRModel
    {
        public long Id { get; set; }
        public long DoaCandidateId { get; set; }
        public long CandidateId { get; set; }
        public long DoaId { get; set; }
        public string? DoaCandidateClearanceId { get; set; }
        public string? RVCaseId { get; set; }
        public DateTime? RequestedDate { get; set; }
        public bool IsCompleted { get; set; }
        public int Retry { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual DoaCandidateModel? DoaCandidate { get; set; }
        public virtual CandidateModel? Candidate { get; set; }
    }
}
