namespace UVP.ExternalIntegration.Business.Model.Response
{
    using Newtonsoft.Json;

    public class  PayrollEntryResponse
    {
        [JsonProperty("ElementEntryId")]
        public string ElementEntryId { get; set; }

        [JsonProperty("EffectiveStartDate")]
        public string EffectiveStartDate { get; set; }

        [JsonProperty("EffectiveEndDate")]
        public string EffectiveEndDate { get; set; }

        [JsonProperty("CreatorType")]
        public string CreatorType { get; set; }

        [JsonProperty("ElementTypeId")]
        public string ElementTypeId { get; set; }

        [JsonProperty("EntryType")]
        public string EntryType { get; set; }

        [JsonProperty("EntrySequence")]
        public string EntrySequence { get; set; }

        [JsonProperty("PersonId")]
        public string PersonId { get; set; }

        [JsonProperty("Reason")]
        public string Reason { get; set; }

        [JsonProperty("Subpriority")]
        public string Subpriority { get; set; }

        [JsonProperty("PersonNumber")]
        public string PersonNumber { get; set; }

        [JsonProperty("AssignmentId")]
        public string AssignmentId { get; set; }

        [JsonProperty("AssignmentNumber")]
        public string AssignmentNumber { get; set; }

        [JsonProperty("PayrollRelationshipNumber")]
        public string PayrollRelationshipNumber { get; set; }

        [JsonProperty("ElementName")]
        public string ElementName { get; set; }

        [JsonProperty("UsageLevel")]
        public string UsageLevel { get; set; }
               

    }
}
