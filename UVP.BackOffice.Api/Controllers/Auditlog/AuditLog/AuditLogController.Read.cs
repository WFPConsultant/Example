namespace UVP.AuditLog.Api.Controllers.AuditLog
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Koa.Platform.ValueWrapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Business.DataBase.Sql;
    using UVP.Shared.Http.User;
    using UVP.Shared.Micro.Api.Controllers;
    using UVP.Shared.Micro.Api.Extensions;
    using UVP.Shared.Micro.Api.UvpPermission;
    using UVP.Shared.Model.Profile;
    using UVP.Shared.Model.Shared;

    /// <summary>
    /// Controller for Batch Micro-service that implements read methods. 
    /// </summary>
    [Microsoft.AspNetCore.Mvc.Route("api/auditLog")]
    public class AuditLogController : ApiController
    {
        private readonly ILogger<AuditLogController> logger;
        private readonly IAuditLogRepository repository;
        private readonly IUserAccessService userHttp;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogController"/> class.
        /// </summary>
        /// <param name="logger">ILogger.</param>
        /// <param name="documentRepository">ICoaDocumentRepository.</param>
        /// <param name="userHttp">user http.</param>
        public AuditLogController(ILogger<AuditLogController> logger, IAuditLogRepository repository, IUserAccessService userHttp)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository;
            this.userHttp = userHttp;
        }

        /// <summary>
        /// Method to search for AuditLogEntry documents on the DB filtered by the provided entity name.
        /// </summary>
        /// <param name="entity">Entity name, ex Doa.</param>
        /// <param name="id">Entity Id.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>Action result.</returns>
        [HttpGet("entity/{entity}/{id}")]
        [UvpAuthorize(PermissionCode.DoaViewLog)]
        public async Task<IActionResult> SearchByEntityAsync(string entity, long id, CancellationToken cancellationToken)
        {
            this.logger.LogDebug($"SearchByEntitytAsync:: it gets all the logs from DB according to the specified entity and id.");
            return await ValueWrapper<long>
                                .FromValue(id)
                                .DoAsync(async _ => await this.repository.SearchByEntityAsync(entity, id, cancellationToken))
                                .ToActionResultAsync(true);
        }

        /// <summary>
        /// Method to search for AuditLogEntry documents on the DB filtered by the provided entity name.
        /// </summary>
        /// <param name="id">Entity Id.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>Action result.</returns>
        [HttpGet("bankinginformation/{id}")]
        [UvpAuthorize(PermissionCode.BankingInformationViewLog)]
        public async Task<IActionResult> SearchBankingInformationLogAsync(long id, CancellationToken cancellationToken)
        {
            this.logger.LogDebug($"SearchBankingInformationLogAsync:: it gets all the logs from DB for banking information according to the specified id.");
            return await ValueWrapper<long>
                    .FromValue(id)
                    .DoAsync(async _ =>
                    {
                        if (RoleTypeHelper.IsAVolunteerRole(this.ApiUser.RoleId) && !(await this.userHttp.CheckBankingInformationBelongsToCandidate(id)))
                        {
                            throw new System.Exception("You cannot see the banking information from this user.");
                        }

                        return await this.repository.SearchByEntityAsync("BankingInformation", id, cancellationToken);
                    })
                    .ToActionResultAsync(true);
        }

    }
}
