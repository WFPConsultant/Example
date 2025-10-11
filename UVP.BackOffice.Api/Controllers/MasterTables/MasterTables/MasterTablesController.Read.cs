namespace UVP.MasterTables.Api.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Koa.Persistence.Abstractions.QueryResult;
    using Koa.Platform.ValueWrapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using UVP.MasterTables.Business.Model;
    using UVP.MasterTables.Business.Query;
    using UVP.Shared.Micro.Api.Cache.DistributedCache;
    using UVP.Shared.Micro.Api.Extensions;
    using UVP.Shared.Model.MasterTables;
    using ResponseCacheAttribute = UVP.Shared.Micro.Api.Cache.DistributedCache.ResponseCacheAttribute;

    /// <summary>
    /// Master Tables controller, with the get crud actions.
    /// </summary>
    public partial class MasterTablesController
    {
        /// <summary>
        /// Gets the master table values filtered by language.
        /// </summary>
        /// <param name="masterCode">The mastertable Id.</param>
        /// <returns>The value list/returns>.
        [AllowAnonymous]
        [ResponseMemoryCache("MasterTable_group")]
        [ResponseCache]
        [HttpGet("{masterCode}")]
        public async Task<MasterTableModel> GetMasterTable(string masterCode) => await this.queryHandler.HandleAsync(new GetMasterTableQuery(masterCode));

        /// <summary>
        /// Gets the master table values filtered by language.
        /// </summary>
        /// <param name="masterCode">The mastertable Id.</param>
        /// <param name="language">Language value which you need.</param>
        /// <returns>The value list.</returns>
        [AllowAnonymous]
        [ResponseMemoryCache]
        [ResponseCache]
        [HttpGet("{masterCode}/languages/{language}")]
        public async Task<MasterTableModel> GetMasterTableByLang(string masterCode, string language)
        {
            return await this.queryHandler.HandleAsync(new GetMasterTableQuery(masterCode, language));
        }
        
        [AllowAnonymous]
        [ResponseMemoryCache]
        [ResponseCache]
        [HttpGet("{masterCode}/languages/{language}/{countryCode}/{institutionCode}")]
        public async Task<MasterTableModel> GetMasterTableValueByLanguageAndCountry(string masterCode, string valueCode, string language, string countryCode, string institutionCode)
        {
            return await this.queryHandler.HandleAsync(new GetMasterTableQuery  
                
                (masterCode, language, active: false, valueCode: valueCode, countryCode: countryCode, institutionCode: institutionCode));
        }

        /// <summary>
        /// Gets the master table values filtered by language.
        /// </summary>
        /// <param name="masterCode">The mastertable Id.</param>
        /// <param name="language">Language value which you need.</param>
        /// <param name="active">Flag to get the active values or not.</param>
        /// <returns>The value list/returns>.
        [AllowAnonymous]
        [ResponseMemoryCache("MasterTable_group")]
        [ResponseCache]
        [HttpGet("{masterCode}/languages/{language}/{active}")]
        public async Task<MasterTableModel> GetMasterTableByLang(string masterCode, string language, bool active)
        {
            return await this.queryHandler.HandleAsync(new GetMasterTableQuery(masterCode, language, active: active)
            {
                ShowAll = true
            });
        }

        /// <summary>
        /// Gets the master table values filtered by language as minimal model.
        /// </summary>
        /// <param name="masterCode">The mastertable Id.</param>
        /// <param name="language">Language value which you need.</param>
        /// <returns>The value list/returns>.
        [AllowAnonymous]
        [ResponseMemoryCache]
        [ResponseCache]
        [HttpGet("{masterCode}/minimal/languages/{language}")]
        public async Task<MasterTableModel> GetMasterTableminimalByLang(string masterCode, string language) => await this.queryHandler.HandleAsync(new GetMasterTableQuery(masterCode, language, minimalModel: true));

        /// <summary>
        /// Gets the master table values filtered by language as minimal model.
        /// </summary>
        /// <param name="masterCode">The mastertable Id.</param>
        /// <param name="language">Language value which you need.</param>
        /// <param name="active">Flag to get the active values or not.</param>
        /// <returns>The value list/returns>.
        [AllowAnonymous]
        [ResponseMemoryCache]
        [ResponseCache]
        [HttpGet("{masterCode}/minimal/languages/{language}/{active}")]
        public async Task<MasterTableModel> GetMasterTableminimalByLang(string masterCode, string language, bool active) => await this.queryHandler.HandleAsync(new GetMasterTableQuery(masterCode, language, minimalModel: true, active: active));

        /// <summary>
        /// Gets the minimal value by masterTable code and value code and language.
        /// </summary>
        /// <param name="masterCode">The mastertable Id.</param>
        /// <param name="valueCode">The code field from the property.</param>
        /// <param name="language">Language value which you need.</param>
        /// <returns>The value in the language requested/returns>.
        [AllowAnonymous]
        [ResponseMemoryCache]
        [ResponseCache]
        [HttpGet("{masterCode}/values/{valueCode}/minimal/languages/{language}")]
        public async Task<MasterTableValueResultModel> GetMasterTableValueByLanguageMiniModel(string masterCode, string valueCode, string language)
        {
            var masterTable = await this.queryHandler.HandleAsync(new GetMasterTableQuery(masterCode, language, minimalModel: true, active: false));
            return masterTable.GetValue(valueCode);
        }

        /// <summary>
        /// Gets the value by masterTable code and value code and language.
        /// </summary>
        /// <param name="masterCode">The mastertable Id.</param>
        /// <param name="valueCode">The code field from the property.</param>
        /// <param name="language">Language value which you need.</param>
        /// <returns>The value in the language requested/returns>.
        [AllowAnonymous]
        [HttpGet("{masterCode}/values/{valueCode}/languages/{language}")]
        public async Task<MasterTableValueResultModel> GetMasterTableValueByLanguage(string masterCode, string valueCode, string language)
        {
            var masterTable = await this.queryHandler.HandleAsync(new GetMasterTableQuery(masterCode, language, active: false, valueCode: valueCode)
            {
                ShowAll = true
            });
            var res = masterTable.GetValue(valueCode);
            return res;
        }

        /// <summary>
        /// Gets the children of a specic mastertable value by mastercode and code and the type by of the children by childrenmastercode and language.
        /// </summary>
        /// <param name="masterCode">The mastertable Id.</param>
        /// <param name="language">Language value which you need.</param>
        /// <param name="valueCode">Gets or sets, the code from the value to retrieve.</param>
        /// <param name="childrenMasterCode">Gets or sets, the code from the child mastertable code to retrieve.</param>
        /// <returns>The value in the language requested/returns>.
        [AllowAnonymous]
        [ResponseMemoryCache]
        [ResponseCache]
        [HttpGet("{masterCode}/values/{valueCode}/children/{childrenMasterCode}/languages/{language}")]
        public async Task<MasterTableModel> GetFilteredChildrenMasterTableByLanguage(string masterCode, string language, string valueCode, string childrenMasterCode) => await this.queryHandler.HandleAsync(new GetMasterTableQuery(masterCode, language, valueCode, childrenMasterCode));

        /// <summary>
        /// Gets the value by masterTable code and value code.
        /// </summary>
        /// <param name="masterCode">The mastertable Id.</param>
        /// <param name="valueCode">The code field from the property.</param>
        /// <returns>The value in the language requested/returns>.
        [AllowAnonymous]
        [ResponseMemoryCache]
        [HttpGet("{masterCode}/values/{valueCode}")]
        public async Task<ValueLocalizedValuesModel> GetMasterTableValueLocalizedValues(string masterCode, string valueCode) => await this.localizedValueQueryHandler.HandleAsync(new GetMasterTableWithoutLanguageQuery(masterCode, valueCode));

        /// <summary>
        /// Gets the value by masterTable code and value code and language.
        /// </summary>
        /// <param name="masterCode">The mastertable Id.</param>
        /// <param name="language">Language value which you need.</param>
        /// <param name="partialValue">The approximate value field from the property.</param>
        /// <returns>The value in the language requested/returns>.
        [AllowAnonymous]
        [ResponseMemoryCache]
        [ResponseCache]
        [HttpGet("{masterCode}/languages/{language}/localizedValue/{partialValue}")]
        public async Task<MasterTableModel> GetMasterTableLocalizedValueByLanguageAutoComplete(string masterCode, string language, string partialValue) => await this.autoCompleteQueryHandler.HandleAsync(new GetMasterTableQuery(masterCode, language, null, null, partialValue));

        /// <summary>
        /// Method to search for templates on the DB with the filters provided in the PageDescriptor object.
        /// </summary>
        /// <param name="pagedModel">PageDescriptor.</param>
        /// <param name="type">MT type.</param>
        /// <param name="languageCode">Language code.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns>Action result.</returns>
        [AllowAnonymous]
        [ResponseCache]
        [HttpPost("tags/{type}/{languageCode}/search")]
        public async Task<IActionResult> SearchTagsAsync([FromBody] PageDescriptor2 pagedModel, string type, string languageCode, CancellationToken cancellationToken)
        {
            this.logger.LogDebug($"Search:: it gets all the tags for {type} from the DB according to the specified PageDescriptor");
            return await ValueWrapper<PageDescriptor2>
                        .FromValue(pagedModel)
                        .Validate()
                        .DoCacheAsync<PageDescriptor2, PagedQueryResult<MasterTableValueResultModel>>(
                                            $"tags/{type}/{languageCode}/search?{pagedModel}",
                                            60,
                                            this.distributedCache,
                                            async _ => await this.tagRepository.GetAllAsync(pagedModel, type, languageCode, cancellationToken))
                        .ToActionResultAsync(true);
        }

        /// <summary>
        /// Gets Chart of Account by entityType and institution.
        /// </summary>
        /// <param name="entityType">entityType.</param>
        /// <param name="institutionCode">institution code.</param>
        /// <returns>Action result.</returns>
        [HttpGet("chartOfAccounts/entityType/{entityType}/institution/{institutionCode}")]
        [ResponseMemoryCache]
        [ResponseCache]
        public async Task<IActionResult> GetChartOfAccountAsync(string entityType, string institutionCode)
        {
            this.logger.LogDebug($"GET: chart of account by entityType {entityType} and institutionCode {institutionCode}");
            return await ValueWrapper<string>
                        .FromValue(entityType)
                        .DoAsync(async _ => await this.chartOfAccountHandler.HandleAsync(new GetChartOfAccountCommand
                        {
                            EntityType = entityType,
                            InstitutionCode = institutionCode,
                        }))
                        .ToActionResultAsync(true);
        }

        /// <summary>
        /// Gets All data of CategoryGroup by masterTableId.
        /// </summary>
        /// <param name="masterTableId">entityType.</param>
        /// <returns>Action result.</returns>
        [HttpGet("categoryGroup/masterTableId/{masterTableId}")]
        [ResponseMemoryCache]
        [ResponseCache]
        public async Task<IActionResult> GetCategoryGroupAsync(int masterTableId)
        {
            this.logger.LogDebug($"GET: All data of CategoryGroup");
            return await ValueWrapper<int>
                        .FromValue(masterTableId)
                        .DoAsync(async _ => await this.categoryGroupHandler.HandleAsync(new GetCategoryGroupCommand
                        {
                            MasterTableId = masterTableId,
                        }))
                        .ToActionResultAsync(true);
        }

        /// <summary>
        /// Gets Chart of Account by entityType.
        /// </summary>
        /// <param name="entityType">entityType.</param>
        /// <returns>Action result.</returns>
        [HttpGet("chartOfAccounts/entityType/{entityType}")]
        [ResponseMemoryCache]
        [ResponseCache]
        public async Task<IActionResult> GetChartOfAccountAsync(string entityType)
        {
            this.logger.LogDebug($"GET: chart of account by entityType {entityType}");
            return await ValueWrapper<string>
                        .FromValue(entityType)
                        .DoAsync(async _ => await this.chartOfAccountHandler.HandleAsync(new GetChartOfAccountCommand
                        {
                            EntityType = entityType,
                        }))
                        .ToActionResultAsync(true);
        }

        /// <summary>
        /// Method to check if all the values on a list exists in mt.
        /// </summary>
        /// <param name="validateMasterTable">Model with the collection to check.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection with the values that doesn't exist.</returns>
        [HttpPost("checkvalues")]
        public async Task<IActionResult> CheckMtValues([FromBody] ValidateMasterTableModel validateMasterTable, CancellationToken cancellationToken) => await ValueWrapper<ValidateMasterTableModel>
            .FromValue(validateMasterTable)
            .DoAsync(async _ => await this.queryHandler.ValidateMTCodeAndValue(validateMasterTable.MasterTables))
            .ToActionResultAsync(false);

        /// <summary>
        /// Gets the master table values.
        /// </summary>
        /// <param name="masterCode">The mastertable code.</param>
        /// <returns>The value list/returns>.
        [HttpPost("{masterCode}/values")]
        public async Task<IActionResult> GetMasterTableValues([FromBody] PageDescriptor2 pagedModel, string masterCode, CancellationToken cancellationToken) => await ValueWrapper<PageDescriptor2>
                .FromValue(pagedModel)
                .Validate()
                .DoAsync(async _ => await this.mTValueQueryHandler.HandleAsync(new GetMasterTableQuery(masterCode), pagedModel))
                .ToActionResultAsync(true);

        /// <summary>
        /// Gets the master table values.
        /// </summary>
        /// <param name="masterCode">The mastertable code.</param>
        /// <returns>The value list/returns>.
        [HttpPost]
        public async Task<IActionResult> GetMasterTables([FromBody] PageDescriptor2 pagedModel, CancellationToken cancellationToken) => await ValueWrapper<PageDescriptor2>
                .FromValue(pagedModel)
                .Validate()
                .DoAsync(async _ => await this.mTValueQueryHandler.GetMasterTables(pagedModel))
                .ToActionResultAsync(true);


        /// <summary>
        /// Gets All data of CategoryGroup by masterTableId.
        /// </summary>
        /// <param name="model">categoryCode.</param>
        /// <returns>Action result.</returns>
        [HttpPost("categoryGroupByCategoryCode")]
        public async Task<IActionResult> GetCandidateGroupByCategoryCode(CategoryGroutRequestModel model)
        {
            this.logger.LogDebug($"GET: All data of CategoryGroup");
            return await ValueWrapper<CategoryGroutRequestModel>
                        .FromValue(model)
                        .DoAsync(async _ => await this.categoryGroupHandler.HandleAsync(new GetCategoryGroupByCategoryCode
                        {
                            CategoryCode = model.Code,
                        }))
                        .ToActionResultAsync(true);
        }
    }
}
