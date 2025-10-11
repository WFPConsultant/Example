namespace UVP.ExternalIntegration.Business.Model.Common
{
    
    public class ConfigurationModel
    {

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
    }
}
