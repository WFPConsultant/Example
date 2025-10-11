using UVP.Shared.Micro.Repositories.Handlers;

namespace UVP.BackOffice.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Koa.Domain.Search.Page;
    using Koa.Hosting.AspNetCore.Model;
    using Koa.Platform.ValueWrapper;
    using Microsoft.AspNetCore.Mvc;
    using UVP.BackOffice.Business;
    using UVP.Shared.Http.BPM;
    using UVP.Shared.Http.Integration;
    using UVP.Shared.Http.MasterTables;
    using UVP.Shared.Micro.Api.Claims;
    using UVP.Shared.Micro.Api.Controllers;
    using UVP.Shared.Micro.Api.Extensions;
    using UVP.Shared.Micro.Api.Translate.Attribute;
    using UVP.Shared.Micro.Api.UvpPermission;
    using UVP.Shared.Model.BackOffice.Document;
    using UVP.Shared.Model.Doa.DoaCandidate;
    using UVP.Shared.Model.Integration;
    using UVP.Shared.Model.MasterTables;
    using UVP.Shared.Model.Profile;

    /// <summary>
    /// BackOffice Controller.
    /// </summary>
    [Route("api/backoffice")]
    public class BackOfficeController : ApiController
    {
        private readonly IEnumerable<IBackOfficeHandler> handlers;
        private readonly IBackOfficeFileHandler fileHandler;
        private readonly IMasterTableHttp mastertableHttp;
        private readonly IIntegrationHttp integrationHttp;
        private readonly IBPMHttp bpmHttp;
        private readonly IFileManagerService fileManagerService;


        /// <summary>
        /// Initializes a new instance of the <see cref="BackOfficeController"/> class.
        /// </summary>
        /// <param name="handlers">Handlers inyected.</param>
        /// <param name="fileHandler">handler of files.</param>
        /// <param name="mastertableHttp">http service for mastertable.</param>
        /// <param name="integrationHttp">http service for integration.</param>
        /// <param name="bpmHttp">http service for bpm.</param>
        public BackOfficeController(IEnumerable<IBackOfficeHandler> handlers,
            IBackOfficeFileHandler fileHandler,
            IMasterTableHttp mastertableHttp,
            IIntegrationHttp integrationHttp,
            IBPMHttp bpmHttp, IFileManagerService fileManagerService)
        {
            this.handlers = handlers;
            this.fileHandler = fileHandler;
            this.mastertableHttp = mastertableHttp;
            this.integrationHttp = integrationHttp;
            this.bpmHttp = bpmHttp;
            this.fileManagerService = fileManagerService;
        }

        /// <summary>
        /// Search generic method for any backoffice entity.
        /// </summary>
        /// <param name="pageDescriptor">page descriptor for filter.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [TypeFilter(typeof(TranslatableAsyncAttribute<dynamic>))]
        [HttpPost("tasks/search")]
        public async Task<IActionResult> GetTasksAsync([FromBody] PageDescriptor pageDescriptor)
        {
            var isOnBehalf = this.IsOnBehalfOf;
            return await this.GetAsync(pageDescriptor, "task", isOnBehalf);
        }

        private async Task<IActionResult> ExportAsync(ExportCsvModel exportCsvModel)
        {
            var language = this.User.Claims.Where(x => x.Type.Equals(UserClaims.Language)).FirstOrDefault().Value;
            var isOnBehalf = this.IsOnBehalfOf;
            var validation = ValueWrapper<ExportCsvModel>.FromValue(exportCsvModel).Validate();
            if (validation.Error != null)
            {
                return this.BadRequest(validation.Error);
            }

            var result = await this.GetBackOfficeHandler("task").Export(exportCsvModel, language, isOnBehalf);

            return this.Ok(result);
        }

        /// <summary>
        /// Export active tasks.
        /// </summary>
        /// <param name="exportCsvModel">export model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [UvpAuthorize(PermissionCode.DownloadTasks)]
        [HttpPost("tasks/export/search")]
        public async Task<IActionResult> ExportTasksAsync([FromBody] ExportCsvModel exportCsvModel)
            => await this.ExportAsync(exportCsvModel);

        /// <summary>
        /// Search generic method for any backoffice entity.
        /// </summary>
        /// <param name="pageDescriptor">page descriptor for filter.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [TypeFilter(typeof(TranslatableAsyncAttribute<dynamic>))]
        [HttpPost("tasks/search/completed")]
        public async Task<IActionResult> GetHistoricTasksAsync([FromBody] PageDescriptor pageDescriptor)
        {
            pageDescriptor.Filters.Add(new Koa.Domain.Search.Filter.FilterDescriptor() { Member = "deleteReason", Operator = Koa.Domain.Search.Filter.FilterOperator.IsEqualTo, Value = "completed" });
            return await this.GetAsync(pageDescriptor, "task", this.IsOnBehalfOf);
        }

        /// <summary>
        /// Export completed tasks.
        /// </summary>
        /// <param name="exportCsvModel">export model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [UvpAuthorize(PermissionCode.DownloadTasks)]
        [HttpPost("tasks/export/search/completed")]
        public async Task<IActionResult> ExportHistoricTasksAsync([FromBody] ExportCsvModel exportCsvModel)
        {
            exportCsvModel.PageDescriptor.Filters.Add(new Koa.Domain.Search.Filter.FilterDescriptor() { Member = "deleteReason", Operator = Koa.Domain.Search.Filter.FilterOperator.IsEqualTo, Value = "completed" });

            return await this.ExportAsync(exportCsvModel);
        }

        /// <summary>
        /// Get the list of task definition keys.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [TypeFilter(typeof(TranslatableAsyncAttribute<List<MasterTableValueResultModel>>))]
        [HttpGet("tasks/codes")]
        public async Task<IActionResult> GetTaskDefinitionKeysAsync() => this.Ok(await this.bpmHttp.GetTaskDefinitionKeysAsync());

        /// <summary>
        /// Search generic method for any backoffice entity.
        /// </summary>
        /// <param name="pageDescriptor">page descriptor for filter.</param>
        /// <param name="entity">Entity type name.</param>
        /// <param name="isOnBehalf">isOnBehalf.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        // [UvpAuthorize(PermissionCode.EditTemplates)]
        [HttpPost("{entity}/search")]
        public async Task<IActionResult> GetAsync([FromBody] PageDescriptor pageDescriptor, string entity, bool isOnBehalf )
        {
            var roleCode = this.ApiUser.RoleId;
            var userId = this.ApiUser.UserId;
            return await ValueWrapper<PageDescriptor>
                                   .FromValue(pageDescriptor)
                                   .Validate()
                                   .DoAsync(async _ => await this.GetBackOfficeHandler(entity).Get(pageDescriptor, userId, isOnBehalf, roleCode))
                                   .ToActionResultAsync(true);
        }

        /// <summary>
        /// Post generic method for any backoffice entity.
        /// </summary>
        /// <param name="object">Model object with the information from the object which would be created.</param>
        /// <param name="entity">Entity type name.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [UvpAuthorize(PermissionCode.EditTemplates)]
        [HttpPost("{entity}")]
        public async Task<IActionResult> PostAsync([FromBody] object @object, string entity) =>
            this.Ok(await this.GetBackOfficeHandler(entity).Post(@object));

        /// <summary>
        /// Put generic method for any backoffice entity.
        /// </summary>
        /// <param name="object">Model object with the information from the object which would be updated.</param>
        /// <param name="entity">Entity type name.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [UvpAuthorize(PermissionCode.EditTemplates)]
        [HttpPut("{entity}")]
        public async Task<IActionResult> PutAsync([FromBody] object @object, string entity) =>
            this.Ok(await this.GetBackOfficeHandler(entity).Put(@object));

        /// <summary>
        /// Put generic method for any backoffice entity.
        /// </summary>
        /// <param name="id">Id db reference from the object which would be updated.</param>
        /// <param name="entity">Entity type name.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [UvpAuthorize(PermissionCode.EditTemplates)]
        [HttpDelete("{entity}/{id}")]
        public async Task<IActionResult> DeleteAsync(long id, string entity) =>
            this.Ok(await this.GetBackOfficeHandler(entity).Delete(id));

        /// <summary>
        /// Search generic method for any backoffice entity.
        /// </summary>
        /// <param name="entityType">page descriptor for filter.</param>
        /// <param name="entityId">Entity type name.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("files/{entityType}/{entityId}")]
        [TypeFilter(typeof(TranslatableAsyncAttribute<GetFilesByEntityResponseModel>))]
        public async Task<IActionResult> GetFilesByEntity(string entityType, long entityId) => await ValueWrapper<string>
                    .FromValue(entityType)
                    .DoAsync(async _ =>
                    {
                        var language = this.User.Claims.Where(x => x.Type.Equals(UserClaims.Language)).FirstOrDefault().Value;

                        var files = await this.fileHandler.GetFilesByentity(entityType, entityId, this.ApiUser.RoleId, this.ApiUser.UserId, this.ApiUser.Permissions);

                        var relatedEntities = await this.mastertableHttp.GetChildrenValueByMasterTableCodeChildrenMasterCodeAndLanguage("DocumentEntity", entityType, "DocumentEntity", language);

                        return new GetFilesByEntityResponseModel() { Files = files, RelatedEntities = relatedEntities };
                    })
                    .ToActionResultAsync(true);

        /// <summary>
        /// Deletes file.
        /// </summary>
        /// <param name="fileId">file id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpDelete("files/{fileId}")]
        public async Task<IActionResult> DeleteFile(long fileId) => await ValueWrapper<long>
                    .FromValue(fileId)
                    .DoAsync(async _ => await this.fileHandler.DeleteFile(fileId, this.ApiUser.Nameidentifier))
                    .ToActionResultAsync(true);

        /// <summary>
        /// Search generic method for any backoffice entity.
        /// </summary>
        /// <param name="parentEntityType">type of the parent.</param>
        /// <param name="parentEntityId">id of the parent.</param>
        /// <param name="descendantsEntityType">type of the entity searched.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("descendantsIds/{parentEntityType}/{parentEntityId}/{descendantsEntityType}")]
        public async Task<IActionResult> GetDescendantsByEntity(string parentEntityType, long parentEntityId, string descendantsEntityType)
        {
            var ids = await this.fileHandler.GetIdsByEntity(parentEntityType, parentEntityId, descendantsEntityType);

            return this.Ok(ids);
        }

        /// <summary>
        /// This operation sends a file to azure storage.
        /// </summary>
        /// <param name="fileUpload">File to upload.</param>
        /// <param name="entityType">entity type.</param>
        /// <param name="entityId">entity id.</param>
        /// <returns>List of documents Id's uploaded.</returns>
        [HttpPost("upload/{entityType}/{entityId}")]
        public async Task<IActionResult> UploadGenericFile([FromBody] MultiUploadedFileModel<FileUploadDataModel> fileUpload, string entityType, long entityId)
            => await this.UploadGenericFile(fileUpload, entityType, entityId, null, null);

        /// <summary>
        /// This operation sends a file to azure storage.
        /// </summary>
        /// <param name="fileUpload">File to upload.</param>
        /// <param name="entityType">entity type.</param>
        /// <param name="entityId">entity id.</param>
        /// <param name="parentEntityType">parent entity type.</param>
        /// <param name="parentEntityId">parent entity id.</param>
        /// <returns>List of documents Id's uploaded.</returns>
        [HttpPost("upload/{entityType}/{entityId}/{parentEntityType}/{parentEntityId}")]
        public async Task<IActionResult> UploadGenericFile([FromBody] MultiUploadedFileModel<FileUploadDataModel> fileUpload, string entityType, long entityId, string parentEntityType, long? parentEntityId)
        {
            var result = await this.fileHandler.UploadGenericFile(fileUpload.ToMultiUploadedFileShared(), this.ApiUser.RoleId, entityType, entityId, parentEntityType, parentEntityId);
            if (result == null || !result.Any())
            {
                return this.BadRequest(result);
            }

            return this.Ok(result);
        }

        /// <summary>
        /// Get DocumentType role permissions
        /// </summary>
        /// <returns>list of permissions</returns>
        [HttpGet("documentTypePermissions")]
        public async Task<IActionResult> GetDocumentTypePermissions() => await ValueWrapper<long>
                    .FromValue(0)
                    .DoAsync(async _ => await this.fileHandler.GetDocumentTypePermissions(this.ApiUser.RoleId))
                    .ToActionResultAsync(true);

        /// <summary>
        /// This operation sends a file to azure storage not mapped to an entity Id.
        /// </summary>
        /// <param name="fileUpload">File to upload.</param>
        /// <param name="entityType">entity type.</param>
        /// <param name="parentEntityType">parent entity type.</param>
        /// <param name="parentEntityId">parent entity id.</param>
        /// <returns>List of documents Id's uploaded.</returns>
        [HttpPost("uploadTemp/{entityType}/{parentEntityType}/{parentEntityId}")]
        public async Task<IActionResult> UploaTempdGenericFile([FromBody] MultiUploadedFileModel<FileUploadDataModel> fileUpload, string entityType, string parentEntityType, long? parentEntityId = null)
        {
            var result = await this.fileHandler.UploadTempGenericFile(fileUpload.ToMultiUploadedFileShared(), this.ApiUser.RoleId, entityType, parentEntityType, parentEntityId);
            return this.Ok(result);
        }

        /// <summary>
        /// This operation sends a file to azure storage not mapped to an entity Id.
        /// </summary>
        /// <param name="fileUpload">File to upload.</param>
        /// <param name="entityType">entity type.</param>
        /// <param name="parentEntityType">parent entity type.</param>
        /// <returns>List of documents Id's uploaded.</returns>
        [HttpPost("uploadTemp/{entityType}/{parentEntityType}")]
        public async Task<IActionResult> UploaTempdGenericFile([FromBody] MultiUploadedFileModel<FileUploadDataModel> fileUpload, string entityType, string parentEntityType) => await this.UploaTempdGenericFile(fileUpload, entityType, parentEntityType, null);

        /// <summary>
        /// This operation sends a file to azure storage not mapped to an entity Id.
        /// </summary>
        /// <param name="fileUpload">File to upload.</param>
        /// <param name="entityType">entity type.</param>
        /// <returns>List of documents Id's uploaded.</returns>
        [HttpPost("uploadTemp/{entityType}")]
        public async Task<IActionResult> UploaTempdGenericFileWithoutParent([FromBody] MultiUploadedFileModel<FileUploadDataModel> fileUpload, string entityType) => await this.UploaTempdGenericFile(fileUpload, entityType, null, null);

        /// <summary>
        /// Gets a file by its path.
        /// </summary>
        /// <param name="path">File's path.</param>
        /// <returns>An array of bytes.</returns>
        [HttpGet("file/download/{id}")]
        [UvpAuthorize(PermissionCode.DownloadCsvFiles)]
        public async Task<IActionResult> GetDownloadedFileById(long id, string fileType)
        {
            return await ValueWrapper<long>
                    .FromValue(id)
                    .DoAsync(async _ => await this.fileManagerService.GetDownloadedTempFileById(id,fileType))
                    .ToActionResultAsync(true);
        }

        /// <summary>
        /// Gets a file by its path.
        /// </summary>
        /// <param name="path">File's path.</param>
        /// <returns>An array of bytes.</returns>
        [HttpPost("download")]
        public async Task<IActionResult> DownloadFile(string path)
        {
            if (path.IsNullOrEmpty())
            {
                return this.BadRequest("Path is empty");
            }

            return await ValueWrapper<string>
                    .FromValue(path)
                    .Validate()
                    .DoAsync(async _ => await this.integrationHttp.GenerateFileArray(path))
                    .ToActionResultAsync(true);
        }

        /// <summary>
        /// Verification file.
        /// </summary>
        /// <param name="fileId">file id.</param>
        /// <param name="isVerified">isVerified.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("files/verification/{fileId}/{isVerified}")]
        [UvpAuthorize(PermissionCode.ValidateDocuments)]
        public async Task<IActionResult> VerificationFile(long fileId, bool isVerified) => await ValueWrapper<long>
                    .FromValue(fileId)
                    .DoAsync(async _ => await this.fileHandler.VerificationFile(fileId, isVerified, this.ApiUser.UserId))
                    .ToActionResultAsync(true);

        /// <summary>
        /// Get Entity Email By ModelCode.
        /// </summary>
        /// <param name="entity">entity.</param>
        /// <param name="modelCode">template model of the email.</param>
        /// <returns>list of emails.</returns>
        [HttpGet("{entity}/{modelCode}")]
        public async Task<IActionResult> GetEmailsByModelCode(string entity, string modelCode)
        {
            var roleCode = this.ApiUser.RoleId;
            var userId = this.ApiUser.UserId;
            return
                 await ValueWrapper<string>.FromValue(entity).Validate().
                 DoAsync(async _ => await this.GetBackOfficeHandler(entity).GetByModelCode(modelCode, userId, roleCode))
                .ToActionResultAsync(true);
        }

        private IBackOfficeHandler GetBackOfficeHandler(string name) =>
        this.handlers.FirstOrDefault(handler => handler.Entity.ToString().ToLower().Equals(name.ToLower()));
    }
}
