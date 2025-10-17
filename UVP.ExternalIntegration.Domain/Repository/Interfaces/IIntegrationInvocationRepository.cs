namespace UVP.ExternalIntegration.Domain.Repository.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using UVP.ExternalIntegration.Domain.Entity.Integration;

    public interface IIntegrationInvocationRepository : IGenericRepository<IntegrationInvocationModel>
    {
        Task<IEnumerable<IntegrationInvocationModel>> GetPendingInvocationsAsync();
        Task<List<IntegrationInvocationModel>> GetRetryableInvocationsAsync(DateTime utcNow, int take = 200, CancellationToken ct = default);
    }
}
