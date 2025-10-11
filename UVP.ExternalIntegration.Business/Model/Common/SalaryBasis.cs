namespace UVP.ExternalIntegration.Business.Model.Common
{
    using Newtonsoft.Json;

    public class SalaryBasisModel
    {
        [JsonProperty("ActionId")]
        public string ActionId { get; set; }

        [JsonProperty("CurrencyCode")]
        public string? CurrencyCode { get; set; }

        [JsonProperty("SalaryAmount")]
        public string? SalaryAmount { get; set; }

        [JsonProperty("SalaryBasisId")]
        public long? SalaryBasisId { get; set; }

        [JsonProperty("SalaryBasisName")]
        public string? SalaryBasisName { get; set; }
    }
}
