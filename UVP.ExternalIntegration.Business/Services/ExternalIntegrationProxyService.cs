using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Koa.Platform.Injection;
using Microsoft.Extensions.Logging;
using UVP.ExternalIntegration.Business.Model.Common;
using UVP.ExternalIntegration.Business.Model.Response;
using UVP.ExternalIntegration.Business.Services.Contracts;
using UVP.Shared.Model.Doa;

namespace UVP.Doa.Business.Saga.Command.DoaCandidateCommand
{
    [Injectable(typeof(IExternalIntegrationProxyService))]
    public class ExternalIntegrationProxyService : IExternalIntegrationProxyService
    {
        private readonly ILogger<IExternalIntegrationProxyService> logger;
        private readonly IExternalIntegrationService externalIntegrationService;
        /// <summary>
        /// Initializes a new instance of the <see cref="UserHttp"/> class.
        /// Default constructor.
        /// </summary>
        /// <param name="client">Http client.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="jsonSerializer">json serializer.</param>
        public ExternalIntegrationProxyService(ILogger<IExternalIntegrationProxyService> logger, IExternalIntegrationService externalIntegrationService)
        {
            this.logger = logger;
            this.externalIntegrationService = externalIntegrationService;
        }

        /// <inheritdoc/>
        public async Task<ReturnObject> CreateEmployeeExternalIntegration(long doaCandidateId)
        {
            return await this.externalIntegrationService.CreateEmployee(doaCandidateId);
        }

        /// <inheritdoc/>
        public async Task<ReturnObjectPayrollOTE> UploadApprovedPayments()
        {
            return  await this.externalIntegrationService.PostPayrollOTEDataToQuantum();
        }

        /// <inheritdoc/>
        public async Task<string> GetQuantumLinkforAssignment(string personId, string assignmentId, string effectiveDate)
        {
            try
            {
                return await this.externalIntegrationService.GetQuantumAssignmentLink(personId, assignmentId, effectiveDate);                
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex,
                    $"Quantum link could not be retrieved for assignmentId: {assignmentId}");
            }
            return null;
        }

        public async Task<ErpVolunteerHiringDataLogResponseModel> GetErpDataPostedToQuantum(long doaCandidateId)
        {
            return await this.externalIntegrationService.GetErpVolunteerHiringDataLog(doaCandidateId);            
        }

        public async Task<List<ErpApiStatus>> GetErpApiStatusForCandidate(long doaCandidateId)
        {
            var response = await this.externalIntegrationService.GetAPIStatusDisplayByDoaCandidateId(doaCandidateId);
            return response.Select(model => new ErpApiStatus()
            {
                ApiCode = model.APICode,
                ApiName = model.APIName,
                ApiStatus = model.APIStatus
            }).ToList();
        }

        public async Task<List<long>> GetErpHiredCandidates()
        {
            return await this.externalIntegrationService.GetErpErrorFrameworkAPITransactionRequest();            
        }

        public async Task<ErpVolunteerHiringDataLogResponseModel> GetErpDataManualPostedToQuantum(long doaCandidateId)
        {
            return await this.externalIntegrationService.GetErpVolunteerManualHiringDataLog(doaCandidateId);
        }
    }
}
