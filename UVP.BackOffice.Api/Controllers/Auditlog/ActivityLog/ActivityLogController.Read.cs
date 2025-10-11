namespace UVP.AuditLog.Api.Controllers.ActivityLog
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Koa.Domain.Search.Page;
    using Koa.Platform.ValueWrapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using UVP.AuditLog.Business.Handlers.Query.Contracts;
    using UVP.AuditLog.Business.Handlers.Query.Models;
    using UVP.Shared.Micro.Api.Controllers;
    using UVP.Shared.Micro.Api.Extensions;
    using UVP.Shared.Micro.Api.Translate.Attribute;
    using UVP.Shared.Micro.Api.UvpPermission;
    using UVP.Shared.Model.AuditLog;
    using UVP.Shared.Model.Profile;

    /// <summary>
    /// Controller for Batch Micro-service that implements read methods.
    /// </summary>
    [Microsoft.AspNetCore.Mvc.Route("api/activitylog")]
    public partial class ActivityLogController : ApiController
    {
        private readonly ILogger<ActivityLogController> logger;
        private readonly IActivityLogQueryHandler activityLogQueryHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLogController"/> class.
        /// </summary>
        /// <param name="logger">ILogger.</param>
        /// <param name="documentRepository">ICoaDocumentRepository.</param>
        /// <param name="userHttp">user http.</param>
        /// <param name="activityLogQueryHandler">ActivityLogQueryHandler.</param>
        public ActivityLogController(ILogger<ActivityLogController> logger, IActivityLogQueryHandler activityLogQueryHandler)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.activityLogQueryHandler = activityLogQueryHandler;
        }

        /// <summary>
        /// Method to search for ActivityLog items on the DB with the filters provided in the PageDescriptor object. This method does not use cache also the user should be logged.
        /// </summary>
        /// <param name="pagedModel">PageDescriptor.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>Action result.</returns>
        [TypeFilter(typeof(TranslatableAsyncAttribute<ActivityLogInfoModel>))]
        [HttpPost("search")]
        [UvpAuthorize(PermissionCode.ActivityViewLog)]
        public async Task<IActionResult> SearchAsync([FromBody] PageDescriptor pagedModel, CancellationToken cancellationToken)
        {
            this.logger.LogDebug($"GetAll:: it gets all the ActivityLog items from the DB according to the specified PageDescriptor");
            var model = new ActivityLogQueryModel() { PageDescriptor = pagedModel, CancellationToken = cancellationToken };
            return await ValueWrapper<PageDescriptor>
                                   .FromValue(pagedModel)
                                   .Validate()
                                   .DoAsync(async _ => await this.activityLogQueryHandler.Handle(model))
                                   .ToActionResultAsync(true);
        }

        /// <summary>
        /// Method to search for log by entity.
        /// </summary>
        /// <param name="entity">Entity name, ex Doa.</param>
        /// <param name="id">Entity Id.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>Action result.</returns>
        [HttpPost("{entity}/{id}")]
        [UvpAuthorize(PermissionCode.CanViewEntityLogs)]
        public async Task<IActionResult> GetLogAsync([FromBody] PageDescriptor pagedModel, string entity, long id, CancellationToken cancellationToken)
        {
            this.logger.LogDebug($"GetLogAsync:: it gets logs from DB according to the specified entity and id.");
            return await ValueWrapper<PageDescriptor>
                                .FromValue(pagedModel)
                                .Validate()
                                .DoAsync(async _ => await this.activityLogQueryHandler.GetLogAsync(pagedModel, entity, id, cancellationToken))
                                 .ToActionResultAsync(true);
        }

        /// <summary>
        /// get all copmted task and futur tasks by doacandidateId.
        /// </summary>

        /// <param name="id">doaCandidateId.</param>
        /// <param name="entity">entity.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>Action result.</returns>
        [HttpGet("progressTrackerTasks/{doaCandidateId}")]
        [UvpAuthorize(PermissionCode.CanViewProgressTracker)]
        public async Task<IActionResult> GetProgressTrackerTasks(long doaCandidateId, CancellationToken cancellationToken)
        {
            this.logger.LogDebug($"GetLogAsync:: it gets logs from DB according to the specified entity and id.");
            return await ValueWrapper<long>
                        .FromValue(doaCandidateId) 
                                .DoAsync(async _ => await this.activityLogQueryHandler.GetProgressTrackerTasks(doaCandidateId, cancellationToken))
                                 .ToActionResultAsync(true);
        }

        /// <summary>
        /// Method to get completed tasks from activityLog table.
        /// </summary>
        /// <param name="completedUserTask">page model.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>Action result.</returns>
        [HttpPost("tasks/completed")]
        public async Task<IActionResult> GetCompletedTasksAsync([FromBody] CompletedUserTasksFilter completedUserTask, CancellationToken cancellationToken)
        {
            this.logger.LogDebug($"GetCompletedTasksAsync:: get completed tasks from activityLog table.");
            return await ValueWrapper<CompletedUserTasksFilter>
                                .FromValue(completedUserTask)
                                .Validate()
                                .DoAsync(async _ => await this.activityLogQueryHandler.GetCompltedTasksAsync(completedUserTask, cancellationToken))
                                .ToActionResultAsync(true);
        }

        /// <param name="id">doaCandidateId.</param>
        /// <param name="entity">entity.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>Action result.</returns>
        [HttpGet("tasksForVra/{doaCandidateId}")]
        [UvpAuthorize(PermissionCode.CanViewVraDetails)]
        public async Task<IActionResult> GetVRATasks(long doaCandidateId, CancellationToken cancellationToken)
        {
            this.logger.LogDebug($"GetLogAsync:: it gets logs from DB according to the specified entity and id.");
            return await ValueWrapper<long>
                        .FromValue(doaCandidateId)
                                .DoAsync(async _ => await this.activityLogQueryHandler.GetVRATasks(doaCandidateId, cancellationToken))
                                 .ToActionResultAsync(true);
        }
    }
}
