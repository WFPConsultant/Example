namespace UVP.ExternalIntegration.Business.Model.Response
{
    using Newtonsoft.Json;
using UVP.ExternalIntegration.Business.Model.Common;

    public class PayrollRelationshipResponse
    {
        [JsonProperty("AssignedPayrollId")]
        public string AssignedPayrollId { get; set; }

        [JsonProperty("PayrollId")]
        public string PayrollId { get; set; }

        [JsonProperty("StartDate")]
        public string StartDate { get; set; }

        [JsonProperty("EndDate")]
        public string EndDate { get; set; }

        [JsonProperty("TimeCardRequired")]
        public string? TimeCardRequired { get; set; }

        [JsonProperty("OverridingPeriodId")]
        public string? OverridingPeriodId { get; set; }

        [JsonProperty("EffectiveStartDate")]
        public string EffectiveStartDate { get; set; }

        [JsonProperty("EffectiveEndDate")]
        public string EffectiveEndDate { get; set; }

        [JsonProperty("Lsed")]
        public string? Lsed { get; set; }

        [JsonProperty("links")]
        public WorkerLink[] links { get; set; }


    }
}
