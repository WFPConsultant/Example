namespace UVP.MasterTables.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Business.DataBase.Sql;
    using UVP.Shared.Micro.Api.Controllers;

    /// <summary>
    /// Controller for MasterData write methods.
    /// </summary>
    [Route("api/masterData")]
    public partial class MasterDataController : ApiController
    {
        private readonly ILogger<MasterDataController> logger;
        private readonly IMasterDataRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterDataController"/> class.
        /// </summary>
        /// <param name="logger">ILogger.</param>
        /// <param name="repository">IMasterDataRepository.</param>
        public MasterDataController(ILogger<MasterDataController> logger, IMasterDataRepository repository)
        {
            this.logger = logger;
            this.repository = repository;
        }
    }
}
