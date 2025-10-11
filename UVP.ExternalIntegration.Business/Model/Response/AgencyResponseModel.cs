namespace UVP.ExternalIntegration.Business.Model.Response
{
    using Newtonsoft.Json;
    using UVP.ExternalIntegration.Business.Model.Common;
    public class AgencyResponseModel
    {

        [JsonProperty("PersonExtraInfoId")]
        public string? PersonExtraInfoId { get; set; }

        [JsonProperty("EffectiveStartDate")]
        public string? EffectiveStartDate { get; set; }

        [JsonProperty("EffectiveEndDate")]
        public string? EffectiveEndDate { get; set; }

        [JsonProperty("PersonId")]
        public string? PersonId { get; set; }

        [JsonProperty("PeiInformationCategory")]
        public string? PeiInformationCategory { get; set; }

        [JsonProperty("CreationDate")]
        public string? CreationDate { get; set; }

        [JsonProperty("LastUpdateDate")]
        public string? LastUpdateDate { get; set; }

        [JsonProperty("PeiAttributeCategory")]
        public string? PeiAttributeCategory { get; set; }

        [JsonProperty("agencyAccountNoProjectCode")]
        public string? AgencyAccountNoProjectCode { get; set; }

        [JsonProperty("agencyReference")]
        public string? AgencyReference { get; set; }

        [JsonProperty("unliquidatedObligation")]
        public string? UnliquidatedObligation { get; set; }

        [JsonProperty("positionType")]
        public string? PositionType { get; set; }

        [JsonProperty("assignmentNumber")]
        public string? AssignmentNumber { get; set; }

        [JsonProperty("assignmentNumber_Display")]
        public string? AssignmentNumber_Display { get; set; }

        [JsonProperty("links")]
        public WorkerLink[] Links { get; set; }
    }
}
