namespace UVP.BackOffice.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Koa.Platform.ValueWrapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using UVP.Shared.Micro.Api.Configuration.DistributedCache;
    using UVP.Shared.Micro.Api.Controllers;
    using UVP.Shared.Micro.Api.Extensions;
    using UVP.Shared.Micro.Api.UvpPermission;
    using UVP.Shared.Model.Profile;

    /// <summary>
    /// The Back office management.
    /// </summary>
    [Route("api/management")]
    public class BackOfficeManagement : ApiController
    {
        private readonly ILogger<BackOfficeManagement> logger;
        private readonly IDistributedCache distributedCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackOfficeManagement"/> class.
        /// </summary>
        /// <param name="backOfficeManagement"> the backOfficeManagement interface.</param>
        /// <param name="logger"> the logger interface.</param>
        public BackOfficeManagement(ILogger<BackOfficeManagement> logger, IDistributedCache distributedCache)
        {            
            this.logger = logger;
            this.distributedCache = distributedCache;
        }

        /// <summary>
        ///  Clear both distritubed and memory cache.
        /// </summary>
        /// <returns>True if all caches have been executed, otherwise false.</returns>
        [HttpPost("cache/clear/all")]
        [UvpAuthorize(PermissionCode.ClearCache)]
        public async Task<IActionResult> ClearAllCaches() => await ValueWrapper<bool>
            .FromValue(true)
            .DoAsync(async _ =>
            {
                this.logger.LogInformation("ClearAllCaches request.");
                var distributedCacheId = await this.distributedCache.PublishAsync("refresh-cache", Guid.NewGuid().ToString());
                this.logger.LogInformation($"The user {this.ApiUser.UserId} calls clear distrivuted cache method with Id: {distributedCacheId}");
                return true;
            })
            .ToActionResultAsync(true);

    }
}
