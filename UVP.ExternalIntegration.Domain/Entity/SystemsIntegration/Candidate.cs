namespace UVP.ExternalIntegration.Domain.Entity.SystemsIntegration
{
    using System;
    using Microsoft.EntityFrameworkCore;

    [Keyless]
    public class Candidate
    {
        public long Id { get; set; }
        public long UserId { get; set; }
    }
}
