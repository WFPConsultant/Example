using System.Threading.Tasks;
using Koa.Domain.Search.Page;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UVP.Shared.Micro.Api.Controllers;
using UVP.Shared.Micro.Domain.Services.Contracts.Contracts;

namespace UVP.BackOffice.Api.Controllers
{
    [Route("api/backoffice/Entity")]
    public class EntityController : ApiController
    {
        private readonly IEntityResourceFactory entityResourceFactory;
        private readonly JsonSerializerSettings settings;

        public EntityController(IEntityResourceFactory entityResourceFactory)
        {
            this.entityResourceFactory = entityResourceFactory;
            this.settings = new JsonSerializerSettings
            {
                Converters =
                {
                    new StringEnumConverter(new CamelCaseNamingStrategy() { OverrideSpecifiedNames = true }, allowIntegerValues: false),
                    new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ" }
                },
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }
        
        [HttpGet]
        public Task<IActionResult> GetAllEntities()
        {
            var entityResourceResponse = this.entityResourceFactory.GetAllEntities();
            return Task.FromResult<IActionResult>(new JsonResult(entityResourceResponse) { SerializerSettings = settings });
        }
        
        [HttpGet("{entityName}")]
        public async Task<IActionResult> GetEntities([FromQuery] PageDescriptor pagedModel, string entityName)
        {
            var entityResourceResponse = await this.entityResourceFactory.GetEntities(entityName, pagedModel);
            return new JsonResult(entityResourceResponse) { SerializerSettings = settings };
        }
        
        [HttpGet("{entityName}/{enntityId}")]
        public async Task<IActionResult> GetModel(string entityName, long enntityId, bool create = false)
        {
            var entityResourceResponse = await this.entityResourceFactory.GetModel(entityName, enntityId, create);
            return new JsonResult(entityResourceResponse) { SerializerSettings = settings };
        }

        [HttpPost("{entityName}/{enntityId}")]
        public async Task<IActionResult> SaveModel(string entityName, long enntityId, [FromBody] string jsonContent, bool create = false)
        {
            var entityResourceResponse = await this.entityResourceFactory.SaveModel(entityName, enntityId, jsonContent, this.settings, create);
            return new JsonResult(entityResourceResponse) { SerializerSettings = settings };
        }
    }
}