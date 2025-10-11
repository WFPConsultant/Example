namespace UVP.ExternalIntegration.Business.Model.Response
{
    using System;
    using UVP.ExternalIntegration.Business.Model.Common;

    public class AssignmentResponseModel
    {
        public long? AssignmentId { get; set; }
        public string? AssignmentNumber { get; set; }
        public string? AssignmentName { get; set; }
        public string? ActionCode { get; set; }
        public string? ReasonCode { get; set; }
        public string? EffectiveStartDate { get; set; }
        public string? EffectiveEndDate { get; set; }
        public string? EffectiveSequence { get; set; }
        public string? EffectiveLatestChange { get; set; }
        public long? BusinessUnitId { get; set; }
        public string? BusinessUnitName { get; set; }
        public string? AssignmentType { get; set; }
        public int? AssignmentStatusTypeId { get; set; }
        public string? AssignmentStatusTypeCode { get; set; }
        public string? AssignmentStatusType { get; set; }
        public string? SystemPersonType { get; set; }
        public long? UserPersonTypeId { get; set; }
        public string? UserPersonType { get; set; }
        public string? ProposedUserPersonTypeId { get; set; }
        public string? ProposedUserPersonType { get; set; }
        public string? ProjectedStartDate { get; set; }
        public string? ProjectedEndDate { get; set; }
        public bool? PrimaryFlag { get; set; }
        public bool? PrimaryAssignmentFlag { get; set; }
        public long? PositionId { get; set; }
        public string? PositionCode { get; set; }
        public bool? SynchronizeFromPositionFlag { get; set; }
        public long? JobId { get; set; }
        public string? JobCode { get; set; }
        public long? GradeId { get; set; }
        public string? GradeCode { get; set; }
        public long? GradeLadderId { get; set; }
        public string? GradeLadderName { get; set; }
        public bool? GradeStepEligibilityFlag { get; set; }
        public long? GradeCeilingStepId { get; set; }
        public string? GradeCeilingStep { get; set; }
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public long? ReportingEstablishmentId { get; set; }
        public string? ReportingEstablishmentName { get; set; }
        public long? LocationId { get; set; }
        public string? LocationCode { get; set; }
        public bool? WorkAtHomeFlag { get; set; }
        public string? AssignmentCategory { get; set; }
        public string? WorkerCategory { get; set; }
        public string? PermanentTemporary { get; set; }
        public string? FullPartTime { get; set; }
        public bool? ManagerFlag { get; set; }
        public string? HourlySalariedCode { get; set; }
        public string? NormalHours { get; set; }
        public string? Frequency { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? SeniorityBasis { get; set; }
        public string? ProbationPeriod { get; set; }
        public string? ProbationPeriodUnit { get; set; }
        public string? ProbationEndDate { get; set; }
        public string? NoticePeriod { get; set; }
        public string? NoticePeriodUOM { get; set; }
        public long? WorkTaxAddressId { get; set; }
        public string? ExpenseCheckSendToAddress { get; set; }
        public string? RetirementAge { get; set; }
        public string? RetirementDate { get; set; }
        public bool? LabourUnionMemberFlag { get; set; }
        public long? UnionId { get; set; }
        public string? UnionName { get; set; }
        public string? BargainingUnitCode { get; set; }
        public long? CollectiveAgreementId { get; set; }
        public string? CollectiveAgreementName { get; set; }
        public long? ContractId { get; set; }
        public string? ContractNumber { get; set; }
        public string? InternalBuilding { get; set; }
        public string? InternalFloor { get; set; }
        public string? InternalOfficeNumber { get; set; }
        public string? InternalMailstop { get; set; }
        public string? DefaultExpenseAccount { get; set; }
        public string? PeopleGroup { get; set; }
        public string? StandardWorkingHours { get; set; }
        public string? StandardFrequency { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public WorkerLink[]? Links { get; set; }
    }
}
