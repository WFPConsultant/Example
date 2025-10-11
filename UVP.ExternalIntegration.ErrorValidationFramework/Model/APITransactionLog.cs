namespace UVP.ExternalIntegration.ErrorValidationFramework.Model
{
    using System;
    using UVP.ExternalIntegration.ErrorValidationFramework.Enum;

    public class APITransactionLog
    {

        public long Id { get; set; }

        public long APITransactionId { get; set; }

        public APICodes APIcode { get; set; }

        public APIStatusCodes APIStatusCode { get; set; }

        public APIErrorCodes APIErrorCode { get; set; }

        public string RequestObject { get; set; }

        public string ResponseObject { get; set; }

        public DateTime RequestSubmittedTimestamp { get; set; }

        public DateTime ResponseReceivedTimestamp { get; set; }

        public APIStatusCodes InitialStatusId { get; set; }

        public bool IsReTriggered { get; set; }

     

    }
}
