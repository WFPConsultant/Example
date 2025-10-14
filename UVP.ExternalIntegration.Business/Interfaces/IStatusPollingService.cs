namespace UVP.ExternalIntegration.Business.Interfaces
{
    using System.Threading.Tasks;

    public interface IStatusPollingService
    {
        /// <summary>
        /// Scan in-flight clearance rows and enqueue GET_CLEARANCE_STATUS invocations where due.
        /// Returns true if executed without unhandled errors.
        /// </summary>
        Task<bool> ProcessOpenClearancesAsync();
        Task<bool> ProcessAcknowledgeAsync();
    }
}
