namespace UVP.BackOffice.Api.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Koa.Platform.ValueWrapper;
    using Microsoft.AspNetCore.Mvc;
    using UVP.Shared.Micro.Api.Extensions;
    using UVP.Shared.Micro.Api.UvpPermission;
    using UVP.Shared.Model.Profile;

    /// <summary>
    /// BackOffice controller with the backoffice admnin actions relateds with mastertables.
    /// </summary>

    public partial class BackOfficeMasterTableController
    {
        /// <summary>
        /// Gets the master table values.
        /// </summary>
        /// <param name="masterCode">The mastertable code.</param>
        /// <returns>The value list/returns>.
        [HttpPost("masterTables/{masterCode}/values")]
        [UvpAuthorize(PermissionCode.ManageMasterTableValues)]
        public async Task<IActionResult> GetMasterTableValues([FromBody] PageDescriptor2 pagedModel, string masterCode, CancellationToken cancellationToken) => await ValueWrapper<PageDescriptor2>
                .FromValue(pagedModel)
                .Validate()
                .DoAsync(async _ => await this.masterTablesRepository.GetMasterTableValues(masterCode, pagedModel))
                .ToActionResultAsync(true);

        /// <summary>
        /// Gets the master table values.
        /// </summary>
        /// <param name="masterCode">The mastertable code.</param>
        /// <returns>The value list/returns>.
        [HttpPost("masterTables")]
        [UvpAuthorize(PermissionCode.ManageMasterTableValues)]
        public async Task<IActionResult> GetMasterTables([FromBody] PageDescriptor2 pagedModel, CancellationToken cancellationToken) => await ValueWrapper<PageDescriptor2>
                .FromValue(pagedModel)
                .Validate()
                .DoAsync(async _ => await this.masterTablesRepository.GetMasterTables(pagedModel))
                .ToActionResultAsync(true);
    }
}
