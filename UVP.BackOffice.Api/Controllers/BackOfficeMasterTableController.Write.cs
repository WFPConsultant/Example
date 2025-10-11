namespace UVP.BackOffice.Api.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Koa.Platform.ValueWrapper;
    using Microsoft.AspNetCore.Mvc;
    using UVP.BackOffice.Business.DataBase.Sql.Repository;
    using UVP.Shared.Micro.Api.Controllers;
    using UVP.Shared.Micro.Api.Extensions;
    using UVP.Shared.Micro.Api.UvpPermission;
    using UVP.Shared.Model.MasterTables;
    using UVP.Shared.Model.Profile;

    /// <summary>
    /// BackOffice controller with the backoffice admnin actions relateds with mastertables.
    /// </summary>
    [Route("api/backOffice")]
    public partial class BackOfficeMasterTableController : ApiController
    {
        private readonly IBackOfficeMasterTablesRepository masterTablesRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackOfficeMasterTableController"/> class.
        /// Default constructor.
        /// </summary>
        /// <param name="masterTablesRepository">masterTablesRepository.</param>
        public BackOfficeMasterTableController(IBackOfficeMasterTablesRepository masterTablesRepository)
        {
            this.masterTablesRepository = masterTablesRepository;
        }

        /// <summary>
        /// Post to create Tags on the back office.
        /// </summary>
        /// <param name="model">Tag creation Model.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Id from the tag created.</returns>
        [HttpPost("tags")]
        public async Task<IActionResult> Post([FromBody]TagModel model, CancellationToken cancellationToken) => await ValueWrapper<TagModel>
           .FromValue(model)
           .Validate()
           .DoAsync(async _ => await this.masterTablesRepository.CreateAsync(model))
           .ToActionResultAsync(true);

        /// <summary>
        /// Put to update an already existing Tag.
        /// </summary>
        /// <param name="model">Tag Update Model.</param>
        /// <param name="tagCode">TagCode from the Tag to update.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Id from the tag Updated.</returns>
        [HttpPut("tags/{tagCode}")]
        public async Task<IActionResult> Put([FromBody]TagUpdateModel model, string tagCode, CancellationToken cancellationToken) => await ValueWrapper<TagUpdateModel>
            .FromValue(model)
            .Validate()
            .DoAsync(async _ => await this.masterTablesRepository.UpdateAsync(tagCode, model))
            .ToActionResultAsync(true);

        /// <summary>
        /// Delete  to remove an already existing Tag.
        /// </summary>
        /// <param name="tagCode">Tag code from the tag to delete.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>Task representing the action to perform.</returns>
        [HttpDelete("tags/{tagCode}")]
        public async Task<IActionResult> Delete(string tagCode, CancellationToken cancellationToken) => await ValueWrapper<string>
            .FromValue(tagCode)
            .DoAsync(async _ => await this.masterTablesRepository.DeleteAsync(tagCode))
            .ToActionResultAsync(true);

        [HttpPut("masterTables/{masterCode}/values/{valueCode}")]
        [UvpAuthorize(PermissionCode.ManageMasterTableValues)]
        public async Task<IActionResult> UpdateMasterTableValue([FromBody] MasterTableValueModel model, string masterCode, string valueCode, CancellationToken cancellationToken) => await ValueWrapper<MasterTableValueModel>
            .FromValue(model)
            .Validate()
             .DoAsync(async _ =>
             {
                 return await this.masterTablesRepository.UpdateMasterTableValue(masterCode, valueCode, model);
             })
            .ToActionResultAsync(true);
    }
}
