namespace UVP.ExternalIntegration.Domain.Repository.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using UVP.ExternalIntegration.Domain.Entity.Integration;

    public interface IIntegrationInvocationLogRepository : IGenericRepository<IntegrationInvocationLog>
    {
        /// <summary>
        /// Returns the FIRST log row for the invocation where RequestPayload IS NOT NULL.
        /// This is the bootstrap request row.
        /// </summary>
        Task<IntegrationInvocationLog?> GetFirstRequestLogAsync(long invocationId, CancellationToken ct = default);
    }
}
