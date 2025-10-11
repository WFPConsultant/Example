namespace UVP.ExternalIntegration.Business.Model.Common
{
    using System;
    using System.Collections.Generic;

    public class ContactRelationships
    {
        public long ContactRelationshipId { get; set; }
        public string EffectiveStartDate { get; set; }
        public string EffectiveEndDate { get; set; }
        public string ContactType { get; set; }
        public string LegislationCode { get; set; }
        public bool EmergencyContactFlag { get; set; }
        public bool PrimaryContactFlag { get; set; }
        public object StatutoryDependent { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public long RelatedPersonId { get; set; }
        public string RelatedPersonNumber { get; set; }
        public List<ContactRelationshipsDFF> contactRelationshipsDFF { get; set; }
        public List<WorkerLink> links { get; set; }
    }
}
