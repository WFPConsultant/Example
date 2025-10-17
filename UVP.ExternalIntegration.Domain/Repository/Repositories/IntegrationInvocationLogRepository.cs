namespace UVP.ExternalIntegration.Domain.Repository.Repositories
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using UVP.ExternalIntegration.Domain.Entity.Integration;
    using UVP.ExternalIntegration.Domain.Entity.SystemsIntegration;
    using UVP.ExternalIntegration.Domain.Repository.Interfaces;
    using UVP.Shared.Micro.Entities.Sql;

    public class IntegrationInvocationLogRepository : GenericRepository<IntegrationInvocationLog>, IIntegrationInvocationLogRepository
    {
        public IntegrationInvocationLogRepository(DataSytemsIntegrationContext db) : base(db) { }

        public async Task<IntegrationInvocationLog?> GetFirstRequestLogAsync(long invocationId, CancellationToken ct = default)
        {
            return await _dbSet
                .Where(x => x.IntegrationInvocationId == invocationId && x.RequestPayload != null)
                .OrderBy(x => x.CreatedOn)
                .FirstOrDefaultAsync(ct);
        }
    }
}
