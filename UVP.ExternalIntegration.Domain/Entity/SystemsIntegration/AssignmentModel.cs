namespace UVP.ExternalIntegration.Domain.Entity.SystemsIntegration
{
    using Microsoft.EntityFrameworkCore;

    [Keyless]
    public class AssignmentModel
    {
        public long Id { get; set; }
        public long DoaId { get; set; }
        public string DutyStationCode { get; set; }
    }
}
