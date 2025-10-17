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

        public DbSet<IntegrationInvocationModel> IntegrationInvocations { get; set; }

        public DbSet<IntegrationInvocationLogModel> IntegrationInvocationLogs { get; set; }

        public DbSet<IntegrationEndpointConfigurationModel> IntegrationEndpointConfigurations { get; set; }

        public DbSet<DoaCandidateClearancesModel> DoaCandidateClearances { get; set; }

        public DbSet<DoaCandidateClearancesOneHRModel> DoaCandidateClearancesOneHR { get; set; }

        public DbSet<DoaCandidateModel> DoaCandidates { get; set; }

        public DbSet<CandidateModel> Candidates { get; set; }

        public DbSet<DoaModel> Doas { get; set; }

        public DbSet<UserModel> Users { get; set; }

        public DbSet<DutyStationModel> DutyStationValues { get; set; }

        public DbSet<AssignmentModel> Assignments { get; set; }

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

           
            modelBuilder.Entity<DoaCandidateModel>().ToTable("DoaCandidate", "dbo");
            modelBuilder.Entity<DoaModel>().ToTable("Doa", "dbo");
            modelBuilder.Entity<CandidateModel>().ToTable("Candidate", "dbo");
            modelBuilder.Entity<UserModel>().ToTable("User", "dbo");
            modelBuilder.Entity<DutyStationModel>().ToTable("DutyStationValue", "dbo");
            modelBuilder.Entity<AssignmentModel>().ToTable("Assignment", "dbo");
            modelBuilder.Entity<DoaCandidateClearancesModel>().ToTable("DoaCandidateClearances", "dbo");
            modelBuilder.Entity<DoaCandidateClearancesOneHRModel>().ToTable("DoaCandidateClearancesOneHR", "dbo");

            // IntegrationEndpointConfiguration
            modelBuilder.Entity<IntegrationEndpointConfigurationModel>(entity =>
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
            modelBuilder.Entity<IntegrationInvocationModel>(entity =>
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
            modelBuilder.Entity<IntegrationInvocationLogModel>(entity =>
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
            modelBuilder.Entity<DoaCandidateModel>(entity =>
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
            modelBuilder.Entity<CandidateModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint");
                entity.Property(e => e.UserId).HasColumnType("bigint");
            });

            // DoaCandidateClearances
            modelBuilder.Entity<DoaCandidateClearancesModel>(entity =>
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
            modelBuilder.Entity<DoaCandidateClearancesOneHRModel>(entity =>
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
            modelBuilder.Entity<DoaModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint");
                entity.Property(e => e.Name).HasMaxLength(200);
            });

            // User
            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint");
                entity.Property(e => e.FirstName).HasMaxLength(250).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(250).IsRequired();
                entity.Property(e => e.GenderCode).HasMaxLength(50);
                entity.Property(e => e.PersonalEmail).HasMaxLength(250);
            });

            // DutyStationValue
            modelBuilder.Entity<DutyStationModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint");
                entity.Property(e => e.Code).HasMaxLength(450);
                entity.Property(e => e.ShortDescription).HasMaxLength(50);
            });

            // Assignment
            modelBuilder.Entity<AssignmentModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("bigint");
                entity.Property(e => e.DoaId).HasColumnType("bigint");
                entity.Property(e => e.DutyStationCode).HasMaxLength(50);
            });
        }
    }
}
