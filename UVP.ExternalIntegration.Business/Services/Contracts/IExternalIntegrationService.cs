using System.Collections.Generic;
using System.Threading.Tasks;
using UVP.ExternalIntegration.Business.Model.Common;
using UVP.ExternalIntegration.Business.Model.Request;
using UVP.ExternalIntegration.Business.Model.Response;

namespace UVP.ExternalIntegration.Business.Services.Contracts
{
    /// <summary>
    /// Implements <see cref="IExternalIntegrationService"/> interface.
    /// </summary>
    public interface IExternalIntegrationService
    {
        /// <summary>
        /// Get the worker by name.
        /// </summary>
        /// <param name="model">EmployeeSearchRequestModel.</param>
        /// <returns>WorkerResponseModel.</returns>
        Task<ReturnObject> GetWorkerByName(EmployeeSearchRequestModel model);

        /// <summary>
        /// Create employee.
        /// </summary>
        /// <param name="doaCandidateId">long.</param>
        /// <returns>string.</returns>
        Task<ReturnObject> CreateEmployee(long doaCandidateId);

        /// <summary>
        /// Get UVP data for quantum posting.
        /// </summary>
        /// <param name="doaCandidateId">long.</param>
        /// <returns>ReturnObject.</returns>
        Task<ReturnObject> GetUVPDataForQuantumPosting(long doaCandidateId);

        /// <summary>
        /// Get UVP data for quantum posting.
        /// </summary>
        /// <param name="doaCandidateId">long.</param>
        /// <returns>ReturnObject.</returns>
        Task<ErpVolunteerHiringDataLogResponseModel> GetErpVolunteerHiringDataLog(long doaCandidateId);

        /// <summary>
        /// Get UVP data for quantum posting (manual).
        /// </summary>
        /// <param name="doaCandidateId">long.</param>
        /// <returns>ReturnObject.</returns>
        Task<ErpVolunteerHiringDataLogResponseModel> GetErpVolunteerManualHiringDataLog(long doaCandidateId);

        /// <summary>
        /// Get UVP data for quantum posting.
        /// </summary>
        /// <param name="doaCandidateId">long.</param>
        /// <returns>ReturnObject.</returns>
        Task<APIStatusDisplayModel[]> GetAPIStatusDisplayByDoaCandidateId(long doaCandidateId);

        /// <summary>
        /// Post Payroll OTE data to Quantum.
        /// </summary>
        /// <param name="doaCandidateId">long.</param>
        /// <returns>ReturnObjectPayrollOTE.</returns>
        Task<ReturnObjectPayrollOTE> PostPayrollOTEDataToQuantum();

        /// <summary>
        /// Generate Quantum link.
        /// </summary>
        /// <param name="PersonId">string</param>
        /// <param name="AssignmentId">string</param>
        /// <param name="EffectiveDate">string</param>
        /// <returns>Link.</returns>
        Task<string> GetQuantumAssignmentLink(string PersonId, string AssignmentId, string EffectiveDate);

        /// <summary>
        /// Save Api Transaction Manual
        /// </summary>
        /// <param name="listErpErrorFrameworkAPITransactionManual"></param>
        /// <returns></returns>
        Task<ReturnObject> SaveApiTransaction(SaveStatusRequestModel saveStatusRequestModel);

        Task<List<long>> GetErpErrorFrameworkAPITransactionRequest();

        /// <summary>
        /// Manual hire.
        /// </summary>
        /// <param name="model">ManualHireRequestModel.</param>
        /// <returns>ReturnObject.</returns>
        Task<ReturnObject> ManualHire(ManualHireRequestModel model);

        /// <summary>
        /// Manual hire.
        /// </summary>
        /// <param name="model">UpdateErpNumberRequestModel.</param>
        /// <returns>ReturnObject.</returns>
        Task<ReturnObject> ManualUpdate(ManualHireRequestModel model);
        /// <summary>
        /// Update Assignment Status.
        /// </summary>
        /// <param name="doaCandidateId">long.</param>
        /// <returns>string.</returns>
        Task<ReturnObject> UpdateAssignmentStatus(long doaCandidateId);

        /// <summary>
        /// Get UVP data for external app.
        /// </summary>
        /// <param name="model">ExternalIntegerationRequestModel.</param>
        /// <returns>string.</returns>
        Task<string> GetUVPDataForExternalAppAsync(ExternalIntegerationRequestModel model);
    }
}
