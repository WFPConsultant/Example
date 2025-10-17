namespace UVP.ExternalIntegration.Domain.Entity.SystemsIntegration
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using UVP.ExternalIntegration.Domain.Entity.Integration;

    public class DataSytemsIntegrationContext : DbContext
    {
        private readonly IConfiguration config;

        public DataSytemsIntegrationContext(DbContextOptions<DataSytemsIntegrationContext> options, IConfiguration config)
               : base(options)
            => this.config = config;

        public DbSet<IntegrationInvocation> IntegrationInvocations { get; set; }

        public DbSet<IntegrationInvocationLog> IntegrationInvocationLogs { get; set; }

        public DbSet<IntegrationEndpointConfiguration> IntegrationEndpointConfigurations { get; set; }

        public DbSet<DoaCandidateClearances> DoaCandidateClearances { get; set; }

        public DbSet<DoaCandidateClearancesOneHR> DoaCandidateClearancesOneHR { get; set; }

        public DbSet<DoaCandidate> DoaCandidates { get; set; }

        public DbSet<Candidate> Candidates { get; set; }

        public DbSet<Doa> Doas { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<DutyStationValue> DutyStationValues { get; set; }

        public DbSet<Assignment> Assignments { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseSqlServer(this.config.GetConnectionString("DefaultConnection"));

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                this.config.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    // Enable retry on transient failures
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,                   
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null 
                    );
                });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<DoaCandidate>().ToTable("DoaCandidate", "dbo");
            modelBuilder.Entity<Doa>().ToTable("Doa", "dbo");
            modelBuilder.Entity<Candidate>().ToTable("Candidate", "dbo");
            modelBuilder.Entity<User>().ToTable("User", "dbo");
            modelBuilder.Entity<DutyStationValue>().ToTable("DutyStationValue", "dbo");
            modelBuilder.Entity<Assignment>().ToTable("Assignment", "dbo");

            // IntegrationEndpointConfiguration
            modelBuilder.Entity<IntegrationEndpointConfiguration>(entity =>
            {
                entity.HasKey(e => e.IntegrationEndpointId);
                entity.Property(e => e.IntegrationEndpointId).HasColumnType("bigint");  // Explicitly set to bigint
                entity.Property(e => e.IntegrationType).HasMaxLength(50).IsRequired();
                entity.Property(e => e.IntegrationOperation).HasMaxLength(100).IsRequired();
                entity.Property(e => e.BaseUrl).HasMaxLength(500).IsRequired();
                entity.Property(e => e.PathTemplate).HasMaxLength(500).IsRequired();
                entity.Property(e => e.HttpMethod).HasMaxLength(10).IsRequired();
                entity.Property(e => e.CreatedUser).HasMaxLength(100);
                entity.Property(e => e.UpdatedUser).HasMaxLength(100);
                entity.HasIndex(e => new { e.IntegrationType, e.IntegrationOperation, e.IsActive });
            });

            // IntegrationInvocation
            modelBuilder.Entity<IntegrationInvocation>(entity =>
            {
                entity.HasKey(e => e.IntegrationInvocationId);
                entity.Property(e => e.IntegrationInvocationId).HasColumnType("bigint");  // Already bigint
                entity.Property(e => e.IntegrationType).HasMaxLength(50).IsRequired();
                entity.Property(e => e.IntegrationOperation).HasMaxLength(100).IsRequired();
                entity.Property(e => e.IntegrationStatus).HasMaxLength(50).IsRequired();
                entity.Property(e => e.CreatedUser).HasMaxLength(100);
                entity.Property(e => e.UpdatedUser).HasMaxLength(100);
                entity.HasIndex(e => e.IntegrationStatus);
            });

            // IntegrationInvocationLog
            modelBuilder.Entity<IntegrationInvocationLog>(entity =>
            {
                entity.HasKey(e => e.IntegrationInvocationLogId);
                entity.Property(e => e.IntegrationInvocationLogId).HasColumnType("bigint");  // Already bigint
                entity.Property(e => e.IntegrationInvocationId).HasColumnType("bigint");
                entity.Property(e => e.IntegrationStatus).HasMaxLength(50).IsRequired();
                entity.Property(e => e.CreatedUser).HasMaxLength(100);
                entity.HasIndex(e => e.IntegrationInvocationId);

                entity.HasOne(e => e.IntegrationInvocation)
                    .WithMany()
                    .HasForeignKey(e => e.IntegrationInvocationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // DoaCandidate
            modelBuilder.Entity<DoaCandidate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint");
                entity.Property(e => e.DoaId).HasColumnType("bigint");
                entity.Property(e => e.CandidateId).HasColumnType("bigint");
                entity.Property(e => e.AssignmentId).HasColumnType("bigint");
                entity.Property(e => e.TentativeTravelDate).HasMaxLength(100);
                entity.Property(e => e.ContractCalculatedEndDate).HasMaxLength(100);
            });

            // Candidate
            modelBuilder.Entity<Candidate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint");
                entity.Property(e => e.UserId).HasColumnType("bigint");
            });

            // DoaCandidateClearances
            modelBuilder.Entity<DoaCandidateClearances>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint");
                entity.Property(e => e.DoaCandidateId).HasColumnType("bigint");
                entity.Property(e => e.RecruitmentClearanceCode).HasMaxLength(50).IsRequired();
                entity.Property(e => e.RecruitmentClearanceTableCode).HasMaxLength(50).IsRequired();
                entity.Property(e => e.StatusCode).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Outcome).HasMaxLength(50);
                entity.HasIndex(e => e.DoaCandidateId);

                entity.HasOne(e => e.DoaCandidate)
                    .WithMany()
                    .HasForeignKey(e => e.DoaCandidateId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // DoaCandidateClearancesOneHR
            modelBuilder.Entity<DoaCandidateClearancesOneHR>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint");
                entity.Property(e => e.DoaCandidateId).HasColumnType("bigint");
                entity.Property(e => e.CandidateId).HasColumnType("bigint");
                entity.Property(e => e.DoaId).HasColumnType("bigint");
                entity.Property(e => e.DoaCandidateClearanceId).HasMaxLength(100);
                entity.Property(e => e.RVCaseId).HasMaxLength(100);
                entity.HasIndex(e => new { e.DoaCandidateId, e.CandidateId });

                entity.HasOne(e => e.DoaCandidate)
                    .WithMany()
                    .HasForeignKey(e => e.DoaCandidateId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Candidate)
                    .WithMany()
                    .HasForeignKey(e => e.CandidateId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Doa
            modelBuilder.Entity<Doa>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint");
                entity.Property(e => e.Name).HasMaxLength(200);
            });

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint");
                entity.Property(e => e.FirstName).HasMaxLength(250).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(250).IsRequired();
                entity.Property(e => e.GenderCode).HasMaxLength(50);
                entity.Property(e => e.PersonalEmail).HasMaxLength(250);
            });

            // DutyStationValue
            modelBuilder.Entity<DutyStationValue>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint");
                entity.Property(e => e.Code).HasMaxLength(450);
                entity.Property(e => e.ShortDescription).HasMaxLength(50);
            });

            // Assignment
            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint");
                entity.Property(e => e.DoaId).HasColumnType("bigint");
                entity.Property(e => e.DutyStationCode).HasMaxLength(50);
            });
        }
    }
}
