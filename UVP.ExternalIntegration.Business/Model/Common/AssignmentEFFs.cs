namespace UVP.ExternalIntegration.Business.Model.Common
{
    public class AssignmentEFFs
    {
        public AssignmentEFFsItems[] Items { get; set; }
    }

    public class AssignmentEFFsItems
    {
        public string? EffectiveEndDate { get; set; }
        public string? EffectiveLatestChange { get; set; }
        public string? EffectiveStartDate { get; set; }
        public int? EffectiveSequence { get; set; }
        public long? AssignmentId { get; set; }
        public string? AssignmentType { get; set; }
        public string? CategoryCode { get; set; }
        public WorkerLink[] Links { get; set; }
    }
}
