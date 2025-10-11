namespace UVP.ExternalIntegration.ErrorValidationFramework.Repository
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using UVP.ExternalIntegration.ErrorValidationFramework.Model;

    public class ErrorDBContext : DbContext
    {
        private readonly IConfiguration config;

        public ErrorDBContext(DbContextOptions<ErrorDBContext> options, IConfiguration config)
               : base(options)
            => this.config = config;

        public virtual DbSet<APITransaction> Transaction { get; set; }

        public virtual DbSet<APITransactionLog> TransactionLog { get; set; }

        public virtual DbSet<APIReTriggerQueue> ReTriggerQueue { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
    }
}
