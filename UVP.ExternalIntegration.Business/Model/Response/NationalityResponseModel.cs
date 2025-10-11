namespace UVP.ExternalIntegration.Business.Model.Response
{
    using System;
    using UVP.ExternalIntegration.Business.Model.Common;
    using Newtonsoft.Json;
    public class NationalityResponseModel
    {
        [JsonProperty("PersonExtraInfoId")]
        public long? PersonExtraInfoId { get; set; }

        [JsonProperty("EffectiveStartDate")]
        public string? EffectiveStartDate { get; set; }

        [JsonProperty("EffectiveEndDate")]
        public string? EffectiveEndDate { get; set; }

        [JsonProperty("PersonId")]
        public long? PersonId { get; set; }

        [JsonProperty("PeiInformationCategory")]
        public string? PeiInformationCategory { get; set; }

        [JsonProperty("CreationDate")]
        public DateTime? CreationDate { get; set; }

        [JsonProperty("LastUpdateDate")]
        public DateTime? LastUpdateDate { get; set; }

        [JsonProperty("PeiAttributeCategory")]
        public string? PeiAttributeCategory { get; set; }

        [JsonProperty("nationalityType")]
        public string? NationalityType { get; set; }

        [JsonProperty("nationalityType_Display")]
        public string? NationalityType_Display { get; set; }

        [JsonProperty("effectiveDate")]
        public string? EffectiveDate { get; set; }

        [JsonProperty("reason")]
        public string? Reason { get; set; }

        [JsonProperty("reason_Display")]
        public string? Reason_Display { get; set; }

        [JsonProperty("nationality")]
        public string? Nationality { get; set; }

        [JsonProperty("nationality_Display")]
        public string? Nationality_Display { get; set; }

        [JsonProperty("links")]
        public WorkerLink[] Links { get; set; }
    }
}
