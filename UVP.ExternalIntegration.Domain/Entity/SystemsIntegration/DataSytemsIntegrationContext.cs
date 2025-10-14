namespace UVP.ExternalIntegration.Domain.Entity.SystemsIntegration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using UVP.ExternalIntegration.Domain.Entity.Integration;

    public class DataSytemsIntegrationContext : DbContext
    {
        private readonly IConfiguration config;

        public DataSytemsIntegrationContext(DbContextOptions<DataSytemsIntegrationContext> options, IConfiguration config)
               : base(options)
            => this.config = config;

        public virtual DbSet<IntegrationInvocation> IntegrationInvocations { get; set; }

        public virtual DbSet<IntegrationInvocationLog> IntegrationInvocationLogs { get; set; }

        public virtual DbSet<IntegrationEndpointConfiguration> IntegrationEndpointConfigurations { get; set; }

        public DbSet<DoaCandidateClearances> DoaCandidateClearances { get; set; }

        public DbSet<DoaCandidateClearancesOneHR> DoaCandidateClearancesOneHR { get; set; }

        public DbSet<DoaCandidate> DoaCandidates { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Doa> Doas { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(this.config.GetConnectionString("DefaultConnection"));
    }
}
