namespace UVP.ExternalIntegration.Business.Model.Response
{
    using System;
    using UVP.ExternalIntegration.Business.Model.Common;

    public class PersonNumberResponse
    {
        public PersonNumberResponseItems[] Items { get; set; }
    }

    public class PersonNumberResponseItems
    {
        public long? PersonId { get; set; }
        public string PersonNumber { get; set; }
        public string CorrespondenceLanguage { get; set; }
        public string BloodType { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfDeath { get; set; }
        public string CountryOfBirth { get; set; }
        public string RegionOfBirth { get; set; }
        public string TownOfBirth { get; set; }
        public long? ApplicantNumber { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }

        public Names names { get; set; }
        public WorkerLink[]? Links { get; set; }
    }
}
