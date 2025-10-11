namespace UVP.ExternalIntegration.Business.Model.Request
{
    using System.Collections.Generic;

    public class SaveStatusRequestModel
    {
        public List<string> apiCodes { get; set; }
        public long doaCandidateId { get; set; }
        public long userId { get; set; }

    }
}
