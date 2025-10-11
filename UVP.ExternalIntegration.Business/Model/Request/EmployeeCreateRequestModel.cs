namespace UVP.ExternalIntegration.Business.Model.Request
{
    using Newtonsoft.Json;
    using UVP.ExternalIntegration.Business.Model.Common;

    /// <summary>
    /// EmployeeCreateRequestModel class.
    /// </summary>
    public class EmployeeCreateRequestModel
    {
        /// <summary>
        /// Gets or sets the DateOfBirth.
        /// </summary>
        [JsonProperty("DateOfBirth")]
        public string? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the Names.
        /// </summary>
        [JsonProperty("names")]
        public EmpInfo[]? Names { get; set; }

        /// <summary>
        /// Gets or sets the Addresses.
        /// </summary>
        /// Property shoulds tay commented or deleted. please pay attention
        // [JsonProperty("addresses")]
        // public Address[]? Addresses { get; set; }

        /// <summary>
        /// Gets or sets the Emails.
        /// </summary>
        [JsonProperty("emails")]
        public Email[]? Emails { get; set; }

        /// <summary>
        /// Gets or sets the LegislativeInfo.
        /// </summary>
        [JsonProperty("legislativeInfo")]
        public LegislativeInfo[] ? LegislativeInfo { get; set; }

        /// <summary>
        /// Gets or sets the WorkRelationships.
        /// </summary>
        [JsonProperty("workRelationships")]
        public WorkRelationship[]? WorkRelationships { get; set; }
    }
}
