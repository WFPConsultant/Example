using System.Collections.Generic;
using System.Threading.Tasks;
using UVP.ExternalIntegration.Business.Model.Common;
using UVP.ExternalIntegration.Business.Model.Response;
using UVP.Shared.Model.Doa;

namespace UVP.Doa.Business.Saga.Command.DoaCandidateCommand
{
    /// <summary>
    /// Used to call internal user services. It's important to add to the service the AddUVPHttpUser during registration in Startup.ConfigureServices.
    /// <code>
    /// public DefaultConstructor(UserHttp userHttp) { ... }
    ///
    /// public void Method1()
    /// {
    ///     ...........
    ///     userHttp.AddHeaders(Request.Headers);
    ///     var result = await userHttp.UserGetByIdAsync();
    ///     .........
    /// }
    /// </code>
    /// </summary>
    public interface IExternalIntegrationProxyService
    {
        /// <summary>
        /// Create quantum employee
        /// </summary>
        /// <param name="doaCandidateId">long.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<ReturnObject> CreateEmployeeExternalIntegration(long doaCandidateId);

        /// <summary>
        /// Upload Approved Payments in Quantum
        /// </summary>
        /// <returns></returns>
        Task<ReturnObjectPayrollOTE> UploadApprovedPayments();

        /// <summary>
        /// This will return the Quantum link for an assignment.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="assignmentId"></param>
        /// <param name="effectiveDate"></param>
        /// <returns></returns>
        Task<string> GetQuantumLinkforAssignment(string personId, string assignmentId, string effectiveDate);

        /// <summary>
        /// Gets the uvp data for quantum posting.
        /// </summary>
        /// <param name="doaCandidateId">doacandidate id.</param>
        /// <returns>uvp data.</returns>
        Task<ErpVolunteerHiringDataLogResponseModel> GetErpDataPostedToQuantum(long doaCandidateId);

        /// <summary>
        /// Gets the uvp data for quantum posting.
        /// </summary>
        /// <param name="doaCandidateId">doacandidate id.</param>
        /// <returns>uvp data.</returns>
        Task<ErpVolunteerHiringDataLogResponseModel> GetErpDataManualPostedToQuantum(long doaCandidateId);

        /// <summary>
        /// Gets the uvp data for quantum posting.
        /// </summary>
        /// <param name="doaCandidateId">doacandidate id.</param>
        /// <returns>uvp data.</returns>
        Task<List<ErpApiStatus>> GetErpApiStatusForCandidate(long doaCandidateId);

        /// <summary>
        /// Gets the uvp data for quantum posting.
        /// </summary>
        /// <param name="doaCandidateId">doacandidate id.</param>
        /// <returns>uvp data.</returns>
        Task<List<long>> GetErpHiredCandidates();
    }
}
