namespace UVP.ExternalIntegration.Domain.Entity.Doa
{
    using System;    
    using System.ComponentModel.DataAnnotations;

    public class ErpErrorFrameworkAPITransactionManual
    {
        [Key]
        public long Id { get; set; }
        public long DoaCandidateId { get; set; }
        public string ApiCode { get; set; }
        public bool IsUpdated { get; set; }
        public string StatusCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedUser { get; set; }
        public bool IsActive { get; set; }
    }
}
