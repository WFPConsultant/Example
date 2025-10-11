namespace UVP.ExternalIntegration.Business.Model.Request
{
    public class UpdateErpNumberRequestModel
    {
        /// <summary>
        /// Gets or sets the AssignmentNumber.
        /// </summary>
        public string ErpAssignmentNumber { get; set; }

        /// <summary>
        /// Gets or sets the ErpNumberChangeReason.
        /// </summary>
        public string ChangeReasonCode { get; set; }

        /// <summary>
        /// Gets or sets the DoaCandidateId.
        /// </summary>
        public long DoaCandidateId { get; set; }

        /// <summary>
        /// Gets or sets the UserId.
        /// </summary>
        public long UserId { get; set; }
    }
}
