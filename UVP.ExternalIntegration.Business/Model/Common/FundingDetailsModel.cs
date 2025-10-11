namespace UVP.ExternalIntegration.Business.Model.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// FundingDetails of query class.
    /// </summary>
    public class FundingDetailsModel
    {
        /// <summary>
        /// Gets or sets CostType.
        /// </summary>
        [JsonProperty("CostType")]
        public string? CostType { get; set; }

        /// <summary>
        /// Gets or sets VersionID.
        /// </summary>
        [JsonProperty("VersionID")]
        public long? VersionID { get; set; }

        /// <summary>
        /// Gets or sets VersionStatus.
        /// </summary>
        [JsonProperty("VersionStatus")]
        public string? VersionStatus { get; set; }

        /// <summary>
        /// Gets or sets Award_Number.
        /// </summary>
        [JsonProperty("Award_Number")]
        public string? Award_Number { get; set; }

        /// <summary>
        /// Gets or sets Award_Name.
        /// </summary>
        [JsonProperty("Award_Name")]
        public string? Award_Name { get; set; }

        /// <summary>
        /// Gets or sets Sponsor_Name.
        /// </summary>
        [JsonProperty("Sponsor_Name")]
        public string? Sponsor_Name { get; set; }

        /// <summary>
        /// Gets or sets project_Number.
        /// </summary>
        [JsonProperty("Project_Number")]
        public string? Project_Number { get; set; }

        /// <summary>
        /// Gets or sets Project_Name.
        /// </summary>
        [JsonProperty("Project_Name")]
        public string? Project_Name { get; set; }

        /// <summary>
        /// Gets or sets Task_Name.
        /// </summary>
        [JsonProperty("Task_Name")]
        public string? Task_Name { get; set; }

        /// <summary>
        /// Gets or sets Task_Number.
        /// </summary>
        [JsonProperty("Task_Number")]
        public string? Task_Number { get; set; }

        /// <summary>
        /// Gets or sets Expenditure_Type_Name  .
        /// </summary>
        [JsonProperty("Expenditure_Type_Name")]
        public string? Expenditure_Type_Name { get; set; }

        /// <summary>
        /// Gets or sets Org_Name.
        /// </summary>
        [JsonProperty("Org_Name")]
        public string? Org_Name { get; set; }

        /// <summary>
        /// Gets or sets Percentage.
        /// </summary>
        [JsonProperty("Percentage")]
        public decimal? Percentage { get; set; }

        /// <summary>
        /// Gets or sets FundCode.
        /// </summary>
        [JsonProperty("FundCode")]
        public string? FundCode { get; set; }

        /// <summary>
        /// Gets or sets OperatingUnit.
        /// </summary>
        [JsonProperty("OperatingUnit")]
        public string? OperatingUnit { get; set; }

        /// <summary>
        /// Gets or sets Agency.
        /// </summary>
        [JsonProperty("Agency")]
        public string? Agency { get; set; }

        /// <summary>
        /// Gets or sets Donor.
        /// </summary>
        [JsonProperty("Donor")]
        public string? Donor { get; set; }

        /// <summary>
        /// Gets or sets CostCenter.
        /// </summary>
        [JsonProperty("CostCenter")]
        public string? CostCenter { get; set; }

        /// <summary>
        /// Gets or sets Project.
        /// </summary>
        [JsonProperty("Project")]
        public string? Project { get; set; }

        /// <summary>
        /// Gets or sets Interagency.
        /// </summary>
        [JsonProperty("Interagency")]
        public string? Interagency { get; set; }

        /// <summary>
        /// Gets or sets Future.
        /// </summary>
        [JsonProperty("Future")]
        public string? Future { get; set; }

    }
}
