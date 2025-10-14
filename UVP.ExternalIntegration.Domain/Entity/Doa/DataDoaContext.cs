namespace UVP.ExternalIntegration.Domain.Entity.Doa
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using UVP.ExternalIntegration.Domain.Entity.Integration;

    public class DataDoaContext : DbContext
    {
        private readonly IConfiguration config;

        public DataDoaContext(DbContextOptions<DataDoaContext> options, IConfiguration config)
               : base(options)
            => this.config = config;

        public virtual DbSet<UserAssignment> Assignment { get; set; }

        public virtual DbSet<SalaryBasis> SalaryBasis { get; set; }

        public virtual DbSet<CoaNonPPM> CoaNONPPM { get; set; }

        public virtual DbSet<CoaPPM> CoaPPM { get; set; }

        public virtual DbSet<PayrollOTE> PayrollOTEData { get; set; }

        public virtual DbSet<FundingDetails> FundingDetails { get; set; }

        public virtual DbSet<APIStatusDisplay> APIStatusDisplay { get; set; }

        public virtual DbSet<ErpErrorFrameworkAPITransactionManual> ErpErrorFrameworkAPITransactionManual { get; set; }

        public virtual DbSet<ErpErrorFrameworkAPITransaction> ErpErrorFrameworkAPITransaction { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(this.config.GetConnectionString("DefaultConnection"));
    }
}
