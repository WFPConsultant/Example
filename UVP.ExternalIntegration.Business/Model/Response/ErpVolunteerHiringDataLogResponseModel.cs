namespace UVP.ExternalIntegration.Business.Model.Response
{
    public class ErpVolunteerHiringDataLogResponseModel
    {
        /// <summary>
        /// Gets or sets the DoaCandidateId.
        /// </summary>
        public long DoaCandidateId { get; set; }

        /// <summary>
        /// Gets or sets the ErpVolunteerHiringDataPostedtoQuantum.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Gets or sets the ErpVolunteerHiringCoaDataHCMPostedtoQuantum.
        /// </summary>
        public string CoaDataHCM { get; set; }

        /// <summary>
        /// Gets or sets the ErpVolunteerHiringCoaDataPPMPostedtoQuantum.
        /// </summary>
        public string CoaDataPPM { get; set; }
    }
}
