namespace UVP.ExternalIntegration.Domain
{
    public class ExternalIntegrationOptions
    {
        /// <summary>
        /// Constant value Section.
        /// </summary>
        public const string Section = "ExternalIntegration";

        /// <summary>
        /// Constant value LogicalName.
        /// </summary>
        public const string LogicalName = "ExternalIntegration";

        /// <summary>
        /// Gets or sets URL of ExternalIntegration.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the RestApiFolder
        /// </summary>
        public string RestApiFolder { get; set; }

        /// <summary>
        /// Gets or sets the InternationalPayrollId
        /// </summary>
        public string InternationalPayrollId { get; set; }

        /// <summary>
        /// Gets or sets the NationalPayrollId
        /// </summary>
        public string NationalPayrollId { get; set; }
    }
}
