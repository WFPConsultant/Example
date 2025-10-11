namespace UVP.ExternalIntegration.Business.Model.Common
{
    using System;

    public class Addresses
    {
        public AddressesItems[] Items { get; set; }
    }

    public class AddressesItems
    {
        public long? AddressId { get; set; }
        public string? EffectiveStartDate { get; set; }
        public string? EffectiveEndDate { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }
        public string? AddressLine4 { get; set; }
        public string? TownOrCity { get; set; }
        public string? Region1 { get; set; }
        public string? Region2 { get; set; }
        public string? Region3 { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? LongPostalCode { get; set; }
        public string? AddlAddressAttribute1 { get; set; }
        public string? AddlAddressAttribute2 { get; set; }
        public string? AddlAddressAttribute3 { get; set; }
        public string? AddlAddressAttribute4 { get; set; }
        public string? AddlAddressAttribute5 { get; set; }
        public string? Building { get; set; }
        public string? FloorNumber { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public long? PersonAddrUsageId { get; set; }
        public string? AddressType { get; set; }
        public bool? PrimaryFlag { get; set; }
        public WorkerLink[] Links { get; set; }
    }
}
