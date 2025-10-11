namespace UVP.ExternalIntegration.Business.Model.Common
{
    using System;

    public class LegislativeInfos
    {
        public LegislativeInfosItems[] Items { get; set; }
    }

    public class LegislativeInfosItems
    {
        public long? PersonLegislativeId { get; set; }
        public string? EffectiveStartDate { get; set; }
        public string? EffectiveEndDate { get; set; }
        public string? LegislationCode { get; set; }
        public string? Gender { get; set; }
        public string? MaritalStatus { get; set; }
        public string? MaritalStatusChangeDate { get; set; }
        public string? HighestEducationLevel { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public WorkerLink[] Links { get; set; }
    }
}
