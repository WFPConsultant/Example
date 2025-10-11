namespace UVP.ExternalIntegration.Business.Model.Response
{
    using System;
    using UVP.ExternalIntegration.Business.Model.Common;
    using Newtonsoft.Json;

    public class ErpCoaPPMResponse
    {
        [JsonProperty("LaborScheduleId")]
        public long? LaborScheduleId { get; set; }

        [JsonProperty("LaborScheduleName")]
        public string? LaborScheduleName { get; set; }

        [JsonProperty("PersonId")]
        public long? PersonId { get; set; }

        [JsonProperty("PersonNumber")]
        public string? PersonNumber { get; set; }

        [JsonProperty("PersonName")]
        public string? PersonName { get; set; }

        [JsonProperty("PersonEmail")]
        public string? PersonEmail { get; set; }

        [JsonProperty("AssignmentId")]
        public long? AssignmentId { get; set; }


        [JsonProperty("AssignmentNumber")]
        public string? AssignmentNumber { get; set; }

        [JsonProperty("AssignmentName")]
        public string? AssignmentName { get; set; }

        [JsonProperty("AssignmentDepartment")]
        public string? AssignmentDepartment { get; set; }

        [JsonProperty("PayElementId")]
        public string? PayElementId { get; set; }

        [JsonProperty("PayElement")]
        public string? PayElement { get; set; }

        [JsonProperty("PayElementName")]
        public string? PayElementName { get; set; }

        [JsonProperty("LaborScheduleTypeCode")]
        public string? LaborScheduleTypeCode { get; set; }

        [JsonProperty("LaborScheduleType")]
        public string? LaborScheduleType { get; set; }

        [JsonProperty("BusinessUnitId")]
        public string? BusinessUnitId { get; set; }

        [JsonProperty("BusinessUnitName")]
        public string? BusinessUnitName { get; set; }

        [JsonProperty("CreationDate")]
        public DateTime? CreationDate { get; set; }

        [JsonProperty("LastUpdateDate")]
        public DateTime? LastUpdateDate { get; set; }

        [JsonProperty("CreatedBy")]
        public string? CreatedBy { get; set; }

        [JsonProperty("LastUpdatedBy")]
        public string? LastUpdatedBy { get; set; }

        [JsonProperty("LegislativeDataGroupId")]
        public string? LegislativeDataGroupId { get; set; }

        [JsonProperty("LegislativeDataGroupName")]
        public string? LegislativeDataGroupName { get; set; }

        [JsonProperty("RuleSource")]
        public string? RuleSource { get; set; }


        [JsonProperty("versions")]
        public Versions Versions { get; set; }


        [JsonProperty("links")]
        public WorkerLink[] Links { get; set; }

    }
}
