using UVP.Shared.Micro.Domain.Services.Activity;

namespace UVP.ExternalIntegration.Api.Controllers.External
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using UVP.ExternalIntegration.Business.Model.Request;
    using UVP.ExternalIntegration.Business.Services.Contracts;
    using UVP.Shared.Micro.Api.Controllers;
    using UVP.Shared.Micro.Api.UvpPermission;
    using UVP.Shared.Micro.Domain;
    using UVP.Shared.Model.Profile;

    /// <summary>
    /// External controller.
    /// </summary>
    [Route("api/external/integration")]
    public partial class ExternalController : ApiController
    {
        private readonly IExternalIntegrationService externalIntegrationService;
        private readonly ILogger<ExternalController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalController"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="externalIntegrationService">External service.</param>
        public ExternalController(
            ILogger<ExternalController> logger,
            IExternalIntegrationService externalIntegrationService)
        {
            this.externalIntegrationService = externalIntegrationService;
            this.logger = logger ?? NullLogger<ExternalController>.Instance;
        }


        /// <summary>
        /// Method to create the employee.
        /// </summary>
        /// <param name="doaCandidateId">Candidate Id.</param>
        /// <returns>Action result.</returns>
        [HttpPost("employee/candidates/{doaCandidateId}")]
        [UvpAuthorize(PermissionCode.IntegrationCreateEmployee)]
        public async Task<IActionResult> CreateEmployee(long doaCandidateId)
        {
            this.logger.LogDebug($"Create employee");
            var response = await this.externalIntegrationService.CreateEmployee(doaCandidateId);
            return this.Ok(response);
        }

        /// <summary>
        /// Method to get the UVP data for quantum posting.
        /// </summary>
        /// <param name="doaCandidateId">Doa Candidate Id.</param>
        /// <returns>Action result.</returns>
        [HttpPost("payroll/payrollote")]
        [UvpAuthorize(PermissionCode.IntegrationCreatePayrollOTEData)]
        public async Task<IActionResult> PostPayrollOTEDataToQuantum()
        {
            this.logger.LogDebug($"PostPayrollOTEDataToQuantum call is initiated.");
            var response = await this.externalIntegrationService.PostPayrollOTEDataToQuantum();
            return this.Ok(response);
        }

        /// <summary>
        /// Save ErpErrorFrameworkAPITransactionManual
        /// </summary>
        /// <param name="saveStatusRequestModel"></param>
        /// <returns></returns>
        [HttpPost("assignment/saveApiTransactionManual")]
        [UvpAuthorize(PermissionCode.IntegrationCreateEmployee)]
        public async Task<IActionResult> SaveSatatus([FromBody] SaveStatusRequestModel saveStatusRequestModel)
        {
            var response = await this.externalIntegrationService.SaveApiTransaction(saveStatusRequestModel);
            return this.Ok(response);
        }

        /// <summary>
        /// Method to manual hire employee.
        /// </summary>
        /// <param name="model">ManualHireRequestModel.</param>
        /// <returns>Action result.</returns>
        [HttpPost("employee/manualHire")]
        [UvpAuthorize(PermissionCode.IntegrationManualHireEmployee)]
        [TypeFilter(typeof(RelatedActionsActivityLogFilter))]
        public async Task<IActionResult> ManualHire([FromBody] ManualHireRequestModel model)
        {
            this.logger.LogDebug($"Manual hire employee");
            var response = await this.externalIntegrationService.ManualHire(model);
            return this.Ok(response);
        }

        /// <summary>
        /// Method to manual hire employee.
        /// </summary>
        /// <param name="model">ManualHireRequestModel.</param>
        /// <returns>Action result.</returns>
        [HttpPost("employee/manualUpdate")]
        [UvpAuthorize(PermissionCode.IntegrationManualUpdate)]
        [TypeFilter(typeof(RelatedActionsActivityLogFilter))]
        public async Task<IActionResult> ManualUpdate([FromBody] UpdateErpNumberRequestModel model)
        {
            this.logger.LogDebug($"Manual hire employee");
            var response = await this.externalIntegrationService.ManualUpdate(new ManualHireRequestModel
            {
                UserId = this.ApiUser.UserId,
                AssignmentNumber = model.ErpAssignmentNumber,
                ReasonCode = model.ChangeReasonCode,
                ReasonTableCode = nameof(MasterTableEnum.ReasonContent),
                DoaCandidateId = model.DoaCandidateId,
            });
            return this.Ok(response);
        }

        /// <summary>
        /// Method to update assignment status.
        /// </summary>
        /// <param name="doaCandidateId">Candidate Id.</param>
        /// <returns>Action result.</returns>
        [HttpPost("employee/updateAssignmentStatus/{doaCandidateId}")]
        [UvpAuthorize(PermissionCode.IntegrationUpdateAssignmentStatus)]
        public async Task<IActionResult> UpdateAssignmentStatus(long doaCandidateId)
        {
            this.logger.LogDebug($"Update assignment status");
            var response = await this.externalIntegrationService.UpdateAssignmentStatus(doaCandidateId);
            return this.Ok(response);
        }
    }
}
