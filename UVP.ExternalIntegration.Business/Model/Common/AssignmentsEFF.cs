using Newtonsoft.Json;

namespace UVP.ExternalIntegration.Business.Model.Common
{
    /// <summary>
    /// AssignmentEFF class.
    /// </summary>
    public class AssignmentEFF
    {
        /// <summary>
        /// Gets or sets the AssignmentType.
        /// </summary>
        [JsonProperty("AssignmentType")]
        public string? AssignmentType { get; set; }

        /// <summary>
        /// Gets or sets the CategoryCode.
        /// </summary>
        [JsonProperty("CategoryCode")]
        public string? CategoryCode { get; set; }
    }
}
