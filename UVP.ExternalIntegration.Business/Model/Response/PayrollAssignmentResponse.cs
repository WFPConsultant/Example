namespace UVP.ExternalIntegration.Business.Model.Response
{
    using UVP.ExternalIntegration.Business.Model.Common;
    using Newtonsoft.Json;

    public class PayrollAssignmentResponse
    {
        public PayrollAssignmentResponseItems[] Items { get; set; }
    }

    public class PayrollAssignmentResponseItems
    {
        public long? PayrollRelationshipId { get; set; }
        public string? PayrollRelationshipNumber { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? PersonNumber { get; set; }
        public string? Country { get; set; }
        public long? PartyId { get; set; }
        public string? PartyNumber { get; set; }
        public string? EffectiveEndDate { get; set; }
        public string? EffectiveStartDate { get; set; }
        public string? OverridingPeriodId { get; set; }

        [JsonProperty("payrollAssignments")]
        public PayrollAssignmentDetails[] PayrollAssignments { get; set; }

    }


    public class PayrollAssignment
    {
        public PayrollAssignmentDetails[] Items { get; set; }
    }

    public class PayrollAssignmentDetails
    {
        public long? RelationshipGroupId { get; set; }
        public string? AssignmentNumber { get; set; }
        public string? EffectiveStartDate { get; set; }
        public string? EffectiveEndDate { get; set; }
        public string? TimeCardRequired { get; set; }
        public string? OverridingPeriodId { get; set; }
        public WorkerLink[] links { get; set; }
    }
}
