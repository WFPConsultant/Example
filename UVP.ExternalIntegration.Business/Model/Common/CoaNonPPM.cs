namespace UVP.ExternalIntegration.Business.Model.Common
{

    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class CoaNonPPMData
    {
        [JsonProperty("percentage")]
        public string Percentage { get; set; }

        [JsonProperty("serialNumber")]
        public int? SerialNumber { get; set; }

        [JsonProperty("agency")]
        public string? Agency { get; set; }

        [JsonProperty("operatingUnit")]
        public string? OperatingUnit { get; set; }

        [JsonProperty("fund")]
        public string? Fund { get; set; }

        [JsonProperty("costCentre")]
        public string? CostCentre { get; set; }

        [JsonProperty("project")]
        public string? Project { get; set; }

        [JsonProperty("donor")]
        public string? Donor { get; set; }

        [JsonProperty("interagency")]
        public string? Interagency { get; set; }

        [JsonProperty("futuresegment")]
        public string? Futuresegment { get; set; }

        [JsonProperty("startdate")]
        public string? Startdate { get; set; }

        [JsonProperty("enddate")]
        public string? EndDate { get; set; }



    }

    public class CoaNonPPMDataDetails
    {
        public List<CoaNonPPMData> CoaNonPPMDataList { get; set; }
    }
}
