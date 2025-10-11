namespace UVP.ExternalIntegration.Domain.Entity.Doa
{
    using System;
    using System.Data;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Provides database connection management using Dapper ORM with SQL Server.
    /// </summary>
    public class DapperContext
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="DapperContext"/> class.
        /// </summary>
        /// <param name="configuration">The configuration instance containing connection string settings.</param>
        /// <exception cref="ArgumentNullException">Thrown when configuration is null.</exception>
        public DapperContext(IConfiguration configuration)
        {
            var config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.connectionString = config.GetConnectionString("DapperConnection");
        }


        /// <summary>
        /// Creates a new SQL Server database connection using the configured connection string.
        /// </summary>
        /// <returns>An <see cref="IDbConnection"/> instance for database operations.</returns>
        public IDbConnection CreateConnection()
            => new SqlConnection(this.connectionString);
    }
}
