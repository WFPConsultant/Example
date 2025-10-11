namespace UVP.ExternalIntegration.ErrorValidationFramework.Model
{
    using System;

    public class APITransaction
    {
        public string Id { get; set; }

        public string APIcode  { get; set; }

        public string APIStatusCode { get; set; }

        public string APIErrorCode { get; set; }

        public string RequestObject { get; set; }

        public string ResponseObject { get; set; }

        public DateTime RequestSubmittedTimestamp { get; set; }

        public DateTime ResponseReceivedTimestamp { get; set; }

        public string InitialStatusId { get; set; }

        public bool IsReTriggered { get; set; }

        public string LatestRequestObject { get; set; }

        public string LatestResponseObject { get; set; }

        public DateTime LatestRequestSubmittedTimestamp { get; set; }

        public DateTime LatestResponseReceivedTimestamp { get; set; }

        public long DoaCandidateId { get; set; }

    }
}
