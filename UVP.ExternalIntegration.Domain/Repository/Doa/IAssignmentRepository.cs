namespace UVP.ExternalIntegration.Domain.Repository.Doa
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UVP.ExternalIntegration.Domain.Entity.Doa;
    using UVP.Shared.Micro.Entities.Sql.Entities;

    public interface IAssignmentRepository
    {
        Task<UserAssignment> GetAssignmentByIdAsync(long assignmentId, bool migrated = false);

        Task<SalaryBasis> GetSalaryBasisDetailsByDoaIdAsync(long doaCandidateId);

        Task<CoaNonPPMDetails> GetCoaNONPPMDetailsByDoaIdAsync(long doaCandidateId);

        Task<CoaPPMDetails> GetCoaPPMDetailsByDoaIdAsync(long doaCandidateId);

        Task<bool> IsHREntryDateAllowedByIdAsync(long doaCandidateId);

        Task<PayrollOTEDetails> GetPayrollOTEDetailsForQuantumPostingAsync();

        Task<int> UpdatePayrollOTEWithQuantumEntryId(long PaymentTableId, long QuantumEntryId, bool isSuccess);

        Task<FundingDetails[]> GetFundingDetailsByDoaIdAsync(long doaCandidateId);

        Task<APIStatusDisplay[]> GetAPIStatusDisplayByDoaIdAsync(long doaCandidateId);

        Task<int> UpdateCoaPPMVersion(long coaPPMCoaId, long? versionId, long? laborScheduleId, string erpStatus);

        Task<int> UpdateCoaNonPPMVersion(long coaNonPPMCoaId, long? personInfoId);

        Task<int> UpdateCoaNonPPMRuleLineHash(long coaNonPPMCoaId, string ruleLineHash, int? serialNumber);

        Task<int> UpdateCoaPPMRuleLine(long coaPPMCoaRuleId, long? distributionRuleId, int? serialNumber);

        Task<Tuple<int, string>> IsDuplicateEntry(long doaCandidateId);       

        Task<bool> IsDuplicateEntryPayroll(long doaCandidateId);

        Task<bool> SaveApiTransaction(List<ErpErrorFrameworkAPITransactionManual> listErpErrorFrameworkAPITransactionManual);

        Task<List<long>> GetErpErrorFrameworkAPITransactions();
        Task<int> ManualHireAsync(ManualHire model, string logType = "ErpManualHire");


        Task<bool> AnyErpErrorFrameworkAPIManualTransactionForCandidate(long doaCandidateId);

        /// <summary>
        /// Update the ErpContactHash and ErpPersonNumber of Dependent.
        /// </summary>
        /// <param name="doaCandidateId">doaCandidateId.</param>
        /// <param name="personNumber">personNumber.</param>
        /// <param name="link">link.</param>
        /// <param name="firstName">firstName.</param>
        /// <param name="lastName">lastName.</param>
        /// <returns>Result of the update.</returns>
        Task<int> UpdateDependentErpContactWithQuantumData(long doaCandidateId, string personNumber, string link, string firstName, string lastName);
        
        Task<List<ErpErrorFrameworkAPITransaction>> GetErpDependentAPITransactions(long doaCandidateId);

        /// <summary>
        /// get external config data by code.
        /// </summary>
        /// <param name="parameters">dictoinary.</param>
        /// <param name="config">string.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        Task<IEnumerable<dynamic>> ExecuteRoutineAsync(ExternalClientConfig config, Dictionary<string, object> parameters);
    }
}
