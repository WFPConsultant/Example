namespace UVP.ExternalIntegration.Domain.Entity.Doa
{
    using Microsoft.EntityFrameworkCore;

    [Keyless]
    public class ManualHire
    {
        public long? DoaCandidateId { get; set; }
      
        public string? ErpAssignmentNumber { get; set; }

        public long? ErpAssignmentId { get; set; }

        public string? ErpAssignmentHash { get; set; }

        public string? ErpPersonNumber { get; set; }

        public long? ErpPersonId { get; set; }

        public string? ErpPersonHash { get; set; }

        public string? ErpContractNumber { get; set; }

        public long? ErpContractId { get; set; }

        public string? ErpContractHash { get; set; }

        public long? UserId { get; set; }

        public string ReasonCode { get; set; }

        public string ReasonTableCode { get; set; }
    }
}
