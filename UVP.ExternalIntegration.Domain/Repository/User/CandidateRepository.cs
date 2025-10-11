namespace UVP.ExternalIntegration.Domain.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Koa.Platform.Injection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using UVP.ExternalIntegration.Domain.Entity.Users;
    using UVP.ExternalIntegration.Domain.Repository.Users;

    /// <summary>
    /// UserRepository class.
    /// </summary>
    [Injectable(typeof(ICandidateRepository))]
    public class CandidateRepository : ICandidateRepository
    {
        private readonly ILogger<CandidateRepository> logger;
        private readonly DataUserContext dataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CandidateRepository"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="dataContext">DataContext.</param>
        public CandidateRepository(ILogger<CandidateRepository> logger, DataUserContext dataContext)
        {
            this.logger = logger;
            this.dataContext = dataContext;
        }

        public async Task<Candidate> GetCandidateByIdAsync(long doaCandidateId)
        {
            var ret = this.dataContext.Candidate.FromSqlInterpolated($"[dbo].[ERP_GetCandidateDetailsById] {doaCandidateId}").AsEnumerable().FirstOrDefault();
            return ret;
        }

        public async Task<DependentDetails> GetDependentsByDoaCandidateIdAsync(long doaCandidateId)
        {
            try
            {
                // Execute the stored procedure and fetch the data
                var dependentList = await this.dataContext.Dependent
                    .FromSqlInterpolated($"[dbo].[ERP_GetDependentDetailsByDOAId] {doaCandidateId}")
                    .ToListAsync();

                // Handle the case where no results are returned
                if (dependentList == null || !dependentList.Any())
                {
                    return new DependentDetails
                    {
                        DependentList = new List<Dependent>(),
                    };
                }

                // Return the results
                return new DependentDetails
                {
                    DependentList = dependentList,
                };
            }
            catch (Exception ex)
            {
                // Log generic exceptions for debugging
                logger.LogError($"Error: {ex.Message}");
                // Return an empty result in case of failure
                return new DependentDetails
                {
                    DependentList = new List<Dependent>(),
                };
            }
        }

    }
}
