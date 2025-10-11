namespace UVP.ExternalIntegration.Business.Model.Common
{
    using System;

    public class GradeSteps
    {
        public GradeStepsItems[] Items { get; set; }
    }

    public class GradeStepsItems
    {
        public long? AssignGradeStepId { get; set; }
        public string? EffectiveStartDate { get; set; }
        public string? EffectiveEndDate { get; set; }
        public long? GradeStepId { get; set; }
        public string? GradeStepName { get; set; }
        public long? GradeId { get; set; }
        public string? ActionCode { get; set; }
        public string? ReasonCode { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public WorkerLink[]? Links { get; set; }
    }
}
