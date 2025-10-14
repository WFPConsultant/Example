namespace UVP.ExternalIntegration.Domain.Entity.Integration
{
    using System;

    /// <summary>
    /// Database entity for tracking OneHR clearance details.
    /// This is a NEW table specific to integration functionality.
    /// </summary>
    public class DoaCandidateClearancesOneHR
    {
        public long Id { get; set; }
        public long DoaCandidateId { get; set; }
        public long CandidateId { get; set; }
        public long DoaId { get; set; }
        public string? DoaCandidateClearanceId { get; set; }
        public string? RVCaseId { get; set; }
        public DateTime RequestedDate { get; set; }
        public bool IsCompleted { get; set; }
        public int Retry { get; set; }
        public DateTime? CompletionDate { get; set; }
    }
}
