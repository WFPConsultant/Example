using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UVP.ExternalIntegration.Business.Model.Request;
using UVP.Shared.Micro.Api.UvpPermission;
using UVP.Shared.Model.Profile;

namespace UVP.ExternalIntegration.Api.Controllers.External
{
    /// <summary>
    /// ExternalIntegration Api controller.
    /// </summary>
    public partial class ExternalController
    {
        /// <summary>
        /// Method to get employee by name.
        /// </summary>
        /// <param name="firstName">First Name.</param>
        /// <param name="lastName">Last Name.</param>
        /// <param name="dateOfBirth">Date Of Birth.</param>
        /// <param name="gender">Gender.</param>
        /// <returns>Worker.</returns>
        [HttpGet("search/{firstName}/{lastName}/{dateOfBirth}/{gender}")]
        [UvpAuthorize(PermissionCode.IntegrationGetWorker)]
        public async Task<IActionResult> GetWorkerByName([FromRoute(Name = "firstName")] string firstName, [FromRoute(Name = "lastName")] string lastName, [FromRoute(Name = "dateOfBirth")] string dateOfBirth, [FromRoute(Name = "gender")] string gender)
        {
            this.logger.LogDebug($"GetWorkerByName: firstName={0},lastName{1},dateOfBirth{2}, gender{3}", firstName, lastName, dateOfBirth, gender);

            var model = new EmployeeSearchRequestModel
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Gender = gender,
            };
            var response = await this.externalIntegrationService.GetWorkerByName(model);
            return this.Ok(response);
        }

        /// <summary>
        /// Method to get the UVP data for quantum posting.
        /// </summary>
        /// <param name="doaCandidateId">DoA Candidate ID.</param>
        /// <returns>Data.</returns>
        [HttpGet("assignment/candidates/{doaCandidateId}")]
        [UvpAuthorize(PermissionCode.IntegrationGetUVPData)]
        public async Task<IActionResult> GetUVPDataForQuantumPosting(long doaCandidateId)
        {
            this.logger.LogDebug($"GetAssignmentDetails: doaCandidateId={0}", doaCandidateId);
            var response = await this.externalIntegrationService.GetUVPDataForQuantumPosting(doaCandidateId);
            return this.Ok(response);
        }

        /// <summary>
        /// Method to get the UVP data for quantum posting.
        /// </summary>
        /// <param name="doaCandidateId">Doa Candidate Id.</param>
        /// <returns>Action result.</returns>
        [HttpGet("assignment/candidates/{doaCandidateId}/log")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUVPDataForQuantumPostingLog(long doaCandidateId)
        {
            this.logger.LogDebug($"GetAssignmentDetailslog: doaCandidateId={0}", doaCandidateId);
            var response = await this.externalIntegrationService.GetErpVolunteerHiringDataLog(doaCandidateId);
            return this.Ok(response);
        }

        /// <summary>
        /// Method to get the UVP data for quantum posting.
        /// </summary>
        /// <param name="doaCandidateId">Doa Candidate Id.</param>
        /// <returns>Action result.</returns>
        [HttpGet("assignment/candidates/{doaCandidateId}/manual")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUVPDataForQuantumPostingManual(long doaCandidateId)
        {
            this.logger.LogDebug($"GetAssignmentDetailslog: doaCandidateId={0}", doaCandidateId);
            var response = await this.externalIntegrationService.GetErpVolunteerManualHiringDataLog(doaCandidateId);
            return this.Ok(response);
        }

        /// <summary>
        /// Method to get the UVP data for quantum posting.
        /// </summary>
        /// <param name="doaCandidateId">Doa Candidate Id.</param>
        /// <returns>Action result.</returns>
        [HttpGet("assignment/candidates/{doaCandidateId}/apiStatus")]
        [AllowAnonymous]
        public async Task<IActionResult> GetErpApiStatus(long doaCandidateId)
        {
            this.logger.LogDebug($"GetAssignmentDetailslog: doaCandidateId={0}", doaCandidateId);
            var response = await this.externalIntegrationService.GetAPIStatusDisplayByDoaCandidateId(doaCandidateId);
            return this.Ok(response);
        }

        /// <summary>
        /// Method to get the UVP data for quantum posting.
        /// </summary>
        /// <param name="doaCandidateId">Doa Candidate Id.</param>
        /// <returns>Action result.</returns>
        [HttpGet("assignment/candidates/hired/ids")]
        [AllowAnonymous]
        public async Task<IActionResult> GetErpErrorFrameworkAPITransactionsLog()
        {
            this.logger.LogDebug("GetErpErrorFrameworkAPITransactionsLog");
            var response = await this.externalIntegrationService.GetErpErrorFrameworkAPITransactionRequest();
            return this.Ok(response);
        }

        /// <summary>
        /// This function will return Quantum link for assigment.
        /// </summary>
        /// <param name="personId">Person ID.</param>
        /// <param name="assignmentId">Assignment ID.</param>
        /// <param name="effectiveDate">Hire date / contract start date.</param>
        /// <returns>URL Link.</returns>
        [HttpGet("assignment/getquantumlink/{personId}/{assignmentId}/{effectiveDate}")]
        [UvpAuthorize(PermissionCode.IntegrationGetQuantumLink)]
        public async Task<IActionResult> GetQuantumLinkForAssigment([FromRoute(Name = "personId")] string personId, [FromRoute(Name = "assignmentId")] string assignmentId, [FromRoute(Name = "effectiveDate")] string effectiveDate)
        {

            this.logger.LogDebug($"Generation of Link for Quantum for a specific Assignment : {1} and Person : {0}", assignmentId, personId);
            var response = await this.externalIntegrationService.GetQuantumAssignmentLink(personId, assignmentId, effectiveDate);
            return this.Ok(response);

        }

        /// <summary>
        /// This function would be used from external app and will return Uvp Data as a json string.
        /// </summary>
        /// <param name="ExternalIntegerationRequestModel">model</param>
        /// <returns>Json string</returns>
        [HttpPost("getUVPDataForExternalApp")]
        [UvpAuthorize(PermissionCode.IntegrationGetUVPData)]
        public async Task<IActionResult> GetUVPDataForExternalApp([FromBody] ExternalIntegerationRequestModel model)
        {
            model.RoleId = this.ApiUser.RoleId;
            model.UserId = this.ApiUser.UserId;
            var response = await this.externalIntegrationService.GetUVPDataForExternalAppAsync(model);
            return this.Ok(response);
        }
    }
}
