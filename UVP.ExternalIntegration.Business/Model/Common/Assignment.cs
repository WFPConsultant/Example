using Newtonsoft.Json;

namespace UVP.ExternalIntegration.Business.Model.Common
{
    /// <summary>
    /// Assignment class.
    /// </summary>
    public class Assignment
    {
        /// <summary>
        /// Gets or sets the ActionCode.
        /// </summary>
        [JsonProperty("ActionCode")]
        public string? ActionCode { get; set; }

        /// <summary>
        /// Gets or sets the AssignmentName.
        /// </summary>
        [JsonProperty("AssignmentName")]
        public string? AssignmentName { get; set; }

        /// <summary>
        /// Gets or sets the BusinessUnitId.
        /// </summary>
        [JsonProperty("BusinessUnitId")]
        public string? BusinessUnitId { get; set; }

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
        public string? GradeCode { get; set; }

        /// <summary>
        /// Gets or sets the GradeLadderId.
        /// </summary>
        [JsonProperty("GradeLadderId")]
        public string? GradeLadderId { get; set; }

              
         /// <summary>
        /// Gets or sets the GradeStepEligibilityFlag.
        /// </summary>
        [JsonProperty("GradeStepEligibilityFlag")]
        public bool? GradeStepEligibilityFlag { get; set; }

        /// <summary>
        /// Gets or sets the DepartmentId.
        /// </summary>
        [JsonProperty("DepartmentId")]
        public string? DepartmentId { get; set; }

        /// <summary>
        /// Gets or sets the ReportingEstablishmentName.
        /// </summary>
        [JsonProperty("ReportingEstablishmentName")]
        public string? ReportingEstablishmentName { get; set; }

        /// <summary>
        /// Gets or sets the LocationCode.
        /// </summary>
        public string? LocationCode { get; set; }

        /// <summary>
        /// Gets or sets the WorkAtHomeFlag.
        /// </summary>
        [JsonProperty("WorkAtHomeFlag")]
        public bool? WorkAtHomeFlag { get; set; }

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
        public bool? ManagerFlag { get; set; }

        /// <summary>
        /// Gets or sets the HourlySalariedCode.
        /// </summary>
        [JsonProperty("HourlySalariedCode")]
        public string? HourlySalariedCode { get; set; }

        /// <summary>
        /// Gets or sets the NormalHours.
        /// </summary>
        [JsonProperty("NormalHours")]
        public string? NormalHours { get; set; }

        /// <summary>
        /// Gets or sets the Frequency.
        /// </summary>
        [JsonProperty("Frequency")]
        public string? Frequency { get; set; }

        /// <summary>
        /// Gets or sets the StartTime.
        /// </summary>
        [JsonProperty("StartTime")]
        public string? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the EndTime.
        /// </summary>
        [JsonProperty("EndTime")]
        public string? EndTime { get; set; }

        /// <summary>
        /// Gets or sets the SeniorityBasis.
        /// </summary>
        [JsonProperty("SeniorityBasis")]
        public string? SeniorityBasis { get; set; }

        /// <summary>
        /// Gets or sets the PeopleGroup.
        /// </summary>
        public string? PeopleGroup { get; set; }

        /// <summary>
        /// Gets or sets the AssignmentsEFF.
        /// </summary>
        [JsonProperty("assignmentsEFF")]
        public AssignmentEFF[] AssignmentsEFF { get; set; }

        /// <summary>
        /// Gets or sets the AssignmentsDFF.
        /// </summary>
        [JsonProperty("assignmentsDFF")]
        public AssignmentDFF[] AssignmentsDFF { get; set; }

        /// <summary>
        /// Gets or sets the GradeSteps.
        /// </summary>
        [JsonProperty("gradeSteps")]
        public GradeStep[] GradeSteps { get; set; }
        /// <summary>
        /// Gets or sets the JobCode.
        /// </summary>
        ///
        [JsonProperty("JobCode")]
        public string JobCode { get; set; }

    }
}
