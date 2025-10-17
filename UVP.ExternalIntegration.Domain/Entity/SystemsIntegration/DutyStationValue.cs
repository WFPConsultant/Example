namespace UVP.ExternalIntegration.Domain.Entity.SystemsIntegration
{
    using Microsoft.EntityFrameworkCore;

    [Keyless]
    public class DutyStationValue
    {
        public long Id { get; set; }
        public string Code { get; set; } // duty station code
        public string ShortDescription { get; set; }
    }
}
