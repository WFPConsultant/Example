namespace UVP.ExternalIntegration.Domain.Repository.Doa
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Dapper;
    using Koa.Platform.Injection;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using UVP.ExternalIntegration.Domain.Entity.Doa;
    using UVP.Shared.Micro.Entities.Sql.Entities;

    /// <summary>
    /// UserRepository class.
    /// </summary>
    [Injectable(typeof(IAssignmentRepository))]
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly ILogger<AssignmentRepository> logger;
        private readonly DataDoaContext dataContext;
        private readonly DapperContext dapperContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssignmentRepository"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="dataContext">DataContext.</param>
        /// <param name="dapperContext">dapperContext.</param>

        public AssignmentRepository(ILogger<AssignmentRepository> logger, DataDoaContext dataContext, DapperContext dapperContext)
        {
            this.logger = logger;
            this.dataContext = dataContext;
            this.dapperContext = dapperContext;
        }

        public async Task<UserAssignment> GetAssignmentByIdAsync(long doaCandidateId, bool migrated = false)
        {
            if (migrated)
            {
                return this.dataContext.Assignment.FromSqlInterpolated($"[dbo].[ERP_LeftGetAssignmentDetailsById] {doaCandidateId}").AsEnumerable().FirstOrDefault();
            }
            else
            {
                return this.dataContext.Assignment.FromSqlInterpolated($"[dbo].[ERP_GetAssignmentDetailsById] {doaCandidateId}").AsEnumerable().FirstOrDefault();
            }
        }

        public async Task<SalaryBasis> GetSalaryBasisDetailsByDoaIdAsync(long doaCandidateId)
        {
            var ret = this.dataContext.SalaryBasis.FromSqlInterpolated($"[dbo].[ERP_GetSalaryDetailsByDOAId] {doaCandidateId}").AsEnumerable().FirstOrDefault();
            return ret;
        }

        public async Task<CoaNonPPMDetails> GetCoaNONPPMDetailsByDoaIdAsync(long doaCandidateId)
        {
            var coaDetails = this.dataContext.CoaNONPPM.FromSqlInterpolated($"[dbo].[ERP_GetCoaNonPPMDetailsByDOAId] {doaCandidateId}").ToList<CoaNonPPM>();

            var coaData = new CoaNonPPMDetails();

            coaData.CoaNonPPMList = coaDetails;

            return coaData;


        }

        public async Task<CoaPPMDetails> GetCoaPPMDetailsByDoaIdAsync(long doaCandidateId)
        {
            var coaDetails = this.dataContext.CoaPPM.FromSqlInterpolated($"[dbo].[ERP_GetCoaPPMDetailsByDOAId] {doaCandidateId}").ToList<CoaPPM>();

            var coaData = new CoaPPMDetails();

            coaData.CoaPPMList = coaDetails;

            return coaData;


        }

        public async Task<bool> IsHREntryDateAllowedByIdAsync(long doaCandidateId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@p_DoaCandidateId", SqlDbType = SqlDbType.BigInt, Value = doaCandidateId, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@p_IsEntryAllowed", SqlDbType = SqlDbType.Bit, Direction = System.Data.ParameterDirection.Output },
            };

            await this.dataContext.Database.ExecuteSqlRawAsync("EXEC ERP_CheckHREntryAllowedDatesByDoaId @p_DoaCandidateId,@p_IsEntryAllowed out", parameters.ToArray());
            var ret = Convert.ToBoolean(parameters[1].Value);

            return ret;
        }

        public async Task<PayrollOTEDetails> GetPayrollOTEDetailsForQuantumPostingAsync()
        {
            var payrollOTEDetails = this.dataContext.PayrollOTEData.FromSqlInterpolated($"[dbo].[ERP_GetPaymentOTEDataForQuantumPosting] ").ToList<PayrollOTE>();

            var payrollOTEData = new PayrollOTEDetails();

            payrollOTEData.PayrollOTEList = payrollOTEDetails;

            return payrollOTEData;


        }

        public async Task<int> UpdatePayrollOTEWithQuantumEntryId(long PaymentTableId, long QuantumEntryId, bool isSuccess)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@p_PaymentTableId", SqlDbType = SqlDbType.BigInt, Value = PaymentTableId },
                new SqlParameter { ParameterName = "@p_QuantumEntryId", SqlDbType = SqlDbType.BigInt, Value = QuantumEntryId },
            };


            if (isSuccess)
            {

                var ret = await this.dataContext.Database.ExecuteSqlRawAsync($" Update Payment Set ErpElementEntryId=@p_QuantumEntryId, ErpPostedStatus='Posted_To_Quantum', updatedDate=GETDATE(), updateuser='Integration' WHERE Id=@p_PaymentTableId  ", parameters.ToArray());

                return ret;
            }
            else
            {
                var ret = await this.dataContext.Database.ExecuteSqlRawAsync($" Update Payment Set ErpPostedStatus='Failed', updatedDate=GETDATE(), updateuser='Integration' WHERE Id=@p_PaymentTableId AND ErpPostedStatus IS NULL  ", parameters.ToArray());

                return ret;

            }
        }

        public async Task<FundingDetails[]> GetFundingDetailsByDoaIdAsync(long doaCandidateId)
        {
            var ret = this.dataContext.FundingDetails.FromSqlInterpolated($"[dbo].[ERP_GetFundingDetailsByDoaId] {doaCandidateId}").AsEnumerable();
            if (ret == null)
            {
                return null;
            }

            return ret.ToArray();
        }

        public async Task<APIStatusDisplay[]> GetAPIStatusDisplayByDoaIdAsync(long doaCandidateId)
        {
            var ret = this.dataContext.APIStatusDisplay.FromSqlInterpolated($"[dbo].[ERP_GetAPIStatusByDoaCandidateId] {doaCandidateId}").AsEnumerable();
            if (ret == null)
            {
                return null;
            }

            return ret.ToArray();
        }


        public async Task<int> UpdateCoaPPMVersion(long coaPPMCoaId, long? versionId, long? laborScheduleId, string erpStatus)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@p_CoaId", SqlDbType = SqlDbType.BigInt, Value = coaPPMCoaId },
                 new SqlParameter { ParameterName = "@p_laborScheduleId", SqlDbType = SqlDbType.BigInt, Value = laborScheduleId },
                  new SqlParameter { ParameterName = "@p_versionId", SqlDbType = SqlDbType.BigInt, Value = versionId },
                   new SqlParameter { ParameterName = "@p_erpVersionStatus", SqlDbType = SqlDbType.VarChar, Value = erpStatus },

            };

            if (coaPPMCoaId > 0)
            {
                var ret = await this.dataContext.Database.ExecuteSqlRawAsync($" Update ErpCoa Set ErpLaborScheduleId=@p_laborScheduleId, VersionStatus= 'PostedToQuantum', ErpVersionStatus=@p_erpVersionStatus, ErpVersionId=@p_versionId, updatedDate=GETDATE(), updateuser='Integration' WHERE Id=@p_CoaId  ", parameters.ToArray());

                return ret;

            }
            else
            {
                return 0;

            }



        }

        public async Task<int> UpdateCoaNonPPMVersion(long coaNonPPMCoaId, long? personInfoId)
        {

            var parameters = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@p_CoaId", SqlDbType = SqlDbType.BigInt, Value = coaNonPPMCoaId },
                 new SqlParameter { ParameterName = "@p_PersonInfoId", SqlDbType = SqlDbType.BigInt, Value = personInfoId },

            };

            if (coaNonPPMCoaId > 0)
            {
                var ret = await this.dataContext.Database.ExecuteSqlRawAsync($" Update ErpCoa Set ErpLaborScheduleId=@p_PersonInfoId, VersionStatus= 'PostedToQuantum', ErpVersionStatus='PostedToQuantum', updatedDate=GETDATE(), updateuser='Integration' WHERE Id=@p_CoaId  ", parameters.ToArray());

                return ret;

            }
            else
            {
                return 0;

            }
        }

        public async Task<int> UpdateCoaNonPPMRuleLineHash(long coaNonPPMCoaRuleId, string ruleLineHash, int? serialNumber)
        {

            var parameters = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@p_coaRuleId", SqlDbType = SqlDbType.BigInt, Value = coaNonPPMCoaRuleId },
                 new SqlParameter { ParameterName = "@p_ruleLineHash", SqlDbType = SqlDbType.VarChar, Value = ruleLineHash },
                  new SqlParameter { ParameterName = "@p_serialNumber", SqlDbType = SqlDbType.Int, Value = serialNumber },

            };

            if (coaNonPPMCoaRuleId > 0)
            {
                var ret = await this.dataContext.Database.ExecuteSqlRawAsync($" Update ErpCoaDistributionRulesHCM Set ErpRuleLineHash=@p_ruleLineHash, SerialNumber= @p_serialNumber, updatedDate=GETDATE(), updateuser='Integration' WHERE Id=@p_coaRuleId  ", parameters.ToArray());

                return ret;

            }
            else
            {
                return 0;
            }



        }

        public async Task<int> UpdateCoaPPMRuleLine(long coaPPMCoaRuleId, long? distributionRuleId, int? serialNumber)
        {

            var parameters = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@p_coaRuleId", SqlDbType = SqlDbType.BigInt, Value = coaPPMCoaRuleId },
                 new SqlParameter { ParameterName = "@p_distributionRuleId", SqlDbType = SqlDbType.VarChar, Value = distributionRuleId },
                  new SqlParameter { ParameterName = "@p_serialNumber", SqlDbType = SqlDbType.Int, Value = serialNumber },

            };

            if (coaPPMCoaRuleId > 0)
            {
                var ret = await this.dataContext.Database.ExecuteSqlRawAsync($" Update ErpCoaDistributionRulesPPM Set ErpRuleLineNumber=@p_serialNumber, ErpDistributionRuleId= @p_distributionRuleId, updatedDate=GETDATE(), updateuser='Integration' WHERE Id=@p_coaRuleId  ", parameters.ToArray());

                return ret;

            }
            else
            {
                return 0;
            }



        }

        public async Task<Tuple<int, string>> IsDuplicateEntry(long doaCandidateId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@p_DoaCandidateId", SqlDbType = SqlDbType.BigInt, Value = doaCandidateId, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@p_ResponseCode", SqlDbType = SqlDbType.BigInt, Direction = System.Data.ParameterDirection.Output },
                new SqlParameter { ParameterName = "@p_ErpPersonNumber", SqlDbType = SqlDbType.NVarChar, Direction = System.Data.ParameterDirection.Output, Size = 50 },
            };

            await this.dataContext.Database.ExecuteSqlRawAsync("EXEC ERP_CheckDuplicateEntryByDoaId @p_DoaCandidateId,@p_ResponseCode out, @p_ErpPersonNumber out", parameters.ToArray());
            return Tuple.Create<int, string>(Convert.ToInt32(parameters[1].Value), Convert.ToString(parameters[2].Value));
        }

        public async Task<bool> IsDuplicateEntryPayroll(long paymentId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@p_PaymentId", SqlDbType = SqlDbType.BigInt, Value = paymentId, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@p_IsDuplicateEntry", SqlDbType = SqlDbType.Bit, Direction = System.Data.ParameterDirection.Output },
            };

            await this.dataContext.Database.ExecuteSqlRawAsync("EXEC ERP_CheckDuplicateEntryByPaymentId @p_PaymentId,@p_IsDuplicateEntry out", parameters.ToArray());
            var ret = Convert.ToBoolean(parameters[1].Value);

            return ret;
        }

        public async Task<bool> SaveApiTransaction(List<ErpErrorFrameworkAPITransactionManual> listErpErrorFrameworkAPITransactionManual)
        {
            try
            {
                this.dataContext.ErpErrorFrameworkAPITransactionManual.AddRange(listErpErrorFrameworkAPITransactionManual);
                this.dataContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return false;

        }

        public async Task<List<long>> GetErpErrorFrameworkAPITransactions()
        {
            List<long> result = null;
            result = await this.dataContext.ErpErrorFrameworkAPITransaction.Where(k => k.APICode == "ErpHire" && k.StatusCode == "Success" && k.DoaCandidateId.HasValue)
                .Select(k => k.DoaCandidateId.Value).Distinct()
                .ToListAsync();

            return result;
        }

        public async Task<int> ManualHireAsync(ManualHire model, string logType = "ErpManualHire")
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@p_doaCandidateId", SqlDbType = SqlDbType.BigInt, Value = model.DoaCandidateId, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@p_erpAssignmentNumber", SqlDbType = SqlDbType.VarChar, Value = model.ErpAssignmentNumber, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@p_erpAssignmentId", SqlDbType = SqlDbType.BigInt, Value = model.ErpAssignmentId, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@p_erpAssignmentHash", SqlDbType = SqlDbType.VarChar, Value = model.ErpAssignmentHash, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@p_erpPersonNumber", SqlDbType = SqlDbType.VarChar, Value = model.ErpPersonNumber, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@p_erpPersonId", SqlDbType = SqlDbType.BigInt, Value = model.ErpPersonId, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@p_erpPersonHash", SqlDbType = SqlDbType.VarChar, Value = model.ErpPersonHash, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@p_erpContractNumber", SqlDbType = SqlDbType.VarChar, Value = model.ErpContractNumber, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@p_erpContractId", SqlDbType = SqlDbType.BigInt, Value = model.ErpContractId, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@p_erpContractHash", SqlDbType = SqlDbType.VarChar, Value = model.ErpContractHash, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@p_userId", SqlDbType = SqlDbType.BigInt, Value = model.UserId, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@p_logType", SqlDbType = SqlDbType.VarChar, Value = logType, Direction = System.Data.ParameterDirection.Input },
                new SqlParameter { ParameterName = "@p_reasonCode", SqlDbType = SqlDbType.VarChar, Value = model.ReasonCode, Direction = System.Data.ParameterDirection.Input, IsNullable = true },
                new SqlParameter { ParameterName = "@p_reasonTableCode", SqlDbType = SqlDbType.VarChar, Value = model.ReasonTableCode, Direction = System.Data.ParameterDirection.Input, IsNullable = true },
            };

            var ret = await this.dataContext.Database.ExecuteSqlRawAsync("EXEC Erp_SaveManualHireQuantumDataInUVP @p_doaCandidateId,@p_erpAssignmentNumber,@p_erpAssignmentId,@p_erpAssignmentHash,@p_erpPersonNumber,@p_erpPersonId,@p_erpPersonHash,@p_erpContractNumber,@p_erpContractId,@p_erpContractHash,@p_userId,@p_logType,@p_reasonCode,@p_reasonTableCode", parameters.ToArray());
            return ret;
        }

        public async Task<bool> AnyErpErrorFrameworkAPIManualTransactionForCandidate(long doaCandidateId)
        {
            return await this.dataContext.ErpErrorFrameworkAPITransaction
                .Where(k => new List<string> { "ErpHire", "ErpCheckPersonExists" }.Contains(k.APICode) && k.DoaCandidateId == doaCandidateId)
                .AllAsync(k => k.StatusCode == "Failure");
        }

        public async Task<List<ErpErrorFrameworkAPITransaction>> GetErpDependentAPITransactions(long doaCandidateId)
        {
            List<ErpErrorFrameworkAPITransaction> result = null;
            result = await this.dataContext.ErpErrorFrameworkAPITransaction.Where(k => k.APICode == "ErpDepedent" && k.DoaCandidateId == doaCandidateId).ToListAsync();

            return result;
        }

        /// <summary>
        /// Update the ErpContactHash and ErpPersonNumber of Dependent.
        /// </summary>
        /// <param name="doaCandidateId">doaCandidateId.</param>
        /// <param name="personNumber">personNumber.</param>
        /// <param name="link">link.</param>
        /// <param name="firstName">firstName.</param>
        /// <param name="lastName">lastName.</param>
        /// <returns>Result of the update.</returns>
        public async Task<int> UpdateDependentErpContactWithQuantumData(long doaCandidateId, string personNumber, string link, string firstName, string lastName)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@p_ErpContactIdentifier", SqlDbType = SqlDbType.VarChar, Value = personNumber },
                new SqlParameter { ParameterName = "@p_ErpContactHash", SqlDbType = SqlDbType.VarChar, Value = link },
                new SqlParameter { ParameterName = "@p_DoaCandidateId", SqlDbType = SqlDbType.BigInt, Value = doaCandidateId },
                new SqlParameter { ParameterName = "@p_FirstName", SqlDbType = SqlDbType.VarChar, Value = firstName },
                new SqlParameter { ParameterName = "@p_LastName", SqlDbType = SqlDbType.VarChar, Value = lastName },
            };

            var ret = await this.dataContext.Database.ExecuteSqlRawAsync($" Update DoaCandidateDependent Set ErpContactHash=@p_ErpContactHash, ErpContactIdentifier=@p_ErpContactIdentifier, updatedDate=GETDATE(), updateuser='Integration' WHERE DoaCandidateId=@p_DoaCandidateId  and REPLACE(FirstName, ' ', '') = @p_FirstName and  REPLACE(LastName, ' ', '')  = @p_LastName", parameters.ToArray());

            return ret;
        }

        /// <summary>
        /// Executes a stored procedure or view with the provided parameters using Dapper.
        /// </summary>
        /// <param name="config">The external client configuration containing routine details.</param>
        /// <param name="parameters">Dictionary of parameters to pass to the routine.</param>
        /// <returns>Dynamic result set from the executed routine.</returns>
        /// <exception cref="NotSupportedException">Thrown when routine type is not supported.</exception>
        public async Task<IEnumerable<dynamic>> ExecuteRoutineAsync(ExternalClientConfig config, Dictionary<string, object> parameters)
        {
            using var connection = this.dapperContext.CreateConnection();

            // Convert dictionary parameters to Dapper dynamic parameters
            var dynamicParams = new DynamicParameters();
            foreach (var param in parameters)
            {
                dynamicParams.Add(param.Key, GetObjectValue(param.Value));
            }

            // Determine command type and query based on routine type
            var commandType = config.CallingRoutineType.ToUpper() switch
            {
                "STOREDPROC" => CommandType.StoredProcedure,
                "VIEW" => CommandType.Text,
                _ => throw new NotSupportedException($"Unsupported routine type: {config.CallingRoutineType}")
            };

            // Build query string - Views need SELECT statement, Stored Procedures use routine name directly
            var query = string.Equals(config.CallingRoutineType, "VIEW", StringComparison.OrdinalIgnoreCase)
                ? $"SELECT * FROM {config.CallingRoutine}" : config.CallingRoutine;

            return await connection.QueryAsync(query, dynamicParams, commandType: commandType);
        }

        /// <summary>
        /// Extracts the actual value from a JsonElement object based on its type.
        /// </summary>
        /// <param name="obj">The JsonElement object to extract value from.</param>
        /// <returns>The converted value as appropriate .NET type (long, bool, or string).</returns>
        private static object GetObjectValue(object obj) => ((JsonElement)obj).ValueKind switch
        {
            JsonValueKind.Number => long.Parse(obj.ToString()),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => obj.ToString()
        };
    }
}
