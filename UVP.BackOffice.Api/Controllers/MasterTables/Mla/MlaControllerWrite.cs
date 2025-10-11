namespace UVP.MasterTables.Api.Controllers.Mla
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using UVP.MasterTables.Business.DataBase.Sql;
    using UVP.Shared.Micro.Api.Controllers;

    /// <summary>
    /// Controller for mla Micro-service that implements write methods.
    /// </summary>
    [Route("api/mla")]
    public partial class MlaController : ApiController
    {
        private readonly IMlaRepository repository;
        private readonly ILogger<MlaController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MlaController"/> class.
        /// </summary>
        /// <param name="repository">MlaRepository.</param>
        /// <param name="logger">Logger.</param>
        public MlaController(IMlaRepository repository, ILogger<MlaController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }
    }
}
