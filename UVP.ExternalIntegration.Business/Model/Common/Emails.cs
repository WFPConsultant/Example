namespace UVP.ExternalIntegration.Business.Model.Common
{
    using System;

    public class Emails
    {
        public EmailsItems[] Items { get; set; }
    }

    public class EmailsItems
    {
        public long? EmailAddressId { get; set; }
        public string? EmailType { get; set; }
        public string? EmailAddress { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string? PrimaryFlag { get; set; }
        public WorkerLink[] Links { get; set; }
    }
}
