namespace UVP.ExternalIntegration.Business.Model.Common
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// CandidateModel class.
    /// </summary>
    public class CandidateModel
    {
        /// <summary>
        /// Gets or sets the FirstName.
        /// </summary>
        [JsonProperty("FirstName")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the LastName.
        /// </summary>
        [JsonProperty("LastName")]
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the DateOfBirth.
        /// </summary>
        [JsonProperty("DateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the Gender.
        /// </summary>
        [JsonProperty("Gender")]
        public string? Gender { get; set; }

        /// <summary>
        /// Gets or sets the LegislationCode.
        /// </summary>
        [JsonProperty("LegislationCode")]
        public string? LegislationCode { get; set; }

        /// <summary>
        /// Gets or sets the MaritalStatus.
        /// </summary>
        [JsonProperty("MaritalStatus")]
        public string? MaritalStatus { get; set; }

        /// <summary>
        /// Gets or sets the AddressType.
        /// </summary>
        [JsonProperty("AddressType")]
        public string? AddressType { get; set; }

        /// <summary>
        /// Gets or sets the AddressLine1.
        /// </summary>
        [JsonProperty("AddressLine1")]
        public string? AddressLine1 { get; set; }

        /// <summary>
        /// Gets or sets the AddressLine2.
        /// </summary>
        [JsonProperty("AddressLine2")]
        public string? AddressLine2 { get; set; }

        /// <summary>
        /// Gets or sets the Country.
        /// </summary>
        [JsonProperty("Country")]
        public string? Country { get; set; }

        /// <summary>
        /// Gets or sets the PostalCode.
        /// </summary>
        [JsonProperty("PostalCode")]
        public string? PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the TownOrCity.
        /// </summary>
        [JsonProperty("TownOrCity")]
        public string? TownOrCity { get; set; }

        /// <summary>
        /// Gets or sets the EmailAddress.
        /// </summary>
        [JsonProperty("EmailAddress")]
        public string? EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the EmailType.
        /// </summary>
        [JsonProperty("EmailType")]
        public string? EmailType { get; set; }

        /// <summary>
        /// Gets or sets the Nationality.
        /// </summary>
        [JsonProperty("Nationality")]
        public string? Nationality { get; set; }

        /// <summary>
        /// Gets or sets the Hire Date.
        /// </summary>
        public string HireDate { get; set; }

        [JsonProperty("Region2")]
        public string Region3 { get; set; }

        public string CountryOfBirth { get; set; }

        public long CandidateId { get; set; }

        public string CountryOfBirthName { get; set; }

        public string CountryName { get; set; }

        public string MaritalStatusName { get; set; }

        public string NationalityName { get; set; }

        public string CurrentNationality { get; set; }

        public string ErpPersonId { get; set; }

        public string  ErpPersonNumber { get; set; }
        public string ErpAssignmentId { get; set; }
        public string ErpAssignmentNumber { get; set; }

        
    }
}
