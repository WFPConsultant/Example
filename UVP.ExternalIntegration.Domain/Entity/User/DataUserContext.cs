namespace UVP.ExternalIntegration.Domain.Entity.Users
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class DataUserContext : DbContext
    {
        private readonly IConfiguration config;

        public DataUserContext(DbContextOptions<DataUserContext> options, IConfiguration config)
               : base(options) => this.config = config;

        public virtual DbSet<Candidate> Candidate { get; set; }

        public virtual DbSet<Dependent> Dependent { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlServer(this.config.GetConnectionString("DefaultConnection"));
    }
}
