namespace UVP.ExternalIntegration.Business.Model.Common
{
    using Newtonsoft.Json;

    /// <summary>
    /// AssignmentModel of query class.
    /// </summary>
    public class AssignmentModel
    {

        /// <summary>
        /// Gets or sets Date of Hiring /Joining.
        /// </summary>
        public string HireDate { get; set; }

        /// <summary>
        /// Gets or sets the WorkerType.
        /// </summary>
        [JsonProperty("WorkerType")]
        public string? WorkerType { get; set; }

        /// <summary>
        /// Gets or sets the LegalEntityId.
        /// </summary>
        [JsonProperty("LegalEntityId")]
        public string? LegalEntityId { get; set; }

        /// <summary>
        /// Gets or sets the ActionCode.
        /// </summary>
        [JsonProperty("ActionCode")]
        public string? ActionCode { get; set; }

        /// <summary>
        /// Gets or sets the BusinessUnitId.
        /// </summary>
        [JsonProperty("BusinessUnitId")]
        public string? BusinessUnitId { get; set; }

        /// <summary>
        /// Gets or sets the AssignmentName.
        /// </summary>
        [JsonProperty("AssignmentName")]
        public string? AssignmentName { get; set; }

        /// <summary>
        /// Gets or sets the UserPersonType.
        /// </summary>
        [JsonProperty("UserPersonType")]
        public string? UserPersonType { get; set; }

        /// <summary>
        /// Gets or sets the AssignmentStatusTypeCode.
        /// </summary>
        [JsonProperty("AssignmentStatusTypeCode")]
        public string? AssignmentStatusTypeCode { get; set; }

        /// <summary>
        /// Gets or sets the GradeCode.
        /// </summary>
        [JsonProperty("GradeCode")]
        public string? GradeCode { get; set; }

        /// <summary>
        /// Gets or sets the GradeLadderId.
        /// </summary>
        [JsonProperty("GradeLadderId")]
        public string? GradeLadderId { get; set; }

        /// <summary>
        /// Gets or sets the GradeId.
        /// </summary>

        [JsonProperty("GradeId")]
        public string GradeId { get; set; }

        /// <summary>
        /// Gets or sets the GradeStepEligibilityFlag.
        /// </summary>
        [JsonProperty("GradeStepEligibilityFlag")]
        public string? GradeStepEligibilityFlag { get; set; }

        /// <summary>
        /// Gets or sets the DepartmentId.
        /// </summary>
        [JsonProperty("DepartmentId")]
        public string? DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the DutyStationCode.
        /// </summary>
        [JsonProperty("DutyStationCode")]
        public string? DutyStationCode { get; set; }

        /// <summary>
        /// Gets or sets the WorkerCategory.
        /// </summary>
        [JsonProperty("WorkerCategory")]
        public string? WorkerCategory { get; set; }

        /// <summary>
        /// Gets or sets the AssignmentCategory.
        /// </summary>
        [JsonProperty("AssignmentCategory")]
        public string? AssignmentCategory { get; set; }

        /// <summary>
        /// Gets or sets the PermanentTemporary.
        /// </summary>
        [JsonProperty("PermanentTemporary")]
        public string? PermanentTemporary { get; set; }

        /// <summary>
        /// Gets or sets the FullPartTime.
        /// </summary>
        [JsonProperty("FullPartTime")]
        public string? FullPartTime { get; set; }

        /// <summary>
        /// Gets or sets the ManagerFlag.
        /// </summary>
        [JsonProperty("ManagerFlag")]
        public string? ManagerFlag { get; set; }

        /// <summary>
        /// Gets or sets the HourlySalariedCode.
        /// </summary>
        [JsonProperty("HourlySalariedCode")]
        public string? HourlySalariedCode { get; set; }

        /// <summary>
        /// Gets or sets the SeniorityBasis.
        /// </summary>
        [JsonProperty("SeniorityBasis")]
        public string? SeniorityBasis { get; set; }

        /// <summary>
        /// Gets or sets the PeopleGroup.
        /// </summary>
        [JsonProperty("PeopleGroup")]
        public string? PeopleGroup { get; set; }

        // these properties are for UN Agency Info
        public string? AgencyAccountProjectCode { get; set; }
        public string? UnliquidatedObligation { get; set; }
        public string? AgencyReference { get; set; }
        public string? PositionType { get; set; }

        public string? AtlasCompany { get; set; }

        public string? ReimbursementEmployerBenefit { get; set; }

        public string? SpecialPostAllowanceAssignment { get; set; }

        public string? ContractType { get; set; }
        public string? FLEXContext { get; set; }
        public string? ContractClause { get; set; }

        public string? EntitledToInternationalEntitle { get; set; }
        public string? InitialDuration { get; set; }
        public string? InitialDurationUnits { get; set; }


        public string? AssignmentType { get; set; }
        public string? CategoryCode { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }

        public string? NormalHours { get; set; }
        public string? Frequency { get; set; }

        public string? GradeStepName { get; set; }


        public string? ApaLocationEntryDate { get; set; }
        public string? LocationEntryDate { get; set; }

        public string ChartType { get; set; }
        public bool IsHREntryDateAllowed { get; set; }

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

        public string QuantumLinkDisplay { get; set; }
        public string VolunteerCategoryDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the JobCode.
        /// </summary>
        public string JobCode { get; set; }


    }
}
