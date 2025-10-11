namespace UVP.ExternalIntegration.Domain.Entity.Doa
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ErpErrorFrameworkAPITransaction
    {
        [Key]
        public Guid Pk_TransactionId { get; set; }
        public string APICode { get; set; }
        public string StatusCode { get; set; }
        public string ErrorCode { get; set; }
        public string RequestObject { get; set; }
        public string ResponseObject { get; set; }
        public DateTime RequestSubmittedTimestamp { get; set; }
        public DateTime? ResponseReceivedTimestamp { get; set; }
        public string InitialStatusCode { get; set; }
        public bool? IsReTriggered { get; set; }
        public string LatestRequestObject { get; set; }
        public string LastestResponseObject { get; set; }
        public DateTime? LatestRequestSubmittedTimestamp { get; set; }
        public DateTime? LatestResponseReceivedTimestamp { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedUser { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedUser { get; set; }
        public bool? IsActive { get; set; }
        public long? DoaCandidateId { get; set; }

    }
}
