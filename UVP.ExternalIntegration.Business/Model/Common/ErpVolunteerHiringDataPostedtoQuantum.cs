namespace UVP.ExternalIntegration.Business.Model.Common
{
    using System;
    using System.Collections.Generic;
    using UVP.ExternalIntegration.Domain.Entity.Users;

    public class ErpVolunteerHiringDataPostedtoQuantum
    {
        /// <summary>
        /// Gets or sets the FirstName.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the LastName.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the DateofBirth.
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the Gender.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the MaritalStatus.
        /// </summary>
        public string MaritalStatus { get; set; }

        /// <summary>
        /// Gets or sets the AddressType.
        /// </summary>
        public string AddressType { get; set; }

        /// <summary>
        /// Gets or sets the AddressLine.
        /// </summary>
        public string AddressLine { get; set; }

        /// <summary>
        /// Gets or sets the PostalCode.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the Country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the TownCity.
        /// </summary>
        public string TownCity { get; set; }

        /// <summary>
        /// Gets or sets the EmailAddress.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the OfficialNationality.
        /// </summary>
        public string OfficialNationality { get; set; }

        /// <summary>
        /// Gets or sets the CountryOfBirth.
        /// </summary>
        public string CountryOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the HireDate.
        /// </summary>
        public string HireDate { get; set; }

        /// <summary>
        /// Gets or sets the LegalEntity.
        /// </summary>
        public string LegalEntity { get; set; }

        /// <summary>
        /// Gets or sets the AssignmentName.
        /// </summary>
        public string AssignmentName { get; set; }

        /// <summary>
        /// Gets or sets the AssignmentStatusType.
        /// </summary>
        public string AssignmentStatusType { get; set; }

        /// <summary>
        /// Gets or sets the BusinessUnit.
        /// </summary>
        public string BusinessUnit { get; set; }

        /// <summary>
        /// Gets or sets the Grade.
        /// </summary>
        public string Grade { get; set; }

        /// <summary>
        /// Gets or sets the GradeLadder.
        /// </summary>
        public string GradeLadder { get; set; }

        /// <summary>
        /// Gets or sets the GradeStep.
        /// </summary>
        public string GradeStep { get; set; }

        /// <summary>
        /// Gets or sets the WorkerCategory.
        /// </summary>
        public string WorkerCategory { get; set; }

        /// <summary>
        /// Gets or sets the PeopleGroup.
        /// </summary>
        public string PeopleGroup { get; set; }

        /// <summary>
        /// Gets or sets the AgencyAccountProject.
        /// </summary>
        public string AgencyAccountProject { get; set; }

        /// <summary>
        /// Gets or sets the UnliquidatedObligation.
        /// </summary>
        public string UnliquidatedObligation { get; set; }

        /// <summary>
        /// Gets or sets the AgencyReference.
        /// </summary>
        public string AgencyReference { get; set; }

        /// <summary>
        /// Gets or sets the PositionType.
        /// </summary>
        public string PositionType { get; set; }

        /// <summary>
        /// Gets or sets the VolunteerCategory.
        /// </summary>
        public string VolunteerCategory { get; set; }

        /// <summary>
        /// Gets or sets the DutyStation.
        /// </summary>
        public string DutyStation { get; set; }

        /// <summary>
        /// Gets or sets the HostEntity.
        /// </summary>
        public string HostEntity { get; set; }

        /// <summary>
        /// Gets or sets the VLAeligibilityGroup.
        /// </summary>
        public string VlaEligibilityGroup { get; set; }

        /// <summary>
        /// Gets or sets the Department.
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Gets or sets the ContractStartDate.
        /// </summary>
        public string ContractStartDate { get; set; }

        /// <summary>
        /// Gets or sets the ContractDurationDays.
        /// </summary>
        public string ContractDurationDays { get; set; }

        /// <summary>
        /// Gets or sets the ContractType.
        /// </summary>
        public string ContractType { get; set; }

        /// <summary>
        /// Gets or sets the MLAcurrency.
        /// </summary>
        public string MlaCurrency { get; set; }

        /// <summary>
        /// Gets or sets the AnnualMLAbaseamount.
        /// </summary>
        public string AnnualMlaBaseAmount { get; set; }

        /// <summary>
        /// Gets or sets the MLAbase.
        /// </summary>
        public string MlaBase { get; set; }

        /// <summary>
        /// Gets or sets the IntegrationStep.
        /// </summary>
        public string IntegrationStep { get; set; }

        /// <summary>
        /// Gets or sets the DependentsDetails.
        /// </summary>
        public List<Dependent> DependentsDetails { get; set; }
    }
}
