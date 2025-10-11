using Microsoft.EntityFrameworkCore;

namespace UVP.ExternalIntegration.Domain.Entity.Users
{
    using System;

    [Keyless]
    public class Candidate
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? LegislationCode { get; set; }
        public string? MaritalStatus { get; set; }
        public string? AddressType { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? TownOrCity { get; set; }
        public string? EmailAddress { get; set; }
        public string? EmailType { get; set; }
        public string? CountryOfBirth { get; set; }

        public long CandidateId { get; set; }

        public string CountryOfBirthName { get; set; }

        public string CountryName { get; set; }

        public string MaritalStatusName { get; set; }
        public string NationalityName { get; set; }
        public string CurrentNationality { get; set; }

        public string ErpPersonId { get; set; }

        public string ErpPersonNumber { get; set; }
        public string ErpAssignmentId { get; set; }
        public string ErpAssignmentNumber { get; set; }

        public string Region3 { get; set; }
    }
}
