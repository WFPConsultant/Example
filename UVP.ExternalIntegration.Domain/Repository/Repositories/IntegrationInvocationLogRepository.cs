namespace UVP.ExternalIntegration.Domain.Repository.Repositories
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using UVP.ExternalIntegration.Domain.Entity.Doa;
    using UVP.ExternalIntegration.Domain.Entity.Integration;
    using UVP.ExternalIntegration.Domain.Repository.Interfaces;

    public class IntegrationInvocationLogRepository : GenericRepository<IntegrationInvocationLog>, IIntegrationInvocationLogRepository
    {
        public IntegrationInvocationLogRepository(DataDoaContext db) : base(db) { }

        public async Task<IntegrationInvocationLog?> GetFirstRequestLogAsync(long invocationId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(x => x.IntegrationInvocationId == invocationId && x.RequestPayload != null)
                .OrderBy(x => x.CreatedOn)
                .FirstOrDefaultAsync(ct);
        }
    }
}
