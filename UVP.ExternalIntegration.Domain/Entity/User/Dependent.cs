namespace UVP.ExternalIntegration.Domain.Entity.Users
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    [Keyless]
    public class Dependent
    {
        public string DateOfBirth { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? LegislationCode { get; set; }
        public string? MaritalStatus { get; set; }
        public string? Gender { get; set; }
        public string? ContactType { get; set; }
        public string? AllowanceEligibleCheckbox { get; set; }
        public string? HouseholdMember { get; set; }

        [NotMapped]
        public string? IntegrationStatusCode { get; set; }

    }

    public class DependentDetails
    {

        public List<Dependent> DependentList { get; set; }
    }
}
