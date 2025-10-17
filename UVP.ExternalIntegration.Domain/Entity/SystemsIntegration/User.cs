namespace UVP.ExternalIntegration.Domain.Entity.SystemsIntegration
{
    using System;
    using Microsoft.EntityFrameworkCore;

    [Keyless]
    public class User
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        //public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string GenderCode { get; set; }
        public DateTime BirthDate { get; set; }
        public string PersonalEmail { get; set; }
        //public string? NationalityISOCode { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public DateTime? UpdatedDate { get; set; }
    }
}
