namespace UVP.ExternalIntegration.Domain.Entity.Doa
{
    using Microsoft.EntityFrameworkCore;

    [Keyless]
    public class CoaPPM
    {
        public string? AssignmentNumber { get; set; }
        public string? PersonNumber { get; set; }
        public string? LaborScheduleName { get; set; }
        public string? VersionName { get; set; }
        public string? VersionComments { get; set; }
        public string? VersionStatusCode { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }

        public int? LineNumber { get; set; }
        public string? LinePercent { get; set; }

        public string? ContractNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? TaskNumber { get; set; }
        public string? FundingSourceNumber { get; set; }
        public long? ExpenditureTypeId { get; set; }
        public long? ExpenditureOrganizationId { get; set; }

        public string? ContextCategory { get; set; }
        public string? VersionStatus { get; set; }

        public long CoaId { get; set; }

        public long RuleLineId { get; set; }

    }


    public class CoaPPMDetails
    {

        public System.Collections.Generic.List<CoaPPM> CoaPPMList { get; set; }
    }
}
