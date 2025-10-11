namespace UVP.ExternalIntegration.Business.Model.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// AssignmentDFF class.
    /// </summary>
    public class AssignmentDFF
    {
        /// <summary>
        /// Gets or sets the AtlasCompany.
        /// </summary>
        [JsonProperty("atlasCompany")]
        public string? AtlasCompany { get; set; }

        /// <summary>
        /// Gets or sets the LocationEntryDate.
        /// </summary>
        [JsonProperty("locationEntryDate")]
        public string? LocationEntryDate { get; set; }

        /// <summary>
        /// Gets or sets the ApaLocation.
        /// </summary>
        [JsonProperty("apaLocation")]
        public string? ApaLocation { get; set; }

        /// <summary>
        /// Gets or sets the APALocationEntryDate.
        /// </summary>
        [JsonProperty("apaLocationEntryDate")]
        public string? APALocationEntryDate { get; set; } 

        /// <summary>
        /// Gets or sets the ReimbursementEmployerBenefitPo.
        /// </summary>
        [JsonProperty("reimbursementEmployerBenefitPo")]
        public string? ReimbursementEmployerBenefitPo { get; set; }

        /// <summary>
        /// Gets or sets the SpecialPostAllowanceAssignment.
        /// </summary>
        [JsonProperty("specialPostAllowanceAssignment")]
        public string? SpecialPostAllowanceAssignment { get; set; }

        /// <summary>
        /// Gets or sets the AdjustedNextGradeStepDate.
        /// </summary>
        [JsonProperty("adjustedNextGradeStepDate")]
        public string? AdjustedNextGradeStepDate { get; set; }
    }
}
