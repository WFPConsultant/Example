namespace UVP.ExternalIntegration.Domain.Repository.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using UVP.ExternalIntegration.Domain.Entity.Doa;
    using UVP.ExternalIntegration.Domain.Entity.Integration;
    using UVP.ExternalIntegration.Domain.Enums;
    using UVP.ExternalIntegration.Domain.Repository.Interfaces;

    public class IntegrationInvocationRepository : GenericRepository<IntegrationInvocation>, IIntegrationInvocationRepository
    {
        public IntegrationInvocationRepository(DataDoaContext context) : base(context)
        {
        }

        public async Task<IEnumerable<IntegrationInvocation>> GetPendingInvocationsAsync()
        {
            return await _dbSet
                .Where(i => i.IntegrationStatus == IntegrationStatus.PENDING.ToString())
                .ToListAsync();
        }
        public async Task<List<IntegrationInvocation>> GetRetryableInvocationsAsync(DateTime utcNow, int take = 200, CancellationToken ct = default)
        {
            //return await _dbSet
            //    .Where(i =>
            //        i.IntegrationStatus == IntegrationStatus.RETRY.ToString() &&
            //        i.NextRetryTime != null &&
            //        i.NextRetryTime <= utcNow)
            //    .OrderBy(i => i.NextRetryTime)
            //    .Take(take)
            //    .ToListAsync(ct);
            return await _dbSet
                .Where(i =>
                    i.IntegrationStatus == IntegrationStatus.RETRY.ToString() &&
                    i.NextRetryTime != null &&
                    i.NextRetryTime <= utcNow)
                // extra safety: in case a concurrent worker flipped status to terminal
                .Where(i => i.IntegrationStatus != IntegrationStatus.PERMANENTLY_FAILED.ToString())
                .OrderBy(i => i.NextRetryTime)
                .Take(take)
                .ToListAsync(ct);
        }
    }
}
