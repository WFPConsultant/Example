namespace UVP.ExternalIntegration.Domain.Entity.Doa
{
    using Microsoft.EntityFrameworkCore;

    [Keyless]
    public class CoaNonPPM
    {
        public string Percentage { get; set; }
        public int? SerialNumber { get; set; }
        public string? Agency { get; set; }
        public string? OperatingUnit { get; set; }
        public string? Fund { get; set; }
        public string? CostCentre { get; set; }
        public string? Project { get; set; }
        public string? Donor { get; set; }
        public string? Interagency { get; set; }
        public string? Futuresegment { get; set; }

        public string? StartDate { get; set; }
        public string? EndDate { get; set; }

        public long CoaId { get; set; }

        public long RuleLineId { get; set; }
    }


    public class CoaNonPPMDetails
    {

        public System.Collections.Generic.List<CoaNonPPM> CoaNonPPMList { get; set; }
    }
}
