namespace UVP.ExternalIntegration.Domain.Entity.Doa
{
    using Microsoft.EntityFrameworkCore;

    [Keyless]
    public class SalaryBasis
    {

        public string? ActionId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? SalaryAmount { get; set; }
        public long? SalaryBasisId { get; set; }
        public string? SalaryBasisName { get; set; }

    }
}
