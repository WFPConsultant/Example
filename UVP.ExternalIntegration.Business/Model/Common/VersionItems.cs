namespace UVP.ExternalIntegration.Business.Model.Common
{
    using System;
    using Newtonsoft.Json;

    public class VersionItems
    {
        [JsonProperty("VersionId")]
        public long? VersionId { get; set; }

        [JsonProperty("VersionName")]
        public string? VersionName { get; set; }

        [JsonProperty("LaborScheduleId")]
        public long? LaborScheduleId { get; set; }

        [JsonProperty("VersionStartDate")]
        public DateTime? VersionStartDate { get; set; }

        [JsonProperty("VersionEndDate")]
        public DateTime? VersionEndDate { get; set; }

        [JsonProperty("VersionStatus")]
        public string? VersionStatus { get; set; }

        [JsonProperty("VersionStatusCode")]
        public string? VersionStatusCode { get; set; }

        [JsonProperty("VersionComments")]
        public string? VersionComments { get; set; }

        [JsonProperty("CreatedBy")]
        public string? CreatedBy { get; set; }

        [JsonProperty("CreationDate")]
        public DateTime? CreationDate { get; set; }

        [JsonProperty("LastUpdateDate")]
        public DateTime? LastUpdateDate { get; set; }

        [JsonProperty("LastUpdatedBy")]
        public string? LastUpdatedBy { get; set; }

        [JsonProperty("BusinessUnitId")]
        public long? BusinessUnitId { get; set; }

        [JsonProperty("BusinessUnitLedger")]
        public int? BusinessUnitLedger { get; set; }

        [JsonProperty("distributionRules")]
        public DistributionRules DistributionRules { get; set; }
    }
}
