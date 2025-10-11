namespace UVP.ExternalIntegration.Domain.Entity.Doa
{
    using Microsoft.EntityFrameworkCore;

    [Keyless]
    public class UserAssignment
    {
        public string HireDate { get; set; }
        public string WorkerType { get; set; }
        public string LegalEntityId { get; set; }
        public string BusinessUnitId { get; set; }
        public string GradeLadderId { get; set; }
        public string GradeId { get; set; }

        public string DepartmentId { get; set; }

        public string ActionCode { get; set; }
     
        public string AssignmentName { get; set; }
        public string UserPersonType { get; set; }
        public string AssignmentStatusTypeCode { get; set; }
        public string GradeCode { get; set; }
       
        public string GradeStepEligibilityFlag { get; set; }
       
        public string DutyStationCode { get; set; }
        public string WorkerCategory { get; set; }
        public string AssignmentCategory { get; set; }
        public string PermanentTemporary { get; set; }
        public string FullPartTime { get; set; }
        public string ManagerFlag { get; set; }
        public string HourlySalariedCode { get; set; }
        public string SeniorityBasis { get; set; }
        public string PeopleGroup { get; set; }

        // these properties are for UN Agency Info
        public string AgencyAccountProjectCode { get; set; }
        public string UnliquidatedObligation { get; set; }
        public string AgencyReference { get; set; }
        public string PositionType { get; set; }
        public string AtlasCompany { get; set; }

        public string ReimbursementEmployerBenefit { get; set; }

        public string SpecialPostAllowanceAssignment { get; set; }

        public string ContractType { get; set; }
        public string FLEXContext { get; set; }
        public string ContractClause { get; set; }

        public string EntitledToInternationalEntitle { get; set; }
        public string InitialDuration { get; set; }
        public string InitialDurationUnits { get; set; }
        public string AssignmentType { get; set; }
        public string CategoryCode { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public string NormalHours { get; set; }
        public string Frequency { get; set; }

        public string GradeStepName { get; set; }
        public string ApaLocationEntryDate { get; set; }
        public string LocationEntryDate { get; set; }

        public string ChartType { get; set; }

        public string HostentityName { get; set; }
        public string VLAEligibilityGroup { get; set; }
        public string Volunteertype { get; set; }
        public string DutyStationName { get; set; }
        public string BusinessUnitIdName { get; set; }
        public string PositionTypeName { get; set; }
        public string GradeLadderName { get; set; }
        public string DepartmentName { get; set; }
        public string WorkerCategoryName { get; set; }
        public string LegalEntityName { get; set; }
        public string GradeStepNameDisplay { get; set; }
        public string VolunteerCategoryDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the JobCode.
        /// </summary>
        public string JobCode { get; set; }

    }
}
