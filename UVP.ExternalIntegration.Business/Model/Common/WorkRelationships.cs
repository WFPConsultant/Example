namespace UVP.ExternalIntegration.Business.Model.Common
{
    using System;

    public class WorkRelationships
    {
        public WorkRelationshipsItems[] Items { get; set; }
    }

    public class WorkRelationshipsItems
    {
        public long? PeriodOfServiceId { get; set; }
        public string? LegislationCode { get; set; }
        public long? LegalEntityId { get; set; }
        public string? LegalEmployerName { get; set; }
        public string? WorkerType { get; set; }
        public bool? PrimaryFlag { get; set; }
        public string? StartDate { get; set; }
        public string? LegalEmployerSeniorityDate { get; set; }
        public string? EnterpriseSeniorityDate { get; set; }
        public bool? OnMilitaryServiceFlag { get; set; }
        public string? WorkerNumber { get; set; }
        public string? ReadyToConvertFlag { get; set; }
        public string? TerminationDate { get; set; }
        public string? NotificationDate { get; set; }
        public string? LastWorkingDate { get; set; }
        public string? RevokeUserAccess { get; set; }
        public string? RecommendedForRehire { get; set; }
        public string? RecommendationReason { get; set; }
        public string? RecommendationAuthorizedByPersonId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string? ProjectedTerminationDate { get; set; }
        public Assignments Assignments { get; set; }
        public Contracts Contracts { get; set; }
        public WorkerLink[] Links { get; set; }
    }
}
