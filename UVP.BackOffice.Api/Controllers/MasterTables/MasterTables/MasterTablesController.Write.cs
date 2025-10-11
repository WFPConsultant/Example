namespace UVP.MasterTables.Api.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Koa.Platform.ValueWrapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using UVP.MasterTables.Business.DataBase.Sql.Repository;
    using UVP.MasterTables.Business.Query;
    using UVP.MasterTables.Business.Query.Handlers;
    using UVP.Shared.Micro.Api.Configuration.DistributedCache;
    using UVP.Shared.Micro.Api.Controllers;
    using UVP.Shared.Micro.Api.Extensions;
    using UVP.Shared.Model.Doa.Doa;
    using UVP.Shared.Model.MasterTables;

    [Route("api/masterTables")]
    public partial class MasterTablesController : ApiController
    {
        private readonly ILogger<MasterTablesController> logger;
        private readonly MTQueryHandler queryHandler;
        private readonly MTLocalizedValueQueryHandler localizedValueQueryHandler;
        private readonly MTAutoCompleteQueryHandler autoCompleteQueryHandler;
        private readonly MTValueQueryHandler mTValueQueryHandler;
        private readonly ITagRepository tagRepository;
        private readonly IDistributedCache distributedCache;
        private readonly ChartOfAccountHandler chartOfAccountHandler;
        private readonly CategoryGroupHandler categoryGroupHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterTablesController"/> class.
        /// Default constructor.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="queryHandler">Query handler to handle query commands.</param>
        /// <param name="tagRepository">Tag repository.</param>
        /// <param name="localizedValueQueryHandler">localized value query handler.</param>
        /// <param name="autoCompleteQueryHandler">auto complete query handler.</param>
        public MasterTablesController(
            ILogger<MasterTablesController> logger,
            MTQueryHandler queryHandler,
            ITagRepository tagRepository,
            MTLocalizedValueQueryHandler localizedValueQueryHandler,
            MTValueQueryHandler mTValueQueryHandler,
            MTAutoCompleteQueryHandler autoCompleteQueryHandler,
            IDistributedCache distributedCache,
            ChartOfAccountHandler chartOfAccountHandler,
            CategoryGroupHandler categoryGroupHandler)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.queryHandler = queryHandler;
            this.tagRepository = tagRepository;
            this.localizedValueQueryHandler = localizedValueQueryHandler;
            this.mTValueQueryHandler = mTValueQueryHandler;
            this.autoCompleteQueryHandler = autoCompleteQueryHandler;
            this.distributedCache = distributedCache;
            this.chartOfAccountHandler = chartOfAccountHandler;
            this.categoryGroupHandler = categoryGroupHandler;
        }

        [HttpPost("tags")]
        public async Task<IActionResult> Post([FromBody] TagModel model, CancellationToken cancellationToken) => await ValueWrapper<TagModel>
           .FromValue(model)
           .Validate()
           .DoAsync(async _ =>
           {
               await this.CleanDistributedAndMemoryCache();
               return await this.tagRepository.CreateAsync(model);
           })
           .ToActionResultAsync(true);

        [HttpPut("tags/{tagCode}")]
        public async Task<IActionResult> Put([FromBody] TagUpdateModel model, string tagCode, CancellationToken cancellationToken) => await ValueWrapper<TagUpdateModel>
            .FromValue(model)
            .Validate()
             .DoAsync(async _ =>
             {
                 await this.CleanDistributedAndMemoryCache();
                 return await this.tagRepository.UpdateAsync(tagCode, model);
             })
            .ToActionResultAsync(true);

        [HttpDelete("tags/{tagCode}")]
        public async Task<IActionResult> Delete(string tagCode, CancellationToken cancellationToken) => await ValueWrapper<string>
            .FromValue(tagCode)
             .DoAsync(async _ =>
             {
                 await this.CleanDistributedAndMemoryCache();
                 return await this.tagRepository.DeleteAsync(tagCode);
             })
            .ToActionResultAsync(true);

        [HttpPost("getcoacombos")]
        public async Task<IActionResult> GetCoaCombos([FromBody] GetFundingOptionsModel model, CancellationToken cancellationToken) =>
            await ValueWrapper<GetFundingOptionsModel>
           .FromValue(model)
           .Validate()
           .DoAsync(async _ => await this.chartOfAccountHandler.HandleAsync(new GetNextFieldCoaCommand
           {
               Filters = model,
               HeType = model.HeType,
               FieldAsked = (CoaFieldEnum)Enum.Parse(typeof(CoaFieldEnum), model.FieldAsked.FirstCharacterToUpper()),
               CountryCode = model.CountryCode,
               InstitutionCode = model.InstitutionCode,
               Query = model.Query,
           }))
           .ToActionResultAsync(true);

        [HttpPut("{masterCode}/values/{valueCode}")]
        public async Task<IActionResult> UpdateValue([FromBody] MasterTableValueModel model, string masterCode, string valueCode, CancellationToken cancellationToken) => await ValueWrapper<MasterTableValueModel>
            .FromValue(model)
            .Validate()
             .DoAsync(async _ =>
             {
                 await this.distributedCache.PublishAsync("refresh-cache", Guid.NewGuid().ToString());
                 return await this.mTValueQueryHandler.UpdateAsync(new GetMasterTableQuery(masterCode, null, valueCode), model);

             })
            .ToActionResultAsync(true);

        private async Task CleanDistributedAndMemoryCache()
        {
            await this.distributedCache.DeleteObjectAsync($"*tags/*");
            await this.distributedCache.DeleteObjectAsync($"*TagCandidate*");
            await this.distributedCache.DeleteObjectAsync($"*TagDoa*");
            await this.distributedCache.PublishAsync("refresh-cache", Guid.NewGuid().ToString());
        }
    }
}
