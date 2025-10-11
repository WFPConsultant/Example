namespace UVP.MasterTables.Api.Controllers.Mla
{
    using System.Threading;
    using System.Threading.Tasks;
    using Koa.Platform.ValueWrapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using UVP.Shared.Micro.Api.Extensions;
    using UVP.Shared.Model.MasterTables;

    /// <summary>
    /// Controller for mla Micro-service that implements write methods.
    /// </summary>
    public partial class MlaController
    {
        /// <summary>
        /// Gets a MLA giving its Id.
        /// </summary>
        /// <param name="id">MLA id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(long id, CancellationToken cancellationToken)
        {
            this.logger.LogDebug($"Get:: it gets a MLA from the DB with the following Id {id}");
            return await ValueWrapper<long>
                .FromValue(id)
                .DoAsync(async _ => await this.repository.FindOneAsync<MlaModel>(id, cancellationToken))
                .ToActionResultAsync(true);
        }

        /// <summary>
        /// Gets a MLA from the DB giving duty station and category parameters.
        /// </summary>
        /// <param name="countryCode">DutyStation Country code.</param>
        /// <param name="step">DutyStation step.</param>
        /// <param name="salaryPlanId">Category salary plan.</param>
        /// <param name="gradeId">Category grade.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [AllowAnonymous]
        [HttpGet("country/{countryCode}/step/{step}/salaryPlan/{salaryPlanId}/grade/{gradeId}")]
        public async Task<IActionResult> GetMlaByDutyStationAndCategoryAsync(string countryCode, byte step, long salaryPlanId, long gradeId)
        {
            this.logger.LogDebug($"Get:: it gets a MLA from the DB giving duty station and category parameters");
            return await ValueWrapper<long>
                .FromValue(salaryPlanId)
                .DoAsync(async _ => await this.repository.GetMlaByDutyStationAndCategory(countryCode, step, salaryPlanId, gradeId))
                .ToActionResultAsync(false);
        }
    }
}
