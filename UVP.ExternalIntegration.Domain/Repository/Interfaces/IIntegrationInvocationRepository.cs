namespace UVP.ExternalIntegration.Domain.Repository.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using UVP.ExternalIntegration.Domain.Entity.Integration;

    public interface IIntegrationInvocationRepository : IGenericRepository<IntegrationInvocation>
    {
        Task<IEnumerable<IntegrationInvocation>> GetPendingInvocationsAsync();
        Task<List<IntegrationInvocation>> GetRetryableInvocationsAsync(DateTime utcNow, int take = 200, CancellationToken ct = default);
    }
}
