namespace UVP.ExternalIntegration.Business.Model.Request
{
    /// <summary>
    /// ManualHireRequestModel class.
    /// </summary>
    public class ManualHireRequestModel
    {
        /// <summary>
        /// Gets or sets the AssignmentNumber.
        /// </summary>
        public string AssignmentNumber { get; set; }

        /// <summary>
        /// Gets or sets the ErpNumberChangeReason.
        /// </summary>
        public string ReasonCode { get; set; }

        /// <summary>
        /// Gets or sets the ErpNumberChangeReason.
        /// </summary>
        public string ReasonTableCode { get; set; }

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
