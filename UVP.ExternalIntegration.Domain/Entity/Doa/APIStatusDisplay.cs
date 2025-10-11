namespace UVP.ExternalIntegration.Domain.Entity.Doa
{
    using Microsoft.EntityFrameworkCore;

    [Keyless]
    public class APIStatusDisplay
    {
        /// <summary>
        /// Gets or sets APIName.
        /// </summary>
        public string? APIName { get; set; }

        /// <summary>
        /// Gets or sets APICode.
        /// </summary>
        public string? APICode { get; set; }

        /// <summary>
        /// Gets or sets APIStatus.
        /// </summary>
        public string? APIStatus { get; set; }
    }
}
