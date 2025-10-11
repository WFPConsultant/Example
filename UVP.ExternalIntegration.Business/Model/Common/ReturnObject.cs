namespace UVP.ExternalIntegration.Business.Model.Common
{
    using System.Net;
    using Newtonsoft.Json;
    using UVP.Shared.Micro.Entities.Models;

    /// <summary>
    /// ReturnObject class.
    /// </summary>
    public class ReturnObject : IEntityActivityAuditable
    {
        /// <summary>
        /// Gets or sets ResponseCode.
        /// </summary>
        [JsonProperty("responsecode")]
        public HttpStatusCode ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets Message.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets Response.
        /// </summary>
        //[JsonIgnore]
        public dynamic Response { get; set; }

        /// <summary>
        /// Gets or sets PersonId
        /// </summary>
        [JsonProperty("PersonId")]
        public long? PersonId { get; set; }

        /// <summary>
        /// Gets or sets PersonNumber
        /// </summary>

        [JsonProperty("PersonNumber")]
        public string PersonNumber { get; set; }

        /// <summary>
        /// Gets or sets PersonHash. 
        /// </summary>
        [JsonProperty("PersonHash")]
        public string PersonHash { get; set; }

        /// <summary>
        /// Gets or sets AssignmentId. 
        /// </summary>
        [JsonProperty("AssignmentId")]
        public long? AssignmentId { get; set; }

        /// <summary>
        /// Gets or sets AssignmentNumber. 
        /// </summary>
        [JsonProperty("AssignmentNumber")]
        public string AssignmentNumber { get; set; }

        /// <summary>
        /// Gets or sets AssignmentHash. 
        /// </summary>
        [JsonProperty("AssignmentHash")]
        public string AssignmentHash { get; set; }

        /// <summary>
        /// Gets or sets ContractId. 
        /// </summary>

        [JsonProperty("ContractId")]
        public long? ContractId { get; set; }

        /// <summary>
        /// Gets or sets ContractNumber. 
        /// </summary>

        [JsonProperty("ContractNumber")]
        public string ContractNumber { get; set; }

        /// <summary>
        /// Gets or sets ContractHash. 
        /// </summary>

        [JsonProperty("ContractHash")]
        public string ContractHash { get; set; }

        [JsonIgnore]
        public string EffectiveDate { get; set; }

        [JsonIgnore]
        public string Nationality { get; set; }

        public ActivityLogRequest GetActivityLogEntryRequest() => new ActivityLogRequest()
        {
            EntityId = this.AssignmentId ?? 0,
            EntityType = "Assignment",
            TaskName = "ManualHire",
            RootBusinessKey = this.AssignmentId?.ToString(),
            RootBusinessKeyType = "Assignment",
        };
    }
}
