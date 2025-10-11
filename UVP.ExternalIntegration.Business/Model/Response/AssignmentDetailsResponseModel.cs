namespace UVP.ExternalIntegration.Business.Model.Response
{
    using System;
    using System.Linq;
    using UVP.ExternalIntegration.Business.Model.Common;
    using UVP.ExternalIntegration.Domain.Entity.Users;

    /// <summary>
    /// AssignmentInfoResponseModel class.
    /// </summary>
    public class AssignmentDetailsResponseModel
    {
        /// <summary>
        /// Gets or sets the CandidateModel.
        /// </summary>
        public CandidateModel Candidate { get; set; }

        /// <summary>
        /// Gets or sets the AssignmentModel.
        /// </summary>
        public AssignmentModel Assignment { get; set; }

        /// <summary>
        /// Gets or sets the DependentModel.
        /// </summary>
        public DependentDetails Dependents { get; set; }

        /// <summary>
        /// Gets or sets the SalaryBasisModel.
        /// </summary>
        public SalaryBasisModel SalaryBasis { get; set; }

        /// <summary>
        /// Gets or sets the FundingDetailsModel.
        /// </summary>s
        public FundingDetailsModel[] FundingDetails { get; set; }

        /// <summary>
        /// Gets or sets the APIStatusDisplayModel.
        /// </summary>s
        public APIStatusDisplayModel[] APIStatusDisplay { get; set; }

        public ErpVolunteerHiringDataPostedtoQuantum GetErpVolunteerHiringDataPostedtoQuantum()
        {
            var data = new ErpVolunteerHiringDataPostedtoQuantum() { IntegrationStep = "HIRE" };
            if (this.Candidate.IsNotNull())
            {
                data.AddressLine = this.Candidate.AddressLine1;
                data.AddressType = this.Candidate.AddressType;
                data.Country = this.Candidate.CountryName;
                data.CountryOfBirth = this.Candidate.CountryOfBirthName;
                data.DateOfBirth = this.Candidate.DateOfBirth;
                data.ContractStartDate = this.Candidate.HireDate;
                data.EmailAddress = this.Candidate.EmailAddress;
                data.FirstName = this.Candidate.FirstName;
                data.LastName = this.Candidate.LastName;
                data.Gender = this.Candidate.Gender;
                data.MaritalStatus = this.Candidate.MaritalStatusName;
                data.OfficialNationality = this.Candidate.NationalityName;
                data.PostalCode = this.Candidate.PostalCode;
                data.TownCity = this.Candidate.TownOrCity;
            }

            if (this.Dependents.IsNotNull())
            {
                data.DependentsDetails = this.Dependents.DependentList;
            }

            if (this.Assignment.IsNotNull())
            {
                data.AgencyAccountProject = this.Assignment.AgencyAccountProjectCode;
                data.AgencyReference = this.Assignment.AgencyReference;
                data.AssignmentName = this.Assignment.AssignmentName;
                data.AssignmentStatusType = this.Assignment.AssignmentStatusTypeCode;
                data.BusinessUnit = this.Assignment.BusinessUnitIdName;
                data.ContractDurationDays = this.Assignment.InitialDuration;
                data.ContractType = this.Assignment.ContractType;
                data.Department = this.Assignment.DepartmentName;
                data.DutyStation = this.Assignment.DutyStationName;
                data.Grade = this.Assignment.GradeCode;
                data.GradeLadder = this.Assignment.GradeLadderName;
                data.GradeStep = this.Assignment.GradeStepNameDisplay;
                data.HireDate = this.Assignment.HireDate;
                data.HostEntity = this.Assignment.HostentityName;
                data.LegalEntity = this.Assignment.LegalEntityName;
                data.PeopleGroup = this.Assignment.PeopleGroup;
                data.PositionType = this.Assignment.PositionTypeName;
                data.VolunteerCategory = this.Assignment.VolunteerCategoryDisplayName;
                data.WorkerCategory = this.Assignment.WorkerCategory;
                data.UnliquidatedObligation = this.Assignment.UnliquidatedObligation;
                data.VlaEligibilityGroup = this.Assignment.VLAEligibilityGroup;
            }

            if (this.SalaryBasis.IsNotNull())
            {
                data.MlaCurrency = this.SalaryBasis.CurrencyCode;
                data.MlaBase = this.SalaryBasis.SalaryBasisName;
                data.AnnualMlaBaseAmount = this.SalaryBasis.SalaryAmount;
            }

            return data;
        }

        public ErpVolunteerHiringCoaDataHCMPostedtoQuantum GetErpVolunteerHiringCoaDataHCMPostedtoQuantum()
        {
            if (this.FundingDetails.IsNotNull())
            {
                var funding = this.FundingDetails.FirstOrDefault(k => string.IsNullOrWhiteSpace(k.Task_Number) && string.IsNullOrWhiteSpace(k.Award_Number));
                if (funding.IsNotNull())
                {
                    return new ErpVolunteerHiringCoaDataHCMPostedtoQuantum()
                    {
                        Agency = funding.Agency,
                        CostCenter = funding.CostCenter,
                        Donor = funding.Donor,
                        FundCode = funding.FundCode,
                        Future = funding.Future,
                        Interagency = funding.Interagency,
                        OperatingUnit = funding.OperatingUnit,
                        Project = funding.Project,
                        TotalPercentage = funding.Percentage,
                        VersionId = funding.VersionID,
                        Versionstatus = funding.VersionStatus,
                        IntegrationStep = "HIRE",
                    };
                }
            }

            return null;
        }

        public ErpVolunteerHiringCoaDataPPMPostedtoQuantum GetErpVolunteerHiringCoaDataPPMPostedtoQuantum()
        {
            if (this.FundingDetails.IsNotNull())
            {
                var funding = this.FundingDetails.FirstOrDefault(k => !string.IsNullOrWhiteSpace(k.Task_Number) && !string.IsNullOrWhiteSpace(k.Award_Number)); // add PPM check
                if (funding.IsNotNull())
                {
                    return new ErpVolunteerHiringCoaDataPPMPostedtoQuantum()
                    {
                        AwardName = funding.Award_Name,
                        AwardNumber = funding.Award_Number,
                        ExpenditureType = funding.Expenditure_Type_Name,
                        OrganizationName = funding.Org_Name,
                        ProjectName = funding.Project_Name,
                        ProjectNumber = funding.Project_Number,
                        SponsorName = funding.Sponsor_Name,
                        TaskName = funding.Task_Name,
                        TaskNumber = funding.Task_Number,
                        TotalPercentage = funding.Percentage,
                        VersionId = funding.VersionID,
                        VersionStatus = funding.VersionStatus,
                        IntegrationStep = "HIRE",
                        CostType = funding.CostType,
                    };
                }
            }

            return null;
        }
    }
}
