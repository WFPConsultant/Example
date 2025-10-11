using Newtonsoft.Json;

namespace UVP.ExternalIntegration.Business.Model.Common
{
    /// <summary>
    /// GradeStep class.
    /// </summary>
    public class GradeStep
    {
        /// <summary>
        /// Gets or sets the GradeId.
        /// </summary>
        [JsonProperty("GradeId")]
        public string? GradeId { get; set; }

        /// <summary>
        /// Gets or sets the GradeStepName.
        /// </summary>
        [JsonProperty("GradeStepName")]
        public string? GradeStepName { get; set; }


    }
}
