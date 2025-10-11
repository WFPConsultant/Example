namespace UVP.ExternalIntegration.Business.Model.Request
{
    /// <summary>
    /// EmployeeSearchRequestModel class.
    /// </summary>
    public class EmployeeSearchRequestModel
    {
        /// <summary>
        /// Gets or sets the FirstName.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the LastName.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the Gender.
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// Gets or sets the DateOfBirth.
        /// </summary>
        public string? DateOfBirth { get; set; }
    }
}
