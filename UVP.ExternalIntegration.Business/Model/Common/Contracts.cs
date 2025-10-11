namespace UVP.ExternalIntegration.Business.Model.Common
{
    using System;

    public class Contracts
    {
        public ContractsItems[] Items { get; set; }
    }

    public class ContractsItems
    {
        public long? ContractId { get; set; }
        public string? ContractNumber { get; set; }
        public string? EffectiveStartDate { get; set; }
        public string? EffectiveEndDate { get; set; }
        public string? ContractEndDate { get; set; }
        public string? ContractType { get; set; }
        public string? Description { get; set; }
        public int? InitialDuration { get; set; }
        public string? InitialDurationUnits { get; set; }
        public int? ExtensionNumber { get; set; }
        public string? ExtensionPeriod { get; set; }
        public string? ExtensionPeriodUnits { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public ContractsDFF ContractsDFF { get; set; }
        public WorkerLink[]? Links { get; set; }
    }
}
