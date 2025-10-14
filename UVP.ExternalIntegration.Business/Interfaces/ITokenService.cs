namespace UVP.ExternalIntegration.Business.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// Generic interface for OAuth token management across different integration systems
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Gets or refreshes access token for the specified integration type
        /// </summary>
        Task<string?> GetAccessTokenAsync(string integrationType);

        /// <summary>
        /// Invalidates cached token for the specified integration type
        /// </summary>
        void InvalidateToken(string integrationType);
    }
}
