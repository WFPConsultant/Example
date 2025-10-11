namespace UVP.ExternalIntegration.Business.Model.Response
{
    using System;
    using UVP.ExternalIntegration.Business.Model.Common;

    public class WorkerResponseModel
    {
        public WorkerResponseModelItems[] Items { get; set; }
    }

    public class WorkerResponseModelItems
    {
        public long? PersonId { get; set; }
        public string? PersonNumber { get; set; }
        public string? CorrespondenceLanguage { get; set; }
        public string? BloodType { get; set; }
        public string? DateOfBirth { get; set; }
        public string? DateOfDeath { get; set; }
        public string? CountryOfBirth { get; set; }
        public string? RegionOfBirth { get; set; }
        public string? TownOfBirth { get; set; }
        public string? ApplicantNumber { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }        
        public Emails Emails { get; set; }
        public LegislativeInfos LegislativeInfo { get; set; }
        public Names Names { get; set; }
        public WorkRelationships WorkRelationships { get; set; }
        public WorkerLink[] links { get; set; }
    }
}
