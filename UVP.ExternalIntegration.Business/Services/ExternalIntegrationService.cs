using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Koa.Domain.Specification;
using Koa.Persistence.EntityRepository;
using Koa.Platform.Injection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UVP.Doa.Domain.Sql.Entities;
using UVP.ExternalIntegration.Business.EndPoints;
using UVP.ExternalIntegration.Business.Model.Common;
using UVP.ExternalIntegration.Business.Model.Request;
using UVP.ExternalIntegration.Business.Model.Response;
using UVP.ExternalIntegration.Business.Services.Contracts;
using UVP.ExternalIntegration.Business.Utilities;
using UVP.ExternalIntegration.Domain;
using UVP.ExternalIntegration.Domain.Entity.Doa;
using UVP.ExternalIntegration.Domain.Entity.Users;
using UVP.ExternalIntegration.Domain.Repository.Doa;
using UVP.ExternalIntegration.Domain.Repository.Users;
using UVP.ExternalIntegration.ErrorValidationFramework.Enum;
using UVP.ExternalIntegration.ErrorValidationFramework.Model;
using UVP.ExternalIntegration.ErrorValidationFramework.Repository;
using UVP.Integration.PubSub.Hangfire;
using UVP.Shared.Domain.Specification;
using UVP.Shared.Micro.Domain;
using UVP.Shared.Micro.Domain.Enum;
using UVP.Shared.Micro.Entities.Sql.Entities;
using UVP.Shared.Model.Shared;
using UVP.TaskProcessor.Domain.Core;
using Assignment = UVP.ExternalIntegration.Business.Model.Common.Assignment;
using Contract = UVP.Doa.Domain.Sql.Entities.Contract;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace UVP.ExternalIntegration.Business.Services
{
    using Contract = Contract;

    /// <summary>
    /// Implements <see cref="IExternalIntegrationService"/> interface.
    /// </summary>
    [Injectable(typeof(IExternalIntegrationService))]
    public class ExternalIntegrationService : IExternalIntegrationService
    {
        private static readonly JsonSerializerOptions JsonOptions = new () { PropertyNameCaseInsensitive = true };
        private readonly ILogger<ExternalIntegrationService> logger;
        private readonly ICandidateRepository candidateRepository;
        private readonly IAssignmentRepository assignmentRepository;
        private readonly IDBLoggerRepository errorFrameworkRepository;
        private readonly IMapper mapper;
        private readonly HttpClient httpClient;
        private readonly ExternalIntegrationOptions appSettings;
        private readonly ExternalIntegrationEndPoints ExternalIntegrationEndPoints;
        private long? personId;
        private string workerHash;
        private string personNumber;
        private string assignmentNumber;
        private string assignmentId;
        private string assignmentHash;
        private string contractNumber;
        private string contractId;
        private string contractHash;
        private string effectiveDate;
        private string integrationStatusCode;
        private string nationality;
        private CandidateModel candidateModel;
        private AssignmentModel assignmentModel;
        private DependentDetails dependentModel;
        private string apiTransactionId;
        private string chartType;
        private string candidateId;
        private long doaCandidateId;
        private string volType;
        private string nationalPayrollId;
        private string internationalPayrollId;
        private string assignmentHref;
        private readonly IEntityRepository<long> entityRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IUvpBackgroundJobClient cognitiveSearchManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalIntegrationService"/> class.
        /// User service.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/>.</param>
        /// <param name="httpClient"><see cref="HttpClient"/>.</param>
        /// <param name="mapper"><see cref="IMapper"/>.</param>
        /// <param name="candidateRepository"><see cref="ICandidateRepository"/>.</param>
        /// <param name="assignmentRepository"><see cref="IAssignmentRepository"/>.</param>
        /// <param name="optionsMonitor"><see cref="optionsMonitor"/>.</param>
        /// <param name="entityRepository">entity repository.</param>
        /// <param name="unitOfWork">unitOfWork.</param>
        public ExternalIntegrationService(
            ILogger<ExternalIntegrationService> logger,
            HttpClient httpClient,
            IMapper mapper,
            ICandidateRepository candidateRepository,
            IAssignmentRepository assignmentRepository,
            IDBLoggerRepository errorRepository,
            IOptionsMonitor<ExternalIntegrationOptions> optionsMonitor,
            IEntityRepository<long> entityRepository,
            IUnitOfWork unitOfWork, 
            IUvpBackgroundJobClient cognitiveSearchManager)
        {
            var appSettings = optionsMonitor.CurrentValue;
            if (appSettings.IsNull())
            {
                throw new Exception("Failed to configure the service");
            }

            this.logger = logger;
            this.mapper = mapper;
            this.httpClient = httpClient;
            this.candidateRepository = candidateRepository;
            this.assignmentRepository = assignmentRepository;
            errorFrameworkRepository = errorRepository;
            this.appSettings = appSettings;
            ExternalIntegrationEndPoints = new ExternalIntegrationEndPoints(appSettings.RestApiFolder);
            nationalPayrollId = appSettings.NationalPayrollId;
            internationalPayrollId = appSettings.InternationalPayrollId;
            this.entityRepository = entityRepository;
            this.unitOfWork = unitOfWork;
            this.cognitiveSearchManager = cognitiveSearchManager;
        }

        /// <inheritdoc cref="IExternalIntegrationService.GetWorkerByName(EmployeeSearchRequestModel)"/>
        public async Task<ReturnObject> GetWorkerByName(EmployeeSearchRequestModel model)
        {
            var response = new ReturnObject();
            try
            {
                var responseWorker = await GetWorkersByNameRequest(model);

                response.Response = responseWorker;
                response.Message = string.Empty;
                response.ResponseCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"ERROR on 'GetWorkerByName': {ex}");

                response.Response = null;
                response.Message = ex.Message;
                response.ResponseCode = HttpStatusCode.BadRequest;
            }

            return response;
        }

        /// <inheritdoc cref="IExternalIntegrationService.CreateEmployee(long, long)"/>
        public async Task<ReturnObject> CreateEmployee(long doaCandidateId)
        {
            this.doaCandidateId = doaCandidateId;
            var returnObject = new ReturnObject();


            try
            {
                //duplicate check at starting of call
                var result = await IsDuplicateEntryCheck(doaCandidateId);
                if (result == null || result.ResponseCode == 1)
                {
                    returnObject.ResponseCode = HttpStatusCode.OK;
                    returnObject.PersonId = -1;
                    returnObject.Message = "DuplicateFromDB";
                    return returnObject;
                }

                returnObject = await HireEmployee(doaCandidateId);
                if (returnObject.ResponseCode == HttpStatusCode.Ambiguous && returnObject.PersonNumber.IsNotNull()
                    && returnObject.Message == "CandidateExistsInQuantum")
                {
                    returnObject.ResponseCode = HttpStatusCode.Ambiguous;
                    returnObject.PersonNumber = returnObject.PersonNumber;
                    returnObject.Message = "CandidateExistsInQuantum";
                    return returnObject;

                }

                if (returnObject.ResponseCode == HttpStatusCode.OK && returnObject.PersonId == -1
                                                                   && returnObject.Message == "DuplicateFromDB")
                {
                    returnObject.ResponseCode = HttpStatusCode.OK;
                    returnObject.PersonId = -1;
                    returnObject.Message = "DuplicateFromDB";
                    return returnObject;

                }


                if (returnObject.ResponseCode == HttpStatusCode.OK)
                {
                    // setting return values

                    returnObject.PersonHash = workerHash;
                    returnObject.PersonId = personId;
                    returnObject.PersonNumber = personNumber;

                    returnObject.AssignmentId = Convert.ToInt64(assignmentId);
                    returnObject.AssignmentNumber = assignmentNumber;
                    returnObject.AssignmentHash = assignmentHash;

                    returnObject.ContractId = Convert.ToInt64(contractId);
                    returnObject.ContractNumber = contractNumber;
                    returnObject.ContractHash = contractHash;


                    await UpdateCoa(chartType, this.doaCandidateId);

                    await UpdateNationality();

                    await UpdateAgencyInfo();

                    // Post dependents information data to Quantum.
                    await UpdateDependentsInfo(doaCandidateId);

                    await UpdateSalaryInfo(doaCandidateId);

                    await UpdateSetExternalIdentifiers(doaCandidateId);

                    await SetErpPayrollRelationship();

                    await UpdateAddress();

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"ERROR on Method 'CreateEmployee': {ex}");

                returnObject.Response = null;
                returnObject.Message = ex.Message;
                returnObject.ResponseCode = HttpStatusCode.BadRequest;
            }

            return returnObject;
        }

        private async Task<ReturnObject> HireEmployee(long doaCandidateId)
        {

            var response = new ReturnObject();
            try
            {


                logger.LogDebug($"GetCandidateById: {0}", doaCandidateId);
                candidateModel = await GetCandidateById(doaCandidateId);

                logger.LogDebug($"GetAssignmentById: {0}", doaCandidateId);
                assignmentModel = await GetAssignmentById(doaCandidateId);

                if (candidateModel != null && assignmentModel != null)
                {
                    logger.LogDebug($"GenerateCreateEmployeeModel: {0},{1}", candidateModel, assignmentModel);
                    var model = await GenerateCreateEmployeeModel(candidateModel, assignmentModel);

                    if (model != null)
                    {
                        logger.LogDebug($"ValidateCreateEmployeeModel: {0}", model);
                        var validateResponse = await ValidateCreateEmployeeModel(model);
                        if (string.IsNullOrEmpty(validateResponse))
                        {
                            // checking duplicate call in Transaction table as well as in database
                            // duplicate check 
                            var result = await IsDuplicateEntryCheck(doaCandidateId);
                            if (result == null || result.ResponseCode == 1)
                            {
                                response.ResponseCode = HttpStatusCode.OK;
                                response.PersonId = -1;
                                response.Message = "DuplicateFromDB";
                                return response;

                            }

                            if (result.ResponseCode == 2)
                            {
                                await LogAPITransaction(this.doaCandidateId, apiTransactionId, APIStep.Result, "", "", APICodes.ErpHire.ToString(), APIErrorCodes.ReHire.ToString(), APIAction.Save.ToString());
                                response.ResponseCode = HttpStatusCode.Ambiguous;
                                response.PersonNumber = result.ErpPersonNumber;
                                response.Message = "CandidateExistsInQuantum";
                                return response;

                            }

                            logger.LogDebug($"CreateEmployeeRequest: {0}", model);
                            var createEmployeeResponse = await CreateEmployeeRequest(model, assignmentModel.HireDate);
                            if (createEmployeeResponse != null && createEmployeeResponse.PersonId != null)
                            {
                                logger.LogDebug("Employee created successfully.");

                                personId = createEmployeeResponse.PersonId;
                                workerHash = await SetWorkerHash(createEmployeeResponse.links);
                                personNumber = createEmployeeResponse.PersonNumber;

                                assignmentId = createEmployeeResponse.WorkRelationships.Items[0].Assignments.Items[0].AssignmentId.ToString();
                                assignmentNumber = createEmployeeResponse.WorkRelationships.Items[0].Assignments.Items[0].AssignmentNumber;
                                assignmentHash = await SetAssignmentHash(createEmployeeResponse.WorkRelationships.Items[0].Assignments.Items[0].Links);

                                contractId = createEmployeeResponse.WorkRelationships.Items[0].Contracts.Items[0].ContractId.ToString();
                                contractNumber = createEmployeeResponse.WorkRelationships.Items[0].Contracts.Items[0].ContractNumber;
                                contractHash = await SetContractHash(createEmployeeResponse.WorkRelationships.Items[0].Contracts.Items[0].Links);

                                effectiveDate = assignmentModel.HireDate;
                                /* originally saving the birth country
                                this.nationality = this.candidateModel.CountryOfBirth;
                                */
                                nationality = candidateModel.CurrentNationality;
                                chartType = assignmentModel.ChartType;
                                candidateId = candidateModel.CandidateId.ToString();
                                volType = assignmentModel.WorkerCategory;

                                response.Response = createEmployeeResponse;
                                response.Message = "Employee created successfully.";
                                response.ResponseCode = HttpStatusCode.OK;
                            }
                            else
                            {
                                await LogAPITransaction(this.doaCandidateId, apiTransactionId, APIStep.Result, "", response.PersonId.ToString(), APICodes.ErpCheckPersonExists.ToString(), APIErrorCodes.DuplicateRecord.ToString());

                                logger.LogDebug($"Some error occured, please contact Administrator {0}.", model);
                                response.Response = null;
                                response.Message = "Some error occured, please contact Administrator.";
                                response.ResponseCode = HttpStatusCode.Ambiguous;
                            }

                        }

                        else
                        {
                            logger.LogDebug($"Error on ValidateCreateEmployeeModel: {0}", validateResponse);

                            response.Response = null;
                            response.Message = validateResponse;
                            response.ResponseCode = HttpStatusCode.NotFound;
                        }
                    }
                }
                else
                {
                    logger.LogDebug("Candidate or Assignment object is NULL");

                    response.Response = null;
                    response.Message = "Candidate or Assignment object is NULL";
                    response.ResponseCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"ERROR on HireEmployee: {ex}");
                if (ex.Message.Contains("PER_EMCOR_DUPL_PERSON_FOUND"))
                {
                    response.Response = null;
                    response.Message = "A Person with that name and date of birth already exists in Quantum. Please do the hire in Quantum manually using the 'REHIRE' action";
                    response.ResponseCode = HttpStatusCode.BadRequest;
                    return response;
                }

                response.Response = null;
                response.Message = "Error on HireEmployee: " + ex.Message;
                response.ResponseCode = HttpStatusCode.BadRequest;
            }

            return response;
        }

        private async Task<CandidateModel> GetCandidateById(long doaCandidateId)
        {
            var model = await candidateRepository.GetCandidateByIdAsync(doaCandidateId);
            return mapper.Map<Candidate, CandidateModel>(model);
        }

        private async Task<AssignmentModel> GetAssignmentById(long doaCandidateId, bool migrated = false)
        {
            var model = await assignmentRepository.GetAssignmentByIdAsync(doaCandidateId, migrated);
            return mapper.Map<UserAssignment, AssignmentModel>(model);
        }

        private async Task<DependentDetails> GetDependentDetailsByDoaCandidateId(long doaCandidateId)
        {
            var doaCandididateModel = entityRepository.Where(new EntityByIdSpecification<DoaCandidate>(doaCandidateId))
                .Include(doa => doa.Doa).FirstOrDefault();

            var dependentDetailsModel = new DependentDetails();
            if (doaCandididateModel?.Doa != null && doaCandididateModel.Doa.IsOnsite)
            {      
                    dependentDetailsModel = await candidateRepository.GetDependentsByDoaCandidateIdAsync(doaCandidateId);
                    return dependentDetailsModel;
            }

            return dependentDetailsModel;
        }

        private async Task<PayrollOTEDetails> GetPayrollOTEDetailsForQuantumPosting()
        {
            var model = await assignmentRepository.GetPayrollOTEDetailsForQuantumPostingAsync();
            return model;
        }

        private async Task<SalaryBasisModel> GetSalaryBasisDetailsByDoaCandidateId(long doaCandidateId)
        {
            var model = await assignmentRepository.GetSalaryBasisDetailsByDoaIdAsync(doaCandidateId);
            return mapper.Map<SalaryBasis, SalaryBasisModel>(model);
        }

        private async Task<CoaNonPPMDetails> GetCoaNonPPMDetailsByDoaCandidateId(long doaCandidateId)
        {
            var model = await assignmentRepository.GetCoaNONPPMDetailsByDoaIdAsync(doaCandidateId);
            return model;
        }

        private async Task<CoaPPMDetails> GetCoaPPMDetailsByDoaCandidateId(long doaCandidateId)
        {
            var model = await assignmentRepository.GetCoaPPMDetailsByDoaIdAsync(doaCandidateId);
            return model;
        }

        private async Task<bool> IsHREntryDateAllowedById(long doaCandidateId)
        {
            var result = await assignmentRepository.IsHREntryDateAllowedByIdAsync(doaCandidateId);
            return result;
        }



        private async Task<string> ValidateCreateEmployeeModel(EmployeeCreateRequestModel model)
        {
            var response = new StringBuilder();
            if (model.DateOfBirth == null || model.DateOfBirth == string.Empty)
            {
                response.Append("The DateOfBirth field is required.");
            }

            if (model.Names == null)
            {
                response.Append("The Names field is required.");
            }

            if (model.Emails == null)
            {
                response.Append("The Emails field is required.");
            }

            if (model.LegislativeInfo == null)
            {
                response.Append("The LegislativeInfo field is required.");
            }

            if (model.WorkRelationships == null)
            {
                response.Append("The WorkRelationships field is required.");
            }
            else
            {
                if (model.WorkRelationships[0].Assignments == null)
                {
                    response.Append("The Assignments field is required.");
                }

                if (model.WorkRelationships[0].Contracts == null)
                {
                    response.Append("The Contracts field is required.");
                }
            }

            return response.ToString();
        }

        private async Task<EmployeeCreateRequestModel> GenerateCreateEmployeeModel(CandidateModel candidate, AssignmentModel assignment)
        {

            // correcting Paygroup


            assignment.PeopleGroup = assignment.PeopleGroup.Replace("\\\\", "\\");
            var model = new EmployeeCreateRequestModel
            {
                DateOfBirth = candidate.DateOfBirth?.ToString("yyyy-MM-dd"),
                Names = new[]
                     {
                        new EmpInfo
                        {
                          FirstName = candidate.FirstName,
                          LastName = candidate.LastName,
                          LegislationCode = candidate.LegislationCode,
                        },
                     },
                Emails = new[]
                     {
                        new Email
                        {
                            EmailAddress = candidate.EmailAddress,
                            EmailType = candidate.EmailType,
                        },
                     },
                LegislativeInfo = new[]
                     {
                        new LegislativeInfo
                        {
                            LegislationCode = candidate.LegislationCode,
                            Gender = candidate.Gender,
                            MaritalStatus = candidate.MaritalStatus,
                        },
                     },
                WorkRelationships = new[]
                     {
                        new WorkRelationship
                        {
                            WorkerType = assignment.WorkerType,
                            LegalEntityId = assignment.LegalEntityId,
                            Assignments = new[]
                            {
                                new Assignment
                                {
                                    ActionCode = assignment.ActionCode,
                                    AssignmentName = assignment.AssignmentName,
                                    BusinessUnitId = assignment.BusinessUnitId,
                                    UserPersonType = assignment.UserPersonType,
                                    AssignmentStatusTypeCode = assignment.AssignmentStatusTypeCode,
                                    JobCode = assignment.JobCode,
                                    GradeCode = assignment.GradeCode,
                                    GradeLadderId = assignment.GradeLadderId,
                                    GradeStepEligibilityFlag = true,
                                    DepartmentId = assignment.DepartmentId,
                                    ReportingEstablishmentName = null,
                                    LocationCode = assignment.DutyStationCode,
                                    WorkAtHomeFlag = false,
                                    WorkerCategory = assignment.WorkerCategory,
                                    AssignmentCategory = assignment.AssignmentCategory,
                                    PermanentTemporary = assignment.PermanentTemporary,
                                    FullPartTime = assignment.FullPartTime,
                                    ManagerFlag = false,
                                    HourlySalariedCode = assignment.HourlySalariedCode,
                                    NormalHours = assignment.NormalHours,
                                    Frequency = assignment.Frequency,
                                    StartTime = assignment.StartTime,
                                    EndTime = assignment.EndTime,
                                    SeniorityBasis = assignment.SeniorityBasis,
                                    PeopleGroup = assignment.PeopleGroup,
                                    AssignmentsEFF = new[]
                                    {
                                        new AssignmentEFF
                                        {
                                            AssignmentType = assignment.AssignmentType,
                                            CategoryCode = assignment.CategoryCode,
                                        },
                                    },
                                    AssignmentsDFF = new[]
                                    {
                                        new AssignmentDFF
                                        {
                                            AtlasCompany = assignment.AtlasCompany,
                                            LocationEntryDate = assignment.LocationEntryDate,
                                            ApaLocation = assignment.DutyStationCode,
                                            APALocationEntryDate = assignment.HireDate,
                                            ReimbursementEmployerBenefitPo = assignment.ReimbursementEmployerBenefit,
                                            SpecialPostAllowanceAssignment = assignment.SpecialPostAllowanceAssignment,
                                            AdjustedNextGradeStepDate = null,
                                        },
                                    },
                                    GradeSteps = new[]
                                    {
                                        new GradeStep
                                        {
                                            GradeId = assignment.GradeId,
                                            GradeStepName = assignment.GradeStepName,
                                        },
                                    },
                                },
                            },
                            Contracts = new[]
                            {
                                new Model.Common.Contract
                                {
                                    ContractType = assignment.ContractType,
                                    InitialDuration = assignment.InitialDuration,
                                    InitialDurationUnits = assignment.InitialDurationUnits,
                                    ContractsDFF = new[]
                                    {
                                        new ContractDFF
                                        {
                                            __FLEX_Context = assignment.FLEXContext,
                                            ContractClause = assignment.ContractClause,
                                            Status = null,
                                            EffectiveEndDate1 = null,
                                            DonorCountryEligibility = null,
                                            EntitledToInternationalEntitle = assignment.EntitledToInternationalEntitle,
                                        },
                                    },
                                },
                            },
                        },
                     },
            };
            return model;
        }

        /// <inheritdoc cref="IExternalIntegrationService.GetWorkersByNameRequest(EmployeeSearchRequestModel)"/>
        private async Task<WorkerResponseModel> GetWorkersByNameRequest(EmployeeSearchRequestModel model)
        {
            var endpoint = ExternalIntegrationEndPoints.GetWorkerByName;
            var path = endpoint.PathFormat("\'" + model.LastName.ToUpper() + "\'", "\'" + model.FirstName.ToUpper() + "\'", "\'" + model.DateOfBirth.ToUpper() + "\'", "\'" + model.Gender.ToUpper() + "\'");

            var response = await ExecuteHttpRequest<WorkerResponseModel>(endpoint.Method, path, logger, apiCode: endpoint.APICode, doaCandidateId: doaCandidateId);
            return response;
        }

        /// <inheritdoc cref="IExternalIntegrationService.CreateEmployeeRequest(EmployeeSearchRequestModel)"/>
        private async Task<WorkerResponseModelItems> CreateEmployeeRequest(EmployeeCreateRequestModel model, string hireDate)
        {
            var endpoint = ExternalIntegrationEndPoints.CreateEmployee;
            var path = endpoint.Path;
            var header = new Dictionary<string, string>
            {
                { "Effective-Of", "RangeStartDate=" + hireDate },
            };
            header.Add("Rest-Framework-Version", "4");

            var response = await ExecuteHttpRequest<WorkerResponseModelItems>(endpoint.Method, path, logger, content: model, headers: header, apiCode: endpoint.APICode, doaCandidateId: doaCandidateId, useGzip: true);
            return response;
        }

        /// <inheritdoc cref="IExternalIntegrationService.UpdateNationality()"/>
        private async Task<ReturnObject> UpdateNationality()
        {

            var response = new ReturnObject();
            try
            {
                var endpoint = ExternalIntegrationEndPoints.UpdateNationality;
                var path = endpoint.PathFormat(workerHash.Trim(), personId.ToString().Trim());

                HttpContent model = new StringContent("{\n            \"PeiInformationCategory\": \"Nationality Information\",\n            \"PeiAttributeCategory\": null,\n            \"nationalityType\": \"Official\",\n            \"effectiveDate\": \"" + effectiveDate + "\" ,\n            \"reason\": \"Personal\",\n            \"nationality\": \"" + nationality + "\" \n}");

                var header = new Dictionary<string, string>();
                header.Add("Rest-Framework-Version", "4");
                var updateResponse = await ExecuteHttpRequest<NationalityResponseModel>(endpoint.Method, path, logger, content: model, apiCode: endpoint.APICode, doaCandidateId: doaCandidateId, headers: header, useGzip: true);

                logger.LogDebug("Nationality updated successfully");

                response.Response = updateResponse;
                response.Message = "Nationality updated successfully.";
                response.ResponseCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error on UpdateNationality: {ex}");

                response.Response = false;
                response.Message = "Error on UpdateNationality: " + ex.Message;
                response.ResponseCode = HttpStatusCode.BadRequest;
            }

            return response;
        }

        /// <inheritdoc cref="IExternalIntegrationService.UpdateNationality()"/>
        private async Task<ReturnObject> UpdateAddress()
        {

            var response = new ReturnObject();
            try
            {
                var endpoint = ExternalIntegrationEndPoints.UpdateAddress;
                var path = endpoint.PathFormat(workerHash.Trim());
                var address = new Address
                {
                    AddressType = candidateModel.AddressType,
                    AddressLine1 = candidateModel.AddressLine1,
                    AddressLine2 = candidateModel.AddressLine2,
                    Country = candidateModel.Country[..2],
                    PostalCode = candidateModel.PostalCode,
                    TownOrCity = candidateModel.TownOrCity,
                    Region3 = candidateModel.Region3,
                };
                HttpContent model = new StringContent(JsonConvert.SerializeObject(address));

                var header = new Dictionary<string, string>();
                header.Add("Rest-Framework-Version", "4");
                var updateResponse = await ExecuteHttpRequest<AddressResponseModel>(endpoint.Method, path, logger, content: model, apiCode: endpoint.APICode, doaCandidateId: doaCandidateId, headers: header, useGzip: true);

                logger.LogDebug("Address updated successfully");

                response.Response = updateResponse;
                response.Message = "Address updated successfully.";
                response.ResponseCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error on UpdateAddress: {ex}");

                response.Response = false;
                response.Message = "Error on UpdateAddress: " + ex.Message;
                response.ResponseCode = HttpStatusCode.BadRequest;
            }

            return response;
        }

        private async Task<ReturnObject> UpdateCoa(string ChartType, long DoaCandidateId)
        {

            var response = new ReturnObject();


            try
            {
                if (ChartType.Equals("QUANTUM_PPM") || ChartType.Equals("QUANTUM_PPM_FF"))
                {
                    var returnObject = await UpdateCoaPPM(DoaCandidateId);

                    response.Response = returnObject;
                    response.Message = "QUANTUM_PPM updated successfully.";
                    response.ResponseCode = HttpStatusCode.OK;

                }
                else if (ChartType.Equals("QUANTUM_NON_PPM"))
                {
                    var returnObject = await UpdateCoaNonPPM(DoaCandidateId);
                    response.Response = returnObject;
                    response.Message = "QUANTUM_NON_PPM updated successfully.";
                    response.ResponseCode = HttpStatusCode.OK;
                }
                else if (ChartType.Equals("NON_QUANTUM_SCA"))
                {
                    var returnObject = UpdateCoaNONQuantum(DoaCandidateId);
                    response.Response = returnObject;
                    response.Message = "NON_QUANTUM_SCA updated successfully.";
                    response.ResponseCode = HttpStatusCode.OK;
                }
                else if (ChartType.Equals("NON_QUANTUM_MISS"))
                {
                    var returnObject = await UpdateCoaNONQuantum(DoaCandidateId);
                    response.Response = returnObject;
                    response.Message = "NON_QUANTUM_MISS updated successfully.";
                    response.ResponseCode = HttpStatusCode.OK;
                }



            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error on UpdateCoa: {ex}");

                response.Response = false;
                response.Message = "Error on UpdateCoa: " + ex.Message;
                response.ResponseCode = HttpStatusCode.BadRequest;
            }

            return response;
        }

        private async Task<ReturnObject> UpdateCoaPPM(long DoaCandidateId)
        {

            var response = new ReturnObject();

            try
            {

                var endpoint = ExternalIntegrationEndPoints.UpdateChartOfAccountsPPM;
                var path = endpoint.Path;

                var coaDetails = await GetCoaPPMDetailsByDoaCandidateId(DoaCandidateId);

                //this.assignmentNumber = "E71402877";
                //this.personNumber = "71402877";


                CoaPPM coaPPM = null;
                if (coaDetails != null && coaDetails.CoaPPMList.Count > 0)
                {
                    StringBuilder distributionRules = new StringBuilder();
                    distributionRules.Append("\"distributionRules\": [\n  ");
                    for (var i = 0; i < coaDetails.CoaPPMList.Count; i++)
                    {
                        coaPPM = coaDetails.CoaPPMList[i];

                        distributionRules.Append(" {\n \"LineNumber\": \"" + coaPPM.LineNumber + "\", \n \"LinePercent\": \"" + coaPPM.LinePercent + "\",\n \"ContractNumber\": \"" + coaPPM.ContractNumber + "\",\n  \"ProjectNumber\": \"" + coaPPM.ProjectNumber + "\",\n \"TaskNumber\": \"" + coaPPM.TaskNumber + "\",\n \"FundingSourceNumber\": \"" + coaPPM.FundingSourceNumber + "\",\n \"ExpenditureTypeId\": \"" + coaPPM.ExpenditureTypeId + "\",\n \"ExpenditureOrganizationId\": \"" + coaPPM.ExpenditureOrganizationId + "\",\n \"ContextCategory\": \"" + coaPPM.ContextCategory + "\" }  ");
                        if (i == coaDetails.CoaPPMList.Count - 1)
                            distributionRules.Append(" \n");
                        else
                            distributionRules.Append(", \n");
                    }
                    distributionRules.Append(" \n  ] ");
                    coaPPM = coaDetails.CoaPPMList[0];

                    // for (var i = 0; i < coaDetails.CoaPPMList.Count; i++)
                    {
                        // coaPPM = coaDetails.CoaPPMList[i];
                        // HttpContent model = new StringContent("{\n\"AssignmentNumber\": \"" + this.assignmentNumber + "\",\n \"PersonNumber\": \"" + this.personNumber + "\" ,\n \"LaborScheduleName\": \"" + coaPPM.LaborScheduleName + "\",\n    \"versions\": [\n {\n \"VersionName\": \"" + coaPPM.VersionName + "\",\n \"VersionComments\": \"" + coaPPM.VersionComments + "\",\n \"VersionStatusCode\": \"" + coaPPM.VersionStatusCode + "\",\n \"VersionStartDate\": \"" + coaPPM.StartDate + "\",\n distributionRules\": [\n  {\n \"LineNumber\": \"" + coaPPM.LineNumber.ToString() + "\", \n \"LinePercent\": \"" + coaPPM.LinePercent.ToString() + "\",\n \"ContractNumber\": \"" + coaPPM.ContractNumber + "\",\n  \"ProjectNumber\": \"" + coaPPM.ProjectNumber + "\",\n \"TaskNumber\": \"" + coaPPM.TaskNumber + "\",\n \"FundingSourceNumber\": \"" + coaPPM.FundingSourceNumber + "\",\n \"ExpenditureTypeId\": \"" + coaPPM.ExpenditureTypeId.ToString() + "\",\n \"ExpenditureOrganizationId\": \"" + coaPPM.ExpenditureOrganizationId + "\",\n \"ContextCategory\": \"" + coaPPM.ContextCategory + "\" \n  }\n  ]\n }\n ]\n}");

                        HttpContent model = new StringContent("{\n\"AssignmentNumber\": \"" + assignmentNumber + "\",\n \"PersonNumber\": \"" + personNumber + "\" ,\n \"LaborScheduleName\": \"" + coaPPM.LaborScheduleName + "\",\n    \"versions\": [\n {\n \"VersionName\": \"" + coaPPM.VersionName + "\",\n \"VersionComments\": \"" + coaPPM.VersionComments + "\",\n \"VersionStatusCode\": \"" + coaPPM.VersionStatusCode + "\",\n \"VersionStartDate\": \"" + coaPPM.StartDate + "\",\n " + distributionRules + "\n }\n ]\n}");

                        var header = new Dictionary<string, string>();
                        header.Add("Effective-Of", "RangeStartDate=" + coaPPM.StartDate);
                        header.Add("Rest-Framework-Version", "4");

                        var updateResponse = await ExecuteHttpRequest<ErpCoaPPMResponse>(endpoint.Method, path, logger, content: model, headers: header, apiCode: endpoint.APICode, doaCandidateId: doaCandidateId, useGzip: true);

                        if (updateResponse != null && updateResponse.LaborScheduleId != null)
                        {

                            for (int i = 0; i < updateResponse.Versions.Items[0].DistributionRules.Items.Length; i++)
                            {

                                await assignmentRepository.UpdateCoaPPMRuleLine(coaDetails.CoaPPMList[i].RuleLineId, updateResponse.Versions.Items[0].DistributionRules.Items[i].DistributionRuleId, updateResponse.Versions.Items[0].DistributionRules.Items[i].LineNumber);
                            }

                            // updating status in Database 
                            await assignmentRepository.UpdateCoaPPMVersion(coaPPM.CoaId, updateResponse.Versions.Items[0].VersionId, updateResponse.LaborScheduleId, "N");


                            // Setting version status as Active in Quantum
                            var statusResponse = await UpdateCoaPPMVersionStatus(doaCandidateId, (long)updateResponse.LaborScheduleId, (long)updateResponse.Versions.Items[0].VersionId);

                            if (statusResponse != null)
                            {
                                // updating status as Active in Database
                                await assignmentRepository.UpdateCoaPPMVersion(coaPPM.CoaId, updateResponse.Versions.Items[0].VersionId, updateResponse.LaborScheduleId, "A");

                            }

                            //} 
                        }


                        logger.LogDebug("Coa PPM updated successfully");

                        response.Response = updateResponse;
                        response.Message = "Coa PPM updated successfully.";
                        response.ResponseCode = HttpStatusCode.OK;

                    }

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error on Coa PPM: {ex}");

                response.Response = false;
                response.Message = "Error on Coa PPM: " + ex.Message;
                response.ResponseCode = HttpStatusCode.BadRequest;
            }

            return response;
        }

        private async Task<ReturnObject> UpdateCoaNonPPM(long DoaCandidateId)
        {

            var response = new ReturnObject();
            var isSuccess = true;

            try
            {

                var endpoint = ExternalIntegrationEndPoints.UpdateChartOfAccountsNonPPM;

                var coaDetails = await GetCoaNonPPMDetailsByDoaCandidateId(DoaCandidateId);

                var path = endpoint.PathFormat(workerHash.Trim(), personId.ToString().Trim());

                if (coaDetails != null && coaDetails.CoaNonPPMList.Count > 0)
                {

                    CoaNonPPM nonPPM = null;


                    for (var i = 0; i < coaDetails.CoaNonPPMList.Count; i++)
                    {
                        nonPPM = coaDetails.CoaNonPPMList[i];
                        HttpContent model = new StringContent("{\n\"PeiInformationCategory\": \"Interagency Costing\",\n \"PeiAttributeCategory\":null,\n  \"percentage\": \"" + nonPPM.Percentage + "\" ,\n \"serialNumber\": \"" + nonPPM.SerialNumber + "\",\n \"agency\": \"" + nonPPM.Agency + "\",\n  \"operatingUnit\": \"" + nonPPM.OperatingUnit + "\",\n  \"fund\": \"" + nonPPM.Fund + "\",\n  \"costCentre\": \"" + nonPPM.CostCentre + "\",\n  \"project\": \"" + nonPPM.Project + "\",\n  \"donor\": \"" + nonPPM.Donor + "\",\n\"interagency\": \"" + nonPPM.Interagency + "\", \n\"futuresegment\": \"" + nonPPM.Futuresegment + "\" \n}");

                        var header = new Dictionary<string, string>();
                        header.Add("Effective-Of", "RangeStartDate=" + nonPPM.StartDate);
                        header.Add("Rest-Framework-Version", "4");

                        var updateResponse = await ExecuteHttpRequest<ErpCoaNonPPMResponse>(endpoint.Method, path, logger, content: model, headers: header, apiCode: endpoint.APICode, doaCandidateId: doaCandidateId, useGzip: true);

                        if (updateResponse != null && updateResponse.PersonExtraInfoId != null)
                        {
                            await assignmentRepository.UpdateCoaNonPPMRuleLineHash(nonPPM.RuleLineId, updateResponse.Links[0].Href, updateResponse.SerialNumber);

                            // this should be called at last successful posting of Rule line
                            if (i == coaDetails.CoaNonPPMList.Count - 1)
                            {
                                await assignmentRepository.UpdateCoaNonPPMVersion(nonPPM.CoaId, updateResponse.PersonExtraInfoId);
                            }
                        }


                        logger.LogDebug("Coa Non PPM updated successfully");

                        response.Response = updateResponse;
                        response.Message = "Coa Non PPM updated successfully.";
                        response.ResponseCode = HttpStatusCode.OK;

                    }

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error on Coa Non PPM: {ex}");


                isSuccess = false;
                response.Response = false;
                response.Message = "Error on Coa Non PPM: " + ex.Message;
                response.ResponseCode = HttpStatusCode.BadRequest;
            }

            return response;
        }

        private async Task<ReturnObject> UpdateCoaNONQuantum(long DoaCandidateId)
        {

            var response = new ReturnObject();
            return response;
        }

        private async Task<ReturnObject> UpdateAgencyInfo()
        {
            var response = new ReturnObject();

            try
            {
                var endpoint = ExternalIntegrationEndPoints.UpdateAgency;

                var path = endpoint.PathFormat(workerHash, personId.ToString());

                HttpContent model = new StringContent("{\n    \"PeiInformationCategory\": \"UNV Specific Information\",\n    \"PeiAttributeCategory\": null,\n    \"agencyAccountNoProjectCode\": \"" + assignmentModel.AgencyAccountProjectCode + "\",\n    \"agencyReference\": \"" + assignmentModel.AgencyReference + "\",\n    \"unliquidatedObligation\": \"" + assignmentModel.UnliquidatedObligation + "\",\n    \"positionType\": \"" + assignmentModel.PositionType + "\",\n    \"assignmentNumber\": \"" + assignmentNumber + "\"\n}");

                var header = new Dictionary<string, string>();
                header.Add("Rest-Framework-Version", "4");
                var updateResponse = await ExecuteHttpRequest<AgencyResponseModel>(endpoint.Method, path, logger, content: model, apiCode: endpoint.APICode, doaCandidateId: doaCandidateId, headers: header, useGzip: true);

                logger.LogDebug("Agency updated successfully");

                response.Response = updateResponse;
                response.Message = "Agency updated successfully.";
                response.ResponseCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error on UpdateAgency: {ex}");

                response.Response = false;
                response.Message = "Error on UpdateAgency: " + ex.Message;
                response.ResponseCode = HttpStatusCode.BadRequest;
            }

            return response;

        }


        private async Task<ReturnObject> UpdateSalaryInfo(long doaCandidateId)
        {
            var response = new ReturnObject();

            logger.LogDebug($"GetSalaryBasisDetailsByDoaCandidateId: {0}", doaCandidateId);
            var salaryBasis = await GetSalaryBasisDetailsByDoaCandidateId(doaCandidateId);

            if (salaryBasis == null)
            {
                response.Message = " Salary Info not defined for this DOA Candidate Id :" + doaCandidateId;
                response.ResponseCode = HttpStatusCode.NoContent;
                response.Response = false;

                return response;
            }

            try
            {
                var endpoint = ExternalIntegrationEndPoints.UpdateSalary;
                var path = endpoint.Path;

                HttpContent model = new StringContent("{\n    \"ActionId\": \"" + salaryBasis.ActionId + "\",\n    \"AssignmentId\": " + assignmentId + ",\n    \"SalaryBasisId\": \"" + salaryBasis.SalaryBasisId + "\",\n    \"DateFrom\": \"" + effectiveDate + "\",\n    \"CurrencyCode\": \"" + salaryBasis.CurrencyCode + "\",\n    \"SalaryAmount\": \"" + salaryBasis.SalaryAmount + "\"\n}");

                var header = new Dictionary<string, string>();
                header.Add("Rest-Framework-Version", "4");
                var updateResponse = await ExecuteHttpRequest<AgencyResponseModel>(endpoint.Method, path, logger, content: model, apiCode: endpoint.APICode, doaCandidateId: this.doaCandidateId, headers: header, useGzip: true);

                logger.LogDebug("Salary Info updated successfully");

                response.Response = updateResponse;
                response.Message = " Salary Info updated successfully.";
                response.ResponseCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error on  Salary Info: {ex}");

                response.Response = false;
                response.Message = "Error on Salary Info: " + ex.Message;
                response.ResponseCode = HttpStatusCode.BadRequest;
            }

            return response;

        }

        private async Task<ReturnObject> UpdateDependentsInfo(long doaCandidateId)
        {
            var response = new ReturnObject();

            logger.LogDebug($"GetDependentDetailsByDoaCandidateId: {0}", doaCandidateId);
            dependentModel = await GetDependentDetailsByDoaCandidateId(doaCandidateId);


            if (dependentModel != null && dependentModel.DependentList != null && dependentModel.DependentList.Count > 0)
            {

                try
                {
                    var endpoint = ExternalIntegrationEndPoints.UpdateDependents;
                    var path = endpoint.Path;

                    Dependent dependent = null;

                    for (int i = 0; i < dependentModel.DependentList.Count; i++)
                    {
                        dependent = dependentModel.DependentList[i];
                        // HttpContent model = new StringContent("{\n \"DateOfBirth\": \"2001-01-21\",\n    \"names\": [\n        {\n        \"FirstName\": \"Tchala 02\",\n        \"LastName\": \"Rimsky-Korsakov 02\",\n        \"LegislationCode\": \"UN\"\n        }\n    ],\n    \"legislativeInfo\": [\n        {\n            \"LegislationCode\": \"UN\",\n            \"Gender\": \"M\",\n            \"MaritalStatus\":\"M\"\n        }\n    ],\n    \"contactRelationships\": [\n        {\n            \"ContactType\": \"UNCLS\",\n            \"EmergencyContactFlag\": true,\n            \"RelatedPersonNumber\": \"71400571\"\n        }\n    ]\n}");
                        HttpContent model = new StringContent("{\n  \"DateOfBirth\":\"" + dependent.DateOfBirth + "\",\n    \"names\": [\n        {\n        \"FirstName\": \"" + dependent.FirstName + "\",\n        \"LastName\": \"" + dependent.LastName + "\",\n        \"LegislationCode\": \"" + dependent.LegislationCode + "\"\n        }\n    ],\n    \"legislativeInfo\": [\n        {\n            \"LegislationCode\": \"" + dependent.LegislationCode + "\",\n             \"MaritalStatus\": \"" + dependent.MaritalStatus + "\",\n   \"Gender\": \"" + dependent.Gender + "\"\n        }\n    ],\n    \"contactRelationships\": [\n        {\n            \"ContactType\": \"" + dependent.ContactType + "\",\n            \"RelatedPersonNumber\": \"" + personNumber + "\",\n\t\t\t\"contactRelationshipsDFF\": [\n                {\n                    \"allowanceEligibleCheckbox\": \"" + dependent.AllowanceEligibleCheckbox + "\",\n                    \"allowanceEffectiveDate\": \"" + effectiveDate + "\",\n                                 \"EffectiveStartDate\": \"" + effectiveDate + "\"   ,\n                \"householdMember\": \"" + dependent.HouseholdMember + "\"\n                }\n            ]\n        }\n    ]\n}");

                        var header = new Dictionary<string, string>
                                {
                                    { "Effective-Of", "RangeStartDate=" + effectiveDate },
                                    { "Upsert-Mode", "false" },
                                };

                        var updateResponse = await ExecuteHttpRequest<DependentResponseModel>(endpoint.Method, path, logger, content: model, apiCode: endpoint.APICode, doaCandidateId: this.doaCandidateId, headers: header, useGzip: true);

                         logger.LogDebug("Dependent Info updated successfully");

                        if (updateResponse.IsNotNull() && updateResponse.links != null &&
                            updateResponse.links.Length > 0)
                        {
                            var hashLink = updateResponse.links.Where(l => l.Rel == "self").FirstOrDefault().Href;
                            /**** update ErpContactHash + ErPersonNumber***/
                            await assignmentRepository.UpdateDependentErpContactWithQuantumData(doaCandidateId, updateResponse.PersonNumber, hashLink, updateResponse.Names.FirstOrDefault()?.FirstName?.Replace(" ", string.Empty), updateResponse.Names.FirstOrDefault()?.LastName?.Replace(" ", string.Empty));
                        }
                            response.Response = updateResponse;
                            response.Message = " Dependent Info updated successfully.";
                            response.ResponseCode = HttpStatusCode.OK;

                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error on  Dependent Info: {ex}");

                    response.Response = false;
                    response.Message = "Error on Dependent Info: " + ex.Message;
                    response.ResponseCode = HttpStatusCode.BadRequest;
                }
            }
            else
            {
                response.ResponseCode = HttpStatusCode.NoContent;
                response.Response = true;
                response.Message = "No Dependent Info found for this DoaCandidateId : " + doaCandidateId;
            }

            return response;
        }





        private async Task<ReturnObject> UpdateSetExternalIdentifiers(long doaCandidateId)
        {
            var response = new ReturnObject();

            try
            {
                var endpoint = ExternalIntegrationEndPoints.SetExernalIdentifiers;

                var path = endpoint.PathFormat(workerHash, personId.ToString());

                var header = new Dictionary<string, string>();
                header.Add("Rest-Framework-Version", "4");
                HttpContent model = new StringContent("{\n    \"ExternalIdentifierNumber\": \"" + candidateId + "\",\n    \"ExternalIdentifierType\": " + "\"ROS\"" + ",\n    \"FromDate\": \"" + effectiveDate + "\",\n    \"AssignmentNumber\": \"" + assignmentNumber + "\",\n    \"Comments\": \"" + "" + "\"\n}");
                var updateResponse = await ExecuteHttpRequest<AgencyResponseModel>(endpoint.Method, path, logger, content: model, apiCode: endpoint.APICode, doaCandidateId: this.doaCandidateId, headers: header, useGzip: true);
                logger.LogDebug("External Identifier ROS Info updated successfully");

                model = new StringContent("{\n    \"ExternalIdentifierNumber\": \"" + doaCandidateId + "\",\n    \"ExternalIdentifierType\": " + "\"UUI\"" + ",\n    \"FromDate\": \"" + effectiveDate + "\",\n    \"AssignmentNumber\": \"" + assignmentNumber + "\",\n    \"Comments\": \"" + "" + "\"\n}");
                updateResponse = await ExecuteHttpRequest<AgencyResponseModel>(endpoint.Method, path, logger, content: model, apiCode: endpoint.APICode, doaCandidateId: this.doaCandidateId, headers: header, useGzip: true);
                logger.LogDebug("External Identifier UUI Info updated successfully");

                response.Response = updateResponse;
                response.Message = " External Identifier Info Setup successfully.";
                response.ResponseCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error on  External Identifier Info: {ex}");

                response.Response = false;
                response.Message = "Error on External Identifier Info: " + ex.Message;
                response.ResponseCode = HttpStatusCode.BadRequest;
            }



            return response;

        }

        private async Task<TResult> ExecuteHttpRequest<TResult>(HttpMethod method, string path, ILogger logger, string authorizationToken = null, string onBehalfOf = null, object content = null, NameValueCollection parameters = null, bool isAuthenticated = true, Dictionary<string, string> headers = null, string apiCode = null, long doaCandidateId = 0, bool useGzip = false)
        {
            var response = await ExecuteHttpRequest(method, path, logger, authorizationToken, onBehalfOf, content, parameters, isAuthenticated, headers, apiCode, doaCandidateId, useGzip);
            var strResponce = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                ErrorModel errorresult;
                try
                {
                    errorresult = JsonConvert.DeserializeObject<ErrorModel>(strResponce);
                }
                catch
                {
                    throw new Exception(" Error Details: " + strResponce);
                }

                throw new Exception("ErrorCode: " + errorresult.OErrorDetails[0].OErrorCode + " Error Details: " + errorresult.OErrorDetails[0].Detail);
            }

            return JsonConvert.DeserializeObject<TResult>(strResponce);
        }

        private HttpClient GetHttpClient(string authorizationToken, string onBehalfOf, bool isAuthenticated, bool useGzip = false)
        {
            var clientHandler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            var baseUrl = httpClient.BaseAddress;
            if (!string.IsNullOrEmpty(authorizationToken))
            {
                var client = useGzip ? new HttpClient
                {
                    BaseAddress = baseUrl,
                }
                : new HttpClient(clientHandler)
                {
                    BaseAddress = baseUrl,
                };
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizationToken.Replace("Bearer ", string.Empty));

                return client;
            }

            if (!isAuthenticated)
            {
                var client = useGzip ? new HttpClient
                    {
                        BaseAddress = baseUrl,
                    }
                    : new HttpClient(clientHandler)
                    {
                        BaseAddress = baseUrl,
                    };
                return client;
            }

            var gzipClient = new HttpClient(clientHandler)
            {
                BaseAddress = baseUrl,
            };
            gzipClient.DefaultRequestHeaders.Authorization = httpClient.DefaultRequestHeaders.Authorization;

            return useGzip ? gzipClient : httpClient;
        }

        private async Task<HttpResponseMessage> ExecuteHttpRequest(HttpMethod method, string path, ILogger logger, string authorizationToken = null, string onBehalfOf = null, object content = null, NameValueCollection parameters = null, bool isAuthenticated = true, Dictionary<string, string> headers = null, string apiCode = null, long doaCandidateId = 0, bool useGzip = false)
        {
            HttpResponseMessage response = null;
            string newTransctionId = "";
            dynamic temp;
            try
            {
                logger.LogInformation("Endpoint: {ENDPOINT}", path);
                var client = GetHttpClient(authorizationToken, onBehalfOf, isAuthenticated, useGzip);
                if (content.IsNotNull())
                {
                    HttpContent httpContent = null; // new StringContent(content.ToString(), Encoding.UTF8, "application/json");

                    if (content is HttpContent castedContent)
                    {
                        httpContent = castedContent;
                    }
                    else
                    {
                        httpContent = SerializeObject(content);
                    }


                    // inserting transaction
                    //this.LogAPITransaction(doaCandidateId, "", APIStep.Start, httpContent.ReadAsStringAsync().Result.ToString(), "", apiCode, "", out newTransctionId);
                    apiTransactionId = await LogAPITransaction(doaCandidateId, "", APIStep.Start, httpContent.ReadAsStringAsync().Result, "", apiCode, "");


                    response = await client.SendClientAsync(method: method, path: path, parameters: parameters, content: httpContent, headers: headers);

                    // response = await client.PostAsync(path, httpContent);


                }
                else
                {
                    // inserting transaction
                    //this.LogAPITransaction(doaCandidateId, "", APIStep.Start, path, "", apiCode, "", out newTransctionId);
                    apiTransactionId = await LogAPITransaction(doaCandidateId, "", APIStep.Start, path, "", apiCode, "");

                    response = await client.SendClientAsync(method: method, path: path, parameters: parameters);
                }

                if (response.IsSuccessStatusCode)
                {
                    temp = await LogAPITransaction(doaCandidateId, apiTransactionId, APIStep.End, "", response.Content.ReadAsStringAsync().Result, apiCode, "");

                    return response;
                }
                else
                {

                    var responseText = new StringBuilder("Requested endpoint: ");
                    responseText.Append(client.BaseAddress.ToString());
                    responseText.Append(path);
                    responseText.AppendLine("Response:");
                    responseText.AppendLine(response.Content.ReadAsStringAsync().Result);

                    await LogAPITransaction(doaCandidateId, apiTransactionId, APIStep.Error, "", responseText.ToString(), apiCode, APIErrorCodes.DuplicateRecord.ToString());

                    return response;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception: Method 'ExecuteHttpRequest': {EXCEPTION}", ex.ToString());

                temp = await LogAPITransaction(doaCandidateId, apiTransactionId, APIStep.Exception, "", ex.ToString(), apiCode, ex.ToString());


                throw;
            }
            finally
            {
                if (response != null && !response.IsSuccessStatusCode)
                {
                    logger.LogInformation(response.StatusCode.ToString());
                }
            }
        }


        private async Task<string> LogAPITransaction(long DoaCandidateId, string ApiId, APIStep step, string requestObject, string responseObject, string apiCode, string apiError, string action = null)
        {
            //string newApiId = "";
            if (step == APIStep.Start) // Before Calling an API
            {
                APITransaction apiTransaction = new APITransaction();
                apiTransaction.RequestObject = requestObject;
                apiTransaction.Id = "-1";
                apiTransaction.APIErrorCode = APIErrorCodes.NoError.ToString();
                apiTransaction.APIcode = apiCode;
                apiTransaction.APIStatusCode = APIStatusCodes.InProgress.ToString();
                apiTransaction.InitialStatusId = APIStatusCodes.InProgress.ToString();
                apiTransaction.LatestRequestObject = requestObject;
                apiTransaction.IsReTriggered = false;
                apiTransaction.ResponseObject = "";
                apiTransaction.LatestResponseObject = "";
                apiTransaction.DoaCandidateId = DoaCandidateId;


                var returnObject = await errorFrameworkRepository.CreateAPITransactionAsyc(apiTransaction, APIAction.Save.ToString());

                return returnObject.NewId;

            }

            if (step == APIStep.End) // After calling an API
            {
                APITransaction apiTransaction = new APITransaction();
                apiTransaction.RequestObject = "";
                apiTransaction.LatestRequestObject = "";
                apiTransaction.InitialStatusId = APIStatusCodes.InProgress.ToString();

                apiTransaction.ResponseObject = responseObject;
                apiTransaction.LatestResponseObject = responseObject;
                apiTransaction.APIStatusCode = APIStatusCodes.Success.ToString();
                apiTransaction.APIcode = apiCode;
                apiTransaction.APIErrorCode = APIErrorCodes.NoError.ToString();
                apiTransaction.Id = ApiId;
                if (apiError.Length != 0)
                    apiTransaction.APIErrorCode = APIErrorCodes.Exception.ToString();
                apiTransaction.DoaCandidateId = DoaCandidateId;

                await errorFrameworkRepository.CreateAPITransactionAsyc(apiTransaction, APIAction.Update.ToString());

                return ApiId;
            }
            if (step == APIStep.Result) // Before Calling an API
            {


                APITransaction apiTransaction = new APITransaction();
                apiTransaction.RequestObject = requestObject;
                apiTransaction.Id = apiTransactionId;
                apiTransaction.APIErrorCode = APIErrorCodes.DuplicateRecord.ToString();
                apiTransaction.APIcode = apiCode;
                apiTransaction.APIStatusCode = APIStatusCodes.Failure.ToString();
                apiTransaction.InitialStatusId = APIStatusCodes.InProgress.ToString();
                apiTransaction.LatestRequestObject = requestObject;
                apiTransaction.IsReTriggered = false;
                apiTransaction.ResponseObject = "";
                apiTransaction.LatestResponseObject = "";
                apiTransaction.DoaCandidateId = DoaCandidateId;
                if (apiError.IsNotNull())
                {
                    apiTransaction.APIErrorCode = apiError;
                }


                if (action.IsNull())
                {
                    action = APIAction.LogResult.ToString();
                }
                else
                {
                    apiTransaction.Id = Guid.Empty.ToString();
                }

                var returnObject = await errorFrameworkRepository.CreateAPITransactionAsyc(apiTransaction, action);

                return returnObject.NewId;

            }

            if (step == APIStep.Error) // Before Calling an API
            {
                APITransaction apiTransaction = new APITransaction();
                apiTransaction.RequestObject = requestObject;
                apiTransaction.Id = apiTransactionId;
                apiTransaction.APIErrorCode = APIErrorCodes.GeneralError.ToString();
                apiTransaction.APIcode = apiCode;
                apiTransaction.APIStatusCode = APIStatusCodes.Failure.ToString();
                apiTransaction.InitialStatusId = APIStatusCodes.InProgress.ToString();
                apiTransaction.LatestRequestObject = requestObject;
                apiTransaction.IsReTriggered = false;
                apiTransaction.ResponseObject = responseObject;
                apiTransaction.LatestResponseObject = responseObject;
                apiTransaction.DoaCandidateId = DoaCandidateId;

                var returnObject = await errorFrameworkRepository.CreateAPITransactionAsyc(apiTransaction, APIAction.LogError.ToString());

                return returnObject.NewId;

            }
            if (step == APIStep.Exception) // Before Calling an API
            {
                APITransaction apiTransaction = new APITransaction();
                apiTransaction.RequestObject = requestObject;
                apiTransaction.Id = apiTransactionId;
                apiTransaction.APIErrorCode = APIErrorCodes.Exception.ToString();
                apiTransaction.APIcode = apiCode;
                apiTransaction.APIStatusCode = APIStatusCodes.Failure.ToString();
                apiTransaction.InitialStatusId = APIStatusCodes.InProgress.ToString();
                apiTransaction.LatestRequestObject = requestObject;
                apiTransaction.IsReTriggered = false;
                apiTransaction.ResponseObject = "";
                apiTransaction.LatestResponseObject = "";
                apiTransaction.DoaCandidateId = DoaCandidateId;

                var returnObject = await errorFrameworkRepository.CreateAPITransactionAsyc(apiTransaction, APIAction.LogError.ToString());

                return returnObject.NewId;

            }
            return "";
        }

        private StringContent SerializeObject(object content)
        {
            var json = JsonConvert.SerializeObject(content, new JsonSerializerSettings
            {
                ContractResolver = content as DefaultContractResolver,
            });

            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private async Task<string> SetWorkerHash(WorkerLink[] links)
        {
            var workerLink = links.Where(l => l.Rel == "self" && l.Name == "workers").FirstOrDefault();
            var len = workerLink.Href.LastIndexOf('/');

            return workerLink.Href[(len + 1)..];
        }

        private async Task<string> SetAssignmentHash(WorkerLink[] links)
        {
            var assignmentLink = links.Where(l => l.Rel == "self" && l.Name == "assignments").FirstOrDefault();
            var len = assignmentLink.Href.LastIndexOf('/');

            return assignmentLink.Href[(len + 1)..];
        }

        private async Task<string> SetContractHash(WorkerLink[] links)
        {
            var contractLink = links.Where(l => l.Rel == "self" && l.Name == "contracts").FirstOrDefault();
            var len = contractLink.Href.LastIndexOf('/');

            return contractLink.Href[(len + 1)..];
        }

        private async Task<string> SetAssignmentHref(WorkerLink[] links)
        {
            var assignmentLink = links.Where(l => l.Rel == "self" && l.Name == "assignments").FirstOrDefault();
            var len = assignmentLink.Href.IndexOf("hcmRestApi");

            return assignmentLink.Href[(len)..];
        }

        /// <inheritdoc cref="IExternalIntegrationService.GetAssignmentDetails(long)"/>
        public async Task<ReturnObject> GetUVPDataForQuantumPosting(long doaCandidateId)
        {
            var response = new ReturnObject();
            try
            {
                var candidateModel = await GetCandidateById(doaCandidateId);

                var assignmentModel = await GetAssignmentById(doaCandidateId);
                if (assignmentModel != null)
                {
                    assignmentModel.IsHREntryDateAllowed = await IsHREntryDateAllowedById(doaCandidateId);
                    assignmentModel.QuantumLinkDisplay = await GetQuantumAssignmentLink(candidateModel.ErpPersonId, candidateModel.ErpAssignmentId, assignmentModel.HireDate);
                }

                var dependentModel = await GetDependentDetailsByDoaCandidateId(doaCandidateId);

                var salaryBasisModel = await GetSalaryBasisDetailsByDoaCandidateId(doaCandidateId);

                var fundingDetailsModel = await GetFundingDetailsByDoaId(doaCandidateId);

                var apiStatusDisplayModel = await GetAPIStatusDisplayByDoaId(doaCandidateId);

                var assignmentDetailsResponse = new AssignmentDetailsResponseModel
                {
                    Candidate = candidateModel,
                    Assignment = assignmentModel,
                    Dependents = dependentModel,
                    SalaryBasis = salaryBasisModel,
                    FundingDetails = fundingDetailsModel,
                    APIStatusDisplay = apiStatusDisplayModel,
                };

                response.Response = assignmentDetailsResponse;
                response.Message = string.Empty;
                response.ResponseCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"ERROR on 'GetAssignmentDetails': {ex}");

                response.Response = null;
                response.Message = ex.Message;
                response.ResponseCode = HttpStatusCode.BadRequest;
            }

            return response;
        }


        public async Task<ReturnObjectPayrollOTE> PostPayrollOTEDataToQuantum()
        {
            var returnObj = await PostPayrollOTEToQuantumAsyc();

            return returnObj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>ReturnObject</returns>
        private async Task<ReturnObjectPayrollOTE> PostPayrollOTEToQuantumAsyc()
        {
            var response = new ReturnObjectPayrollOTE();

            logger.LogDebug($"PostPayrollOTEToQuantum: {0}", DateTime.Today.ToString());
            var payrollOTEdata = await GetPayrollOTEDetailsForQuantumPosting();


            if (payrollOTEdata != null && payrollOTEdata.PayrollOTEList.Count > 0)
            {

                try
                {
                    var endpoint = ExternalIntegrationEndPoints.PostOneTimeEntryPayrollToQuantum;
                    var path = endpoint.Path;

                    PayrollOTE payrollEntry = null;

                    for (var i = 0; i < payrollOTEdata.PayrollOTEList.Count; i++)
                    {
                        payrollEntry = payrollOTEdata.PayrollOTEList[i];

                        var result = await IsDuplicateEntryCheckPayroll(payrollEntry.PaymentId);
                        if (result == false)
                        {
                            var header = new Dictionary<string, string>();

                            //  if (payrollEntry.EntryType.Trim() == "E")
                            if (!payrollEntry.ErpIsRecurring)
                            {
                                header = new Dictionary<string, string>
                                {
                                    { "Effective-Of", "RangeStartDate=" + payrollEntry.EffectiveStartDate.ToString("yyyy-MM-dd") },
                                    { "Upsert-Mode", "false" },
                                };
                            }
                            else
                            {
                                header = new Dictionary<string, string>
                                {
                                    { "Effective-Of", "RangeStartDate=" + payrollEntry.EffectiveStartDate.ToString("yyyy-MM-dd") + ";RangeEndDate=" + payrollEntry.EffectiveEndDate.ToString("yyyy-MM-dd") },
                                   // { "Effective-Of", "RangeEndDate=" + payrollEntry.EffectiveEndDate.ToString("yyyy-MM-dd") },
                                    { "Upsert-Mode", "false" },

                                };

                            }

                            HttpContent model = new StringContent("{\n    \"AssignmentId\": \"" + payrollEntry.ErpAssignmentNumber + "\",\n    \"ElementTypeId\": " + payrollEntry.Element_Type_Id + ",\n    \"PersonId\": \"" + payrollEntry.ErpPersonId + "\",\n    \"EntryType\": \"" + payrollEntry.EntryType.Trim() + "\",\n    \"CreatorType\": \"" + "H" + "\",\n     \"EntrySequence\": \"" + payrollEntry.EntrySequence + "\",\n  \"elementEntryValues\": [\n        {\n        \"InputValueId\": \"" + payrollEntry.Input_value_id + "\",\n        \"ScreenEntryValue\": \"" + payrollEntry.Input_value_amount + "\" \n },\n {\n        \"InputValueId\": \"" + payrollEntry.Currency_value_id + "\",\n        \"ScreenEntryValue\": \"" + payrollEntry.Currency_value_name + "\" \n }\n    ]\n}");
                            try
                            {
                                var updateResponse = await ExecuteHttpRequest<PayrollEntryResponse>(endpoint.Method, path, logger, content: model, apiCode: endpoint.APICode, doaCandidateId: payrollEntry.PaymentId, headers: header, useGzip: true);
                                if (updateResponse.ElementEntryId != null && updateResponse.ElementEntryId.Trim().Length > 0)
                                {
                                    await assignmentRepository.UpdatePayrollOTEWithQuantumEntryId(payrollEntry.PaymentId, Convert.ToInt64(updateResponse.ElementEntryId), true);
                                }
                                else
                                {
                                    await assignmentRepository.UpdatePayrollOTEWithQuantumEntryId(payrollEntry.PaymentId, 0, false);
                                }

                                response.Response = updateResponse;
                                response.Message = " Payroll Info updated successfully.";
                                response.ResponseCode = HttpStatusCode.OK;
                            }
                            catch
                            {
                                await assignmentRepository.UpdatePayrollOTEWithQuantumEntryId(payrollEntry.PaymentId, 0, false);
                            }

                            logger.LogDebug("Payroll entry posted  successfully");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error on  Payroll Info: {ex}");

                    response.Response = false;
                    response.Message = "Error on Payroll Info: " + ex.Message;
                    response.ResponseCode = HttpStatusCode.BadRequest;
                }
            }
            else
            {
                response.ResponseCode = HttpStatusCode.NoContent;
                response.Response = true;
                response.Message = "No New Payroll Entries found that needs to be Posted to Quantum.";
            }

            return response;
        }

        private async Task<FundingDetailsModel[]> GetFundingDetailsByDoaId(long doaCandidateId)
        {
            var model = await assignmentRepository.GetFundingDetailsByDoaIdAsync(doaCandidateId);
            return mapper.Map<FundingDetails[], FundingDetailsModel[]>(model);
        }


        public async Task<string> GetQuantumAssignmentLink(string PersonId, string AssignmentId, string EffectiveDate)
        {
            var commonUtil = new CommonUtilities(appSettings.Url);
            var result = await commonUtil.BuildQuantumAssignmentLink(PersonId, AssignmentId, EffectiveDate);
            return result;
        }

        private async Task<APIStatusDisplayModel[]> GetAPIStatusDisplayByDoaId(long doaCandidateId)
        {
            var model = await assignmentRepository.GetAPIStatusDisplayByDoaIdAsync(doaCandidateId);
            return mapper.Map<APIStatusDisplay[], APIStatusDisplayModel[]>(model);
        }

        private async Task<ReturnObject> UpdateCoaPPMVersionStatus(long doaCandidateId, long labourScheduleId, long versionId)
        {
            var response = new ReturnObject();

            try
            {
                var endpoint = ExternalIntegrationEndPoints.UpdateChartOfAccountsPPMVersionStatus;

                var path = endpoint.PathFormat(labourScheduleId.ToString(), versionId.ToString());

                var header = new Dictionary<string, string>();
                header.Add("Rest-Framework-Version", "4");

                HttpContent model = new StringContent("{\n   \"VersionStatusCode\":\"A\"\n}");
                var updateResponse = await ExecuteHttpRequest<AgencyResponseModel>(endpoint.Method, path, logger, content: model, apiCode: endpoint.APICode, doaCandidateId: this.doaCandidateId, headers: header);
                logger.LogDebug("PPM Version Status updated successfully");

                response.Response = updateResponse;
                response.Message = " PPM Version Status updated successfully.";
                response.ResponseCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error on PPM Version Status : {ex}");

                response.Response = false;
                response.Message = "PPM Version Status : " + ex.Message;
                response.ResponseCode = HttpStatusCode.BadRequest;
            }



            return response;

        }

        private async Task<DuplicateEntryResponseModel> IsDuplicateEntryCheck(long doaCandidateId)
        {
            var result = await assignmentRepository.IsDuplicateEntry(doaCandidateId);
            var response = new DuplicateEntryResponseModel
            {
                ResponseCode = result.Item1,
                ErpPersonNumber = result.Item2,
            };
            return response;
        }

        public async Task<ErpVolunteerHiringDataLogResponseModel> GetErpVolunteerHiringDataLog(long doaCandidateId)
        {
            var dataToPost = await GetUVPDataForQuantumPosting(doaCandidateId);
            var assignmentDetailsResponse = dataToPost.Response as AssignmentDetailsResponseModel;
            if (assignmentDetailsResponse.IsNotNull())
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
                };
                var data = assignmentDetailsResponse.GetErpVolunteerHiringDataPostedtoQuantum();
                var requestDependent = await assignmentRepository.GetErpDependentAPITransactions(doaCandidateId);
                var dependentTransactionModel = requestDependent.Select(x => new ErpDependentAPITransactionModel
                {

                    StatusCode = x.StatusCode,
                    Dependent = JsonConvert.DeserializeObject<DependentResponseModel>(x.RequestObject),
                    Pk_TransactionId = x.Pk_TransactionId,
                    CreatedOn = x.CreatedOn
                });

                if (data.DependentsDetails.IsNotNull() && data.DependentsDetails.Count > 0)
                {
                    foreach (var item in data.DependentsDetails)
                    {
                        var matchingTransaction = dependentTransactionModel
                            .Where(s => s.Dependent?.Names?.FirstOrDefault()?.FirstName?.Replace(" ", string.Empty) == item.FirstName?.Replace(" ", string.Empty) &&
                                        s.Dependent?.Names?.FirstOrDefault()?.LastName?.Replace(" ", string.Empty) == item.LastName?.Replace(" ", string.Empty))
                            .OrderByDescending(s => s.CreatedOn)
                            .FirstOrDefault();
                        if (matchingTransaction == null)
                        {
                            logger.LogError($"No match found for Dependent: {item?.FirstName} {item?.LastName}");
                        }
                        item.IntegrationStatusCode = matchingTransaction?.StatusCode;
                    }
                }
                var erpVolunteerHiringDataLog = new ErpVolunteerHiringDataLogResponseModel
                {
                    DoaCandidateId = doaCandidateId,
                    Data = JsonConvert.SerializeObject(data, settings),
                    CoaDataHCM = JsonConvert.SerializeObject(assignmentDetailsResponse.GetErpVolunteerHiringCoaDataHCMPostedtoQuantum(), settings),
                    CoaDataPPM = JsonConvert.SerializeObject(assignmentDetailsResponse.GetErpVolunteerHiringCoaDataPPMPostedtoQuantum(), settings),
                };
                return erpVolunteerHiringDataLog;
            }

            return null;
        }

        public async Task<ErpVolunteerHiringDataLogResponseModel> GetErpVolunteerManualHiringDataLog(long doaCandidateId)
        {
            var isManual = await assignmentRepository.AnyErpErrorFrameworkAPIManualTransactionForCandidate(doaCandidateId);
            if (isManual)
            {
                return await GetErpVolunteerHiringDataLog(doaCandidateId);
            }

            return null;
        }

        public async Task<ReturnObject> SaveApiTransaction(SaveStatusRequestModel saveStatusRequestModel)
        {
            var response = new ReturnObject();
            List<ErpErrorFrameworkAPITransactionManual> listErpErrorFrameworkAPITransactionManual = new List<ErpErrorFrameworkAPITransactionManual>();

            if (saveStatusRequestModel.apiCodes.Any(q => q.StartsWith(APICodes.ErpExternalIdentifier.ToString())))
            {
                saveStatusRequestModel.apiCodes.RemoveAll(x => x.StartsWith(APICodes.ErpExternalIdentifier.ToString()));
                saveStatusRequestModel.apiCodes.Add(APICodes.ErpExternalIdentifier.ToString());
            }
            foreach (var code in saveStatusRequestModel.apiCodes)
            {


                listErpErrorFrameworkAPITransactionManual.Add(new ErpErrorFrameworkAPITransactionManual
                {
                    ApiCode = code,
                    DoaCandidateId = saveStatusRequestModel.doaCandidateId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedUser = saveStatusRequestModel.userId,
                    StatusCode = "Success",
                    IsActive = true,
                    IsUpdated = true,
                });
            }
            var result = await assignmentRepository.SaveApiTransaction(listErpErrorFrameworkAPITransactionManual);
            response.Response = result;
            return response;
        }


        private async Task<bool> IsDuplicateEntryCheckPayroll(long paymentId)
        {
            var result = await assignmentRepository.IsDuplicateEntryPayroll(paymentId);
            return result;
        }


        public async Task<APIStatusDisplayModel[]> GetAPIStatusDisplayByDoaCandidateId(long doaCandidateId)
        {
           
            try
            {
                return await GetAPIStatusDisplayByDoaId(doaCandidateId);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error on GetAPIStatusDisplayByDoaId : {ex}");
            }
            return Array.Empty<APIStatusDisplayModel>();
        }

        public async Task<List<long>> GetErpErrorFrameworkAPITransactionRequest()
        {
            try
            {
                return await assignmentRepository.GetErpErrorFrameworkAPITransactions();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error on GetErpErrorFrameworkAPITransactionRequest : {ex}");
            }

            return new List<long>();
        }

        private async Task<ReturnObject> UpdateHireData(ManualHireRequestModel model, string logType)
        {
            var response = new ReturnObject();
            try
            {
                doaCandidateId = model.DoaCandidateId;

                var erpAssignment = await GetQuantumAssignmentRequest(model.AssignmentNumber);

                var arrAssignmentNumber = model.AssignmentNumber.Split('-');
                var personNumber = arrAssignmentNumber[0].Substring(1);
                var erpNames = await GetWorkerByPersonNumberRequest(personNumber);

                var assignmentModel = await GetAssignmentById(model.DoaCandidateId, model.ReasonCode == UpdateErpNumberReason.Migration.GetDescription());
                var candidateModel = await GetCandidateById(model.DoaCandidateId);

                if (erpAssignment.Items.Length == 0 || erpNames.Items.Length == 0)
                {
                    response.Response = false;
                    response.Message = "Record not in Quantum for Assignment Number : " + model.AssignmentNumber;
                    response.ResponseCode = HttpStatusCode.NotFound;
                }
                else if ((assignmentModel == null || candidateModel == null) && model.ReasonCode != UpdateErpNumberReason.Migration.GetDescription())
                {
                    response.Response = false;
                    response.Message = "Assignment Data is not available for Assignment Id : " + model.DoaCandidateId + ". Please contact UNV Support.";
                    response.ResponseCode = HttpStatusCode.NotFound;
                }
                else
                {
                    if (model.ReasonCode == UpdateErpNumberReason.GlobalTransfer.GetDescription())
                    {
                        var contract = await entityRepository.Where(new Specification<Contract>(x =>
                                                                            x.DoaCandidateId == model.DoaCandidateId &&
                                                                            (x.StatusCode == nameof(ContractStateMasterTableEnum.CONTRACT_CURRENT) ||
                                                                            x.StatusCode == nameof(ContractStateMasterTableEnum.CONTRACT_ACCEPTED))))
                                                                    .OrderByDescending(x => x.StartDate)
                                                                    .FirstOrDefaultAsync();
                        if (contract.IsNotNull() && contract.StartDate.HasValue)
                        {
                            assignmentModel.HireDate = contract.StartDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        }
                    }

                    var validateResponse = await ValidateQuantumAssignment(erpAssignment, erpNames, assignmentModel, candidateModel, model.AssignmentNumber, model.ReasonCode);
                    if (string.IsNullOrEmpty(validateResponse.validationResult))
                    {
                        // Update details
                        var requestmodel = new ManualHire
                        {
                            DoaCandidateId = model.DoaCandidateId,
                            ErpPersonId = validateResponse.erpModel.PersonId,
                            ErpPersonNumber = validateResponse.erpModel.PersonNumber,
                            ErpPersonHash = validateResponse.erpModel.WorkerHash,
                            ErpAssignmentId = validateResponse.erpModel.AssignmentId,
                            ErpAssignmentNumber = validateResponse.erpModel.AssignmentNumber,
                            ErpAssignmentHash = validateResponse.erpModel.AssignmentHash,
                            ErpContractId = validateResponse.erpModel.ContractId,
                            ErpContractNumber = validateResponse.erpModel.ContractNumber,
                            ErpContractHash = validateResponse.erpModel.ContractHash,
                            UserId = model.UserId,
                            ReasonCode = model.ReasonCode ?? string.Empty,
                            ReasonTableCode = model.ReasonTableCode ?? string.Empty,
                        };

                        var responseManualHire = await assignmentRepository.ManualHireAsync(requestmodel, logType);
                        if (responseManualHire > 0)
                        {
                            _ = await SaveErpVolunteerHiringDataPostedToQuantumLog();
                            cognitiveSearchManager?.EnqueueLowPriority<ISynchronizeCognitiveSearchJob>(x => x.UpdateEntity(requestmodel.DoaCandidateId.Value, typeof(DoaCandidate).FullName));
                            response.Response = true;
                            response.Message = "Quantum Data has been updated successfully in UVP.";
                            response.ResponseCode = HttpStatusCode.OK;
                        }
                        else
                        {
                            response.Response = false;
                            response.Message = "Some error occured while saving data in UVP, please try after some time. If issue still persists, kindly contact UNV Support.";
                            response.ResponseCode = HttpStatusCode.NotFound;
                        }
                    }
                    else
                    {
                        response.Response = false;
                        response.Message = validateResponse.validationResult;
                        response.ResponseCode = HttpStatusCode.NotFound;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"ERROR on 'ManualHire': {ex}");

                response.Response = null;
                response.Message = ex.Message;
                response.ResponseCode = HttpStatusCode.BadRequest;
            }

            return response;
        }

        /// <inheritdoc cref="IExternalIntegrationService.ManualHire(ManualHireRequestModel)"/>
        public async Task<ReturnObject> ManualHire(ManualHireRequestModel model)
        {
            return await UpdateHireData(model, "ErpManualHire");
        }

        /// <inheritdoc cref="IExternalIntegrationService.ManualHire(ManualHireRequestModel)"/>
        public async Task<ReturnObject> ManualUpdate(ManualHireRequestModel model)
        {
            return await UpdateHireData(model, "ErpManualUpdate");
        }

        /// <inheritdoc cref="IExternalIntegrationService.UpdateAssignmentStatus(long)"/>
        public async Task<ReturnObject> UpdateAssignmentStatus(long doaCandidateId)
        {
            var response = new ReturnObject();
            try
            {
                this.doaCandidateId = doaCandidateId;

                var model = await GetCandidateById(doaCandidateId);
                if (model != null)
                {
                    var erpAssignment = await GetQuantumAssignmentRequest(model.ErpAssignmentNumber);
                    if (erpAssignment.Items.Length == 0)
                    {
                        response.Response = false;
                        response.Message = "Record not in Quantum for Assignment Number : " + model.ErpAssignmentNumber;
                        response.ResponseCode = HttpStatusCode.NotFound;
                    }
                    else
                    {
                        var validateResponse = await ValidateQuantumAssignmentStatus(erpAssignment, model.ErpAssignmentNumber);
                        if (validateResponse)
                        {
                            var responseAssignment = await UpdateAssignmentStatusRequest();
                            if (responseAssignment != null && responseAssignment.AssignmentNumber != null)
                            {
                                logger.LogDebug("Assignment status has been updated successfully.");

                                response.Response = responseAssignment;
                                response.Message = "Assignment status has been updated successfully.";
                                response.ResponseCode = HttpStatusCode.OK;
                            }
                            else
                            {
                                logger.LogDebug($"Some error occured, please contact Administrator {0}.", model);

                                response.Response = null;
                                response.Message = "Some error occured, please contact Administrator.";
                                response.ResponseCode = HttpStatusCode.Ambiguous;
                            }
                        }
                        else
                        {
                            response.Response = null;
                            response.Message = "No Active Payroll Assignment status is available, hence cannot update to Suspend Payroll status.";
                            response.ResponseCode = HttpStatusCode.NotFound;
                        }
                    }
                }
                else
                {
                    response.Response = null;
                    response.Message = "Assignment Data is not available for Assignment Id : " + doaCandidateId + ". Please contact UNV Support.";
                    response.ResponseCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"ERROR on 'UpdateAssignmentStatus': {ex}");

                response.Response = null;
                response.Message = ex.Message;
                response.ResponseCode = HttpStatusCode.BadRequest;
            }

            return response;
        }

        private async Task<ReturnObject> SetErpPayrollRelationship()
        {

            var response = new ReturnObject();

            string payrollId = "";


            if (volType == "INT")
            {
                payrollId = internationalPayrollId;
            }
            else
            {
                payrollId = nationalPayrollId;
            }

            //this.volType = "INT";
            //this.assignmentNumber = "E71402980";
            //this.effectiveDate = "2023-01-17";
            try
            {
                var endpoint = ExternalIntegrationEndPoints.GetPayrollRelationship;

                var header = new Dictionary<string, string>();
                header.Add("Rest-Framework-Version", "4");
                header.Add("Effective-Of", "RangeStartDate=" + effectiveDate);

                var path = endpoint.PathFormat(assignmentNumber);

                var responsePayroll = await ExecuteHttpRequest<PayrollAssignmentResponse>(endpoint.Method, path, logger, headers: header, apiCode: endpoint.APICode, doaCandidateId: doaCandidateId, useGzip: true);

                if (responsePayroll != null && responsePayroll.Items.Length > 0 && responsePayroll.Items[0].PayrollAssignments != null && responsePayroll.Items[0].PayrollAssignments.Length > 0)
                {

                    string setPayrollURL = await GetSetPayrollRelationshipURL(responsePayroll.Items[0].PayrollAssignments[0].links);

                    setPayrollURL = setPayrollURL.Substring(setPayrollURL.IndexOf("/hcmRestApi"));

                    HttpContent model = new StringContent("{\n    \"PayrollId\": \"" + payrollId + "\",\n    \"StartDate\": \"" + effectiveDate + "\",\n    \"EndDate\": \"4712-12-31\",\n    \"TimeCardRequired\": null ,\n    \"OverridingPeriodId\": null,\n     \"EffectiveStartDate\": \"" + effectiveDate + "\",\n  \"EffectiveEndDate\": \"" + "4712-12-31" + "\"\n }");

                    var updateResponse = await ExecuteHttpRequest<PayrollRelationshipResponse>(HttpMethod.Post, setPayrollURL, logger, content: model, apiCode: APICodes.ErpSetPayrollRelationship.ToString(), doaCandidateId: doaCandidateId, headers: header, useGzip: true);
                    logger.LogDebug(" PayrollRelationship updated successfully");

                    response.Response = updateResponse;
                    response.Message = " PayrollRelationship updated successfully.";
                    response.ResponseCode = HttpStatusCode.OK;


                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $" PayrollRelationship error : {ex}");


            }


            return response;

        }


        private async Task<string> GetSetPayrollRelationshipURL(WorkerLink[] links)
        {
            var workerLink = links.Where(l => l.Rel == "child" && l.Name == "assignedPayrolls").FirstOrDefault();

            return workerLink.Href;
        }

        private async Task<PersonNumberResponse> GetWorkerByPersonNumberRequest(string personNumber)
        {
            var endpoint = ExternalIntegrationEndPoints.GetPersonNumber;
            var path = endpoint.PathFormat("\'" + personNumber.ToUpper() + "\'");

            var response = await ExecuteHttpRequest<PersonNumberResponse>(endpoint.Method, path, logger, apiCode: endpoint.APICode, doaCandidateId: doaCandidateId);
            return response;
        }

        private async Task<WorkerResponseModel> GetQuantumAssignmentRequest(string assignmentNumber)
        {
            var endpoint = ExternalIntegrationEndPoints.GetQuantumAssignment;
            var path = endpoint.PathFormat("\'" + assignmentNumber.ToUpper() + "\'");

            var response = await ExecuteHttpRequest<WorkerResponseModel>(endpoint.Method, path, logger, apiCode: endpoint.APICode, doaCandidateId: doaCandidateId);
            return response;
        }

        private async Task<(string validationResult, ErpManualSaveModel erpModel)> ValidateQuantumAssignment(WorkerResponseModel erpAssignment, PersonNumberResponse erpPersonNumber, AssignmentModel assignmentModel, CandidateModel candidateModel, string assignmentNumber, string reasonCode)
        {
            var response = new StringBuilder();
            var erpModelResult = new ErpManualSaveModel();
            var hasFirstName = false;
            var hasLastName = false;
            var hasDateOfBirth = false;
            var hasDutyStation = false;
            var hasAssignmentType = false;
            var hasGradeLadderName = false;
            var hasHireDate = false;

            foreach (var personNumber in erpPersonNumber.Items)
            {
                var firstName = personNumber.names.Items.Where(c => c.FirstName.ToLower().Trim() == candidateModel.FirstName.ToLower().Trim()).FirstOrDefault();
                if (firstName != null)
                {
                    hasFirstName = true;
                }

                var lastName = personNumber.names.Items.Where(c => c.LastName.ToLower().Trim() == candidateModel.LastName.ToLower().Trim()).FirstOrDefault();
                if (lastName != null)
                {
                    hasLastName = true;
                }

                if (personNumber.DateOfBirth == candidateModel.DateOfBirth)
                {
                    hasDateOfBirth = true;
                }
            }

            if ((hasFirstName && hasLastName && hasDateOfBirth) || reasonCode == UpdateErpNumberReason.GlobalTransfer.GetDescription())
            {
                foreach (var assignment in erpAssignment.Items)
                {
                    foreach (var workRelationship in assignment.WorkRelationships.Items)
                    {
                        var assignmentDetails = workRelationship.Assignments.Items.Where(c => c.AssignmentNumber.ToLower().Trim() == assignmentNumber.ToLower().Trim()).FirstOrDefault();
                        if (assignmentDetails != null)
                        {
                            if (!string.IsNullOrEmpty(assignmentModel.HireDate) && !string.IsNullOrEmpty(workRelationship.StartDate) &&
                                workRelationship.StartDate.ToLower().Trim() == assignmentModel.HireDate.ToLower().Trim())
                            {
                                hasHireDate = true;
                            }
                            else
                            {
                                hasHireDate = false;
                            }

                            if (hasHireDate || reasonCode == UpdateErpNumberReason.Migration.GetDescription())
                            {
                                if (assignmentDetails.LocationCode.ToLower().Trim() == assignmentModel.DutyStationCode.ToLower().Trim())
                                {
                                    hasDutyStation = true;
                                }
                                else
                                {
                                    hasDutyStation = false;
                                }

                                if (assignmentDetails.UserPersonType.ToLower().Trim() == assignmentModel.UserPersonType.ToLower().Trim())
                                {
                                    hasAssignmentType = true;
                                }
                                else
                                {
                                    hasAssignmentType = false;
                                }

                                if (assignmentDetails.GradeLadderName.ToLower().Trim() == assignmentModel.GradeLadderName.ToLower().Trim())
                                {
                                    hasGradeLadderName = true;
                                }
                                else
                                {
                                    hasGradeLadderName = false;
                                }

                                if (hasDutyStation && hasAssignmentType && hasGradeLadderName)
                                {
                                    erpModelResult.PersonId = assignment.PersonId;
                                    erpModelResult.PersonNumber = assignment.PersonNumber;
                                    erpModelResult.WorkerHash = await SetWorkerHash(assignment.links);

                                    erpModelResult.AssignmentId = assignmentDetails.AssignmentId;
                                    erpModelResult.AssignmentNumber = assignmentDetails.AssignmentNumber;
                                    erpModelResult.AssignmentHash = await SetAssignmentHash(assignmentDetails?.Links);

                                    erpModelResult.ContractId = assignmentDetails.ContractId;
                                    erpModelResult.ContractNumber = assignmentDetails.ContractNumber;
                                    erpModelResult.ContractHash = workRelationship.Contracts == null ? erpModelResult.AssignmentHash : await SetContractHash(workRelationship.Contracts.Items.Where(x => x.ContractId == assignmentDetails.ContractId).FirstOrDefault().Links);

                                    break;
                                }

                                if (!hasDutyStation)
                                {
                                    response.AppendLine("Duty station not matched.");
                                }

                                if (!hasAssignmentType)
                                {
                                    response.AppendLine("Assignment type not matched.");
                                }

                                if (!hasGradeLadderName)
                                {
                                    response.AppendLine("Volunteer type not matched.");
                                }
                            }
                            else
                            {
                                if (reasonCode != UpdateErpNumberReason.Migration.GetDescription())
                                {
                                    response.AppendLine("Hire date not matched.");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (!hasFirstName)
                {
                    response.AppendLine("First name not matched.");
                }

                if (!hasLastName)
                {
                    response.AppendLine("Last name not matched.");
                }

                if (!hasDateOfBirth)
                {
                    response.AppendLine("Date of birth not matched.");
                }
            }

            if (!response.ToString().IsNullOrEmpty())
            {
                response.Insert(0, "For the given Assignment Number, following did not match:");
                response.AppendLine();
            }

            return (response.ToString(), erpModelResult);
        }

        private async Task<bool> ValidateQuantumAssignmentStatus(WorkerResponseModel erpAssignment, string assignmentNumber)
        {
            var hasStatusActiveProcess = false;

            foreach (var assignment in erpAssignment.Items)
            {
                foreach (var workRelationship in assignment.WorkRelationships.Items)
                {
                    var assignmentDetails = workRelationship.Assignments.Items.Where(c => c.AssignmentNumber.ToLower().Trim() == assignmentNumber.ToLower().Trim()).FirstOrDefault();
                    if (assignmentDetails != null)
                    {
                        if (assignmentDetails.AssignmentStatusTypeCode == "ACTIVE_PROCESS")
                        {
                            assignmentHref = await SetAssignmentHref(assignmentDetails?.Links);

                            hasStatusActiveProcess = true;
                        }

                        break;
                    }
                }
            }

            return hasStatusActiveProcess;
        }

        private async Task<AssignmentResponseModel> UpdateAssignmentStatusRequest()
        {
            var endpoint = ExternalIntegrationEndPoints.UpdateAssignmentStatus;
            var path = endpoint.Path.Replace(endpoint.Path, assignmentHref);

            var header = new Dictionary<string, string>
            {
                { "Effective-Of", "RangeMode=UPDATE;RangeStartDate=" + DateTime.Now.ToString("yyyy-MM-dd") },
            };

            var body = new
            {
                ActionCode = "ASG_CHANGE",
                ReasonCode = "UPDATE_ASSIGNMENT_STATUS",
                AssignmentStatusTypeCode = "SUSPEND_PROCESS",
            };

            HttpContent model = new StringContent(JsonConvert.SerializeObject(body));

            var response = await ExecuteHttpRequest<AssignmentResponseModel>(endpoint.Method, path, logger, content: model, headers: header, apiCode: endpoint.APICode, doaCandidateId: doaCandidateId);
            return response;
        }

        private async Task<bool> SaveErpVolunteerHiringDataPostedToQuantumLog()
        {
            var data = await GetErpVolunteerHiringDataLog(doaCandidateId);
            if (data.IsNotNull())
            {
                var erpData = new ErpVolunteerHiringDataPostedtoQuantumLog
                {
                    DoaCandidateId = data.DoaCandidateId,
                    Data = data.Data,
                    CoaDataHCM = data.CoaDataHCM,
                    CoaDataPPM = data.CoaDataPPM,
                };
                entityRepository.Add(erpData);

                await unitOfWork.CommitAsync();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Executes the integration request for a stored procedure or view based on external client configuration.
        /// </summary>
        /// <param name="model">The request model containing code, input parameters, user ID, and role ID.</param>
        /// <returns>A JSON serialized result from the procedure or view.</returns>
        public async Task<string> GetUVPDataForExternalAppAsync(ExternalIntegerationRequestModel model)
        {
            try
            {
                // Get client configuration and validate role access
                var config = await this.GetClientConfigurationAsync(model.Code);
                await this.EnsureRoleAccessAsync(config.UniqueCode, model.RoleId);

                // Parse and validate input parameters against configuration
                var inputDict = ParseInputParams(model.InputParams);
                ValidateInputAgainstConfig(config.InputParameters, inputDict, config.CallingRoutineType);

                // Log request, execute database routine (stored procedure or view), and log response
                var logId = await this.LogInputAsync(config.Id, model.InputParams, model.UserId);
                var result = await this.assignmentRepository.ExecuteRoutineAsync(config, inputDict);
                await this.UpdateLogOutputAsync(logId, result, model.UserId);

                // Serialize and return the result
                this.logger.LogDebug("Successful data integration for Code: {Code}", model.Code);
                var list = result.Select(row => row as IDictionary<string, object>).ToList();
                return JsonSerializer.Serialize(list);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Unexpected error during UVP external data fetch for Code: {Code}", model.Code);
                throw new BusinessException($"Failed to fetch UVP data for external integration. Code: {ex.Message}");
            }
        }

        private async Task<ExternalClientConfig> GetClientConfigurationAsync(string code)
        {
            var config = await this.entityRepository
                .Where(new Specification<ExternalClientConfig>(x => x.UniqueCode == code && x.IsActive))
                .FirstOrDefaultAsync() ?? throw new InvalidOperationException("Invalid configuration.");

            return config;
        }

        /// <summary>
        /// Ensures that the specified role has access to the external integration endpoint.
        /// </summary>
        /// <param name="uniqueCode">The unique code identifying the endpoint configuration.</param>
        /// <param name="roleId">The role ID requesting access.</param>
        /// <exception cref="NotAccessBusinessException">Thrown when access is denied for the role.</exception>
        private async Task EnsureRoleAccessAsync(string uniqueCode, int roleId)
        {
            var accessAllowed = await this.entityRepository
                .Where(new Specification<ExternalClientRoleMapping>(
                    x => x.ExternalClientConfig.UniqueCode == uniqueCode &&
                         x.RoleCode == roleId &&
                         x.IsActive &&
                         x.IsAccessProvided))
                .FirstOrDefaultAsync();

            if (accessAllowed == null)
            {
                throw new NotAccessBusinessException("Access denied for this role and code.");
            }
        }

        /// <summary>
        /// Logs the input parameters of the external request into the database.
        /// </summary>
        /// <param name="configId">The configuration ID being used.</param>
        /// <param name="inputJson">The input JSON string received in the request.</param>
        /// <param name="userId">The ID of the user making the request.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task<long> LogInputAsync(int configId, string inputJson, long userId)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var log = new ExternalClientEndpointCallingLog
            {
                ExternalClientConfigId = configId,
                InputReceived = inputJson,
                InputReceivedTimeStamp = now,
                CreatedDate = now,
                CreatedBy = userId.ToString(),
                IsActive = true,
            };

            this.entityRepository.Add(log);
            await this.unitOfWork.CommitAsync();
            return log.Id;
        }

        /// <summary>
        /// Updates the log with the output returned from the stored procedure or view execution.
        /// </summary>
        /// <param name="logId">The ID of the log to update.</param>
        /// <param name="result">The result data to be logged.</param>
        /// <param name="userId">The ID of the user who initiated the request.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task UpdateLogOutputAsync(long logId, IEnumerable<dynamic> result, long userId)
        {
            var log = await this.entityRepository
                .Where(new Specification<ExternalClientEndpointCallingLog>(x => x.Id == logId))
                .FirstOrDefaultAsync();

            if (log != null)
            {
                var now = DateTime.UtcNow.ToLocalTime();
                log.OutputSent = JsonSerializer.Serialize(result);
                log.OutputSentTimeStamp = now;
                log.UpdatedDate = now;
                log.UpdatedBy = userId.ToString();

                await this.unitOfWork.CommitAsync();
            }
        }

        /// <summary>
        /// Validates that the request parameters match the configuration parameters.
        /// </summary>
        /// <param name="configInputParamsJson">JSON string containing expected parameter definitions from configuration.</param>
        /// <param name="requestParams">Dictionary of actual parameters received in the request.</param>
        /// <param name="callingRoutineType">The type of routine being called (e.g., "VIEW", "PROCEDURE").</param>
        /// <exception cref="InvalidOperationException">Thrown when validation fails due to missing parameters, type mismatches, or configuration issues.</exception>
        private static void ValidateInputAgainstConfig(string configInputParamsJson, Dictionary<string, object> requestParams, string callingRoutineType)
        {
            // Parse configuration parameters and handle VIEW type special case
            var inputParameters = ParseInputParameters(configInputParamsJson);
            if (inputParameters.Count == 0 && string.Equals(callingRoutineType, "VIEW", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (inputParameters.Count == 0)
            {
                throw new InvalidOperationException("No input parameters defined in configuration.");
            }

            // Validate parameter configuration integrity
            ValidateParameterTypesPresence(inputParameters);

            // Compare expected vs actual parameters and validate types
            var expectedNames = GetExpectedNames(inputParameters);
            var requestNames = requestParams.Keys.Select(k => k.Trim().ToLower()).ToHashSet();
            ValidateMissingAndExtraParams(expectedNames, requestNames);
            ValidateTypesMatch(inputParameters, requestParams);
        }

        /// <summary>
        /// Parses a JSON string containing input parameter definitions into a list of parameter models.
        /// </summary>
        /// <param name="json">The JSON string containing parameter definitions from configuration.</param>
        /// <returns>A list of InputParametersModel objects, or an empty list if JSON is null/empty.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the JSON format is invalid.</exception>
        private static List<InputParametersModel> ParseInputParameters(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<InputParametersModel>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<InputParametersModel>>(json, JsonOptions)
                       ?? new List<InputParametersModel>();
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Invalid input parameter format in configuration.", ex);
            }
        }

        /// <summary>
        /// Validates that all input parameters in the configuration have a type specified.
        /// </summary>
        /// <param name="inputParameters">List of input parameter definitions to validate.</param>
        /// <exception cref="InvalidOperationException">Thrown when any parameter is missing a type definition.</exception>
        private static void ValidateParameterTypesPresence(List<InputParametersModel> inputParameters)
        {
            var missingTypeParam = inputParameters.FirstOrDefault(p => string.IsNullOrWhiteSpace(p.Type));
            if (missingTypeParam != null)
            {
                throw new InvalidOperationException(
                    $"Invalid configuration: missing type for parameter: {missingTypeParam.Name}");
            }
        }

        private static HashSet<string> GetExpectedNames(List<InputParametersModel> inputParameters) => inputParameters
                .Where(p => !string.IsNullOrWhiteSpace(p.Name) && !string.IsNullOrWhiteSpace(p.Type))
                .Select(p => p.Name.Trim().ToLower())
                .ToHashSet();

        /// <summary>
        /// Validates that all required parameters are present and no unexpected parameters exist.
        /// </summary>
        /// <param name="expectedNames">Set of expected parameter names.</param>
        /// <param name="requestNames">Set of actual parameter names from the request.</param>
        /// <exception cref="InvalidOperationException">Thrown when required parameters are missing or unexpected parameters are present.</exception>
        private static void ValidateMissingAndExtraParams(HashSet<string> expectedNames, HashSet<string> requestNames)
        {
            var missingParams = expectedNames.Except(requestNames).ToList();
            if (missingParams.Count > 0)
            {
                throw new InvalidOperationException($"Missing required parameters: {string.Join(", ", missingParams)}");
            }

            var extraParams = requestNames.Except(expectedNames).ToList();
            if (extraParams.Count > 0)
            {
                throw new InvalidOperationException($"Unexpected parameters in request: {string.Join(", ", extraParams)}");
            }
        }

        /// <summary>
        /// Validates that input parameters match the expected types defined in configuration.
        /// </summary>
        /// <param name="inputParameters">The expected parameter definitions from configuration.</param>
        /// <param name="requestParams">The actual parameters received in the request.</param>
        /// <exception cref="InvalidOperationException">Thrown when parameter types don't match.</exception>
        private static void ValidateTypesMatch(List<InputParametersModel> inputParameters, Dictionary<string, object> requestParams)
        {
            // Check each configured parameter against the actual request values
            foreach (var paramDefinition in inputParameters)
            {
                var paramName = paramDefinition.Name.Trim();
                var paramType = paramDefinition.Type.Trim();

                // Find matching parameter in request (case-insensitive)
                var match = requestParams.FirstOrDefault(kvp => string.Equals(kvp.Key, paramName, StringComparison.OrdinalIgnoreCase));
                if (match.Key == null)
                {
                    continue;
                }

                // Validate type compatibility
                var actualType = GetTypeAsString(match.Value);
                if (!string.Equals(paramType, actualType, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"Type mismatch for parameter '{paramName}': expected '{paramType}', got '{actualType}'");
                }
            }
        }

        /// <summary>
        /// Determines the string representation of a parameter's data type.
        /// </summary>
        /// <param name="value">The parameter value to check.</param>
        /// <returns>A string representing the data type (int, bigint, varchar, bit, or unknown).</returns>
        private static string GetTypeAsString(object value) => value switch
        {
            int => "int",
            long => "bigint",
            string => "varchar",
            bool => "bit",
            JsonElement el when el.ValueKind == JsonValueKind.Number => "bigint",
            JsonElement el when el.ValueKind == JsonValueKind.String => "varchar",
            JsonElement el when el.ValueKind == JsonValueKind.True || el.ValueKind == JsonValueKind.False => "bit",
            _ => "unknown"
        };

        /// <summary>
        /// Parses a JSON string into a dictionary of input parameters.
        /// </summary>
        /// <param name="json">The JSON string containing input parameters.</param>
        /// <returns>A dictionary with parameter names as keys and their values as objects.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the JSON format is invalid.</exception>
        private static Dictionary<string, object> ParseInputParams(string json)
        {
            if (string.IsNullOrWhiteSpace(json) || json.Trim() == "[]")
            {
                return new Dictionary<string, object>();
            }

            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                       ?? new Dictionary<string, object>();
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Invalid input parameters format. Expected a JSON object.", ex);
            }
        }
    }
}
