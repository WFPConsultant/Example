namespace UVP.ExternalIntegration.Business.Model.Common
{
    using System.Collections.Generic;

    public class ContactRelationshipsDFF
    {
        public long ContactRelationshipId { get; set; }
        public string EffectiveStartDate { get; set; }
        public string EffectiveEndDate { get; set; }
        public string allowanceEligibleCheckbox { get; set; }
        public string allowanceEffectiveDate { get; set; }
        public object allowanceExpirationDate { get; set; }
        public string householdMember { get; set; }
        public object contactIdentifier { get; set; }
        public string specialEducationGrant { get; set; }
        public object expiryDateSpecialEducationGran { get; set; }
        public object __FLEX_Context { get; set; }
        public List<WorkerLink> links { get; set; }
    }

}
