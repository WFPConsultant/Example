namespace UVP.MasterTables.Api.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Koa.Platform.ValueWrapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using UVP.Shared.Micro.Api.Extensions;
    using UVP.Shared.Model.MasterTables;

    /// <summary>
    /// Controller for MasterData read methods.
    /// </summary>
    public partial class MasterDataController
    {
        /// <summary>
        /// Gets a MasterData from the DB.
        /// </summary>
        /// <param name="id">MasterData id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(long id, CancellationToken cancellationToken)
        {
            this.logger.LogDebug($"Get:: it gets a MasterData from the DB with the following Id {id}");
            return await ValueWrapper<long>
                .FromValue(id)
                .DoAsync(async _ => await this.repository.FindOneAsync<MasterDataModel>(id, cancellationToken))
                .ToActionResultAsync(true);
        }

        /// <summary>
        /// Gets a MasterData from the DB by its parent.
        /// </summary>
        /// <param name="parentTableCode">Parent Table Code.</param>
        /// <param name="parentCode">Cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("parent/{parentTableCode}/{parentCode}")]
        public async Task<IActionResult> GetAsync(string parentTableCode, string parentCode)
        {
            this.logger.LogDebug($"Get:: it gets a MasterData from the DB given its parent code and tableCode");
            return await ValueWrapper<string>
                .FromValue(parentTableCode)
                .DoAsync(async _ => await this.repository.GetMasterDataByParent(parentTableCode, parentCode))
                .ToActionResultAsync(true);
        }
    }
}
