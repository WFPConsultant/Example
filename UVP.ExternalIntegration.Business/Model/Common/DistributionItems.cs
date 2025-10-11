namespace UVP.ExternalIntegration.Business.Model.Common
{
    using System;
    using Newtonsoft.Json;

    public class DistributionItems
    {
        [JsonProperty("DistributionRuleId")]
        public long? DistributionRuleId { get; set; }

        [JsonProperty("LineNumber")]
        public int? LineNumber { get; set; }

        [JsonProperty("LinePercent")]
        public string? LinePercent { get; set; }

        [JsonProperty("ContractId")]
        public long? ContractId { get; set; }

        [JsonProperty("ContractName")]
        public string? ContractName { get; set; }

        [JsonProperty("ContractNumber")]
        public string? ContractNumber { get; set; }

        [JsonProperty("ProjectId")]
        public long? ProjectId { get; set; }

        [JsonProperty("ProjectName")]
        public string? ProjectName { get; set; }

        [JsonProperty("ProjectNumber")]
        public string? ProjectNumber { get; set; }

        [JsonProperty("TaskId")]
        public long? TaskId { get; set; }

        [JsonProperty("TaskName")]
        public string? TaskName { get; set; }

        [JsonProperty("TaskNumber")]
        public string? TaskNumber { get; set; }

        [JsonProperty("FundingSourceId")]
        public string? FundingSourceId { get; set; }

        [JsonProperty("FundingSourceName")]
        public string? FundingSourceName { get; set; }

        [JsonProperty("FundingSourceNumber")]
        public string? FundingSourceNumber { get; set; }

        [JsonProperty("ExpenditureTypeId")]
        public long? ExpenditureTypeId { get; set; }

        [JsonProperty("ExpenditureTypeName")]
        public string? ExpenditureTypeName { get; set; }

        [JsonProperty("ExpenditureItemDate")]
        public string? ExpenditureItemDate { get; set; }

        [JsonProperty("ExpenditureOrganizationId")]
        public long? ExpenditureOrganizationId { get; set; }

        [JsonProperty("ExpenditureOrganizationName")]
        public string? ExpenditureOrganizationName { get; set; }

        [JsonProperty("WorkTypeId")]
        public string? WorkTypeId { get; set; }

        [JsonProperty("WorkTypeName")]
        public string? WorkTypeName { get; set; }

        [JsonProperty("GLAccountCombinationId")]
        public string? GLAccountCombinationId { get; set; }

        [JsonProperty("BillableFlag")]
        public string? BillableFlag { get; set; }

        [JsonProperty("CapitalizableFlag")]
        public long? CapitalizableFlag { get; set; }

        [JsonProperty("ContextCategory")]
        public string? ContextCategory { get; set; }

        [JsonProperty("ContractLineId")]
        public string? ContractLineId { get; set; }

        [JsonProperty("FundingAllocationId")]
        public string? FundingAllocationId { get; set; }

        [JsonProperty("CreatedBy")]
        public string? CreatedBy { get; set; }

        [JsonProperty("CreationDate")]
        public DateTime? CreationDate { get; set; }

        [JsonProperty("LastUpdateDate")]
        public DateTime? LastUpdateDate { get; set; }

        [JsonProperty("LastUpdatedBy")]
        public string? LastUpdatedBy { get; set; }


        [JsonProperty("GLAccount")]
        public string? GLAccount { get; set; }

        [JsonProperty("ChartOfAccountsId")]
        public int? ChartOfAccountsId { get; set; }

    }
}
