namespace UVP.ExternalIntegration.Domain.Entity.Integration
{
    using System;
    using UVP.ExternalIntegration.Domain.Entity.SystemsIntegration;

    /// <summary>
    /// Database entity for tracking clearance status.
    /// </summary>
    public class DoaCandidateClearancesModel
    {
        public long Id { get; set; }
        public long DoaCandidateId { get; set; }
        public string RecruitmentClearanceCode { get; set; }
        public string RecruitmentClearanceTableCode { get; set; }
        public DateTime RequestedDate { get; set; }
        public string StatusCode { get; set; } = string.Empty;
        public string? LinkDetailRemarks { get; set; }
        public string? AdditionalRemarks { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? Outcome { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public virtual DoaCandidateModel? DoaCandidate { get; set; }
    }
}
