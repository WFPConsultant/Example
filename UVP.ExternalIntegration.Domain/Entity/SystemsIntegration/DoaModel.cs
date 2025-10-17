namespace UVP.ExternalIntegration.Domain.Entity.SystemsIntegration
{
    using System;
    using Microsoft.EntityFrameworkCore;

    [Keyless]
    public class DoaModel
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
