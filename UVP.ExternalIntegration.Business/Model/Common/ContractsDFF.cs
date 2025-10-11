namespace UVP.ExternalIntegration.Business.Model.Common
{
    public class ContractsDFF
    {
        public ContractsDFFItems[] Items { get; set; }
    }

    public class ContractsDFFItems
    {
        public long? ContractId { get; set; }
        public string? EffectiveStartDate { get; set; }
        public string? EffectiveEndDate { get; set; }
        public string? __FLEX_Context { get; set; }
        public string? contractClause { get; set; }
        public string? contractClause_Display { get; set; }
        public string? status { get; set; }
        public string? effectiveEndDate1 { get; set; }
        public string? donorCountryEligibility { get; set; }
        public string? donorCountryEligibility_Display { get; set; }
        public string? entitledToInternationalEntitle { get; set; }
        public WorkerLink[]? Links { get; set; }
    }
}
