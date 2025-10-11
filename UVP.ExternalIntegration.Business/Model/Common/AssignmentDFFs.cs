namespace UVP.ExternalIntegration.Business.Model.Common
{
    public class AssignmentDFFs
    {
        public AssignmentDFFsItems[] Items { get; set; }
    }

    public class AssignmentDFFsItems
    {
        public long? AssignmentId { get; set; }
        public string? EffectiveStartDate { get; set; }
        public string? EffectiveEndDate { get; set; }
        public int? EffectiveSequence { get; set; }
        public string? EffectiveLatestChange { get; set; }
        public string? AtlasCompany { get; set; }
        public string? LocationEntryDate { get; set; }
        public string? ApaLocation { get; set; }
        public string? ApaLocationEntryDate { get; set; }
        public string? TelecommuteLocation { get; set; }
        public string? PostAdjustmentLocation { get; set; }
        public string? SpouseAllowanceLocation { get; set; }
        public string? StaffDuesLocation { get; set; }
        public string? WellBeingDifferentialLocation { get; set; }
        public string? SecurityChargeLocation { get; set; }
        public string? LeaveStatus { get; set; }
        public string? ReleaseAgency { get; set; }
        public string? ReceivingAgency { get; set; }
        public string? ReimbursementEmployerBenefitPo { get; set; }
        public string? SpecialPostAllowanceAssignment { get; set; }
        public string? AdjustedNextGradeStepDate { get; set; }
        public string? __FLEX_Context { get; set; }
        public WorkerLink[]? Links { get; set; }
    }
}
