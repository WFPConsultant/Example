namespace UVP.ExternalIntegration.Business.Model.Response
{
    using System;
    using UVP.ExternalIntegration.Business.Model.Common;
    using Newtonsoft.Json;

    public class ErpCoaNonPPMResponse
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


        [JsonProperty("percentage")]
        public string? Percentage { get; set; }

        [JsonProperty("serialNumber")]
        public int? SerialNumber { get; set; }

        [JsonProperty("agency")]
        public string? Agency { get; set; }

        [JsonProperty("agency_Desc")]
        public string? AgencyDesc { get; set; }

        [JsonProperty("operatingUnit")]
        public string? OperatingUnit { get; set; }

        [JsonProperty("operatingUnit_Display")]
        public string? OperatingUnitDisplay { get; set; }

        [JsonProperty("operatingUnit_Desc")]
        public string? OperatingUnitDesc { get; set; }

        [JsonProperty("fund")]
        public string? Fund { get; set; }

        [JsonProperty("fund_Display")]
        public string? FundDisplay { get; set; }

        [JsonProperty("fund_Desc")]
        public string? FundDesc { get; set; }

        [JsonProperty("costCentre")]
        public string? CostCentre { get; set; }

        [JsonProperty("costCentre_Display")]
        public string? CostCentreDisplay { get; set; }

        [JsonProperty("costCentre_Desc")]
        public string? CostCentreDesc { get; set; }

        [JsonProperty("project")]
        public string? Project { get; set; }

        [JsonProperty("project_Display")]
        public string? ProjectDisplay { get; set; }

        [JsonProperty("project_Desc")]
        public string? ProjectDesc { get; set; }


        [JsonProperty("donor")]
        public string? Donor { get; set; }

        [JsonProperty("donor_Desc")]
        public string? DonorDesc { get; set; }

        [JsonProperty("interagency")]
        public string? Interagency { get; set; }

        [JsonProperty("interagency_Display")]
        public string? InteragencyDisplay { get; set; }

        [JsonProperty("interagency_Desc")]
        public string? InteragencyDesc { get; set; }

        [JsonProperty("futuresegment")]
        public string? Futuresegment { get; set; }

        [JsonProperty("futuresegment_Display")]
        public string? FuturesegmentDisplay { get; set; }

        [JsonProperty("futuresegment_Desc")]
        public string? FuturesegmentDesc { get; set; }

        [JsonProperty("links")]
        public WorkerLink[] Links { get; set; }
    }
}
