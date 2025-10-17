namespace UVP.ExternalIntegration.Business.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using global::UVP.ExternalIntegration.Business.Interfaces;
    using global::UVP.ExternalIntegration.Business.Mapper.ResponseMapper.Interfaces;
    using global::UVP.ExternalIntegration.Domain.Entity.Integration;
    using global::UVP.ExternalIntegration.Domain.Entity.SystemsIntegration;
    using global::UVP.ExternalIntegration.Domain.Repository.Interfaces;
    using Newtonsoft.Json.Linq;
    using Serilog;

    /// <summary>
    /// Unified result mapper that handles all integration systems using strategy pattern.
    /// No system-specific logic here - all customization via IResultMappingStrategy implementations.
    /// </summary>
    public class UnifiedResultMapperService : IResultMapperService
    {
        private readonly IGenericRepository<DoaCandidateClearances> _clearancesRepo;
        private readonly IGenericRepository<DoaCandidateClearancesOneHR> _clearancesOneHRRepo;
        private readonly IGenericRepository<IntegrationInvocationLog> _invocationLogRepo;
        private readonly IGenericRepository<Candidate> _doaCandidateRepo;
        private readonly IResultMappingStrategyFactory _strategyFactory;
        private readonly IResultFieldExtractor _fieldExtractor;
        private readonly ILogger _logger = Log.ForContext<UnifiedResultMapperService>();

        public UnifiedResultMapperService(
            IGenericRepository<DoaCandidateClearances> clearancesRepo,
            IGenericRepository<DoaCandidateClearancesOneHR> clearancesOneHRRepo,
            IGenericRepository<IntegrationInvocationLog> invocationLogRepo,
            IGenericRepository<Candidate> doaCandidateRepo,
            IResultFieldExtractor fieldExtractor,
            IResultMappingStrategyFactory strategyFactory)
        {
            _clearancesRepo = clearancesRepo;
            _clearancesOneHRRepo = clearancesOneHRRepo;
            _invocationLogRepo = invocationLogRepo;
            _doaCandidateRepo = doaCandidateRepo;
            _fieldExtractor = fieldExtractor;
            _strategyFactory = strategyFactory;
        }

        public async Task ProcessResponseAsync(IntegrationInvocation invocation, string response, string integrationType)
        {
            var strategy = _strategyFactory.GetStrategy(integrationType);
            if (strategy == null)
            {
                _logger.Warning("No strategy found for integration type: {Type}", integrationType);
                return;
            }

            var operation = invocation.IntegrationOperation?.ToUpperInvariant();

            switch (operation)
            {
                case "CREATE_CLEARANCE_REQUEST":
                    await HandleCreateClearanceAsync(invocation, response, strategy);
                    break;

                case "GET_CLEARANCE_STATUS":
                    await HandleStatusCheckAsync(invocation, response, strategy);
                    break;

                case "ACKNOWLEDGE_RESPONSE":
                    await HandleAcknowledgeAsync(invocation, strategy);
                    break;

                default:
                    _logger.Warning("Unknown operation: {Operation} for {Type}", operation, integrationType);
                    break;
            }
        }

        private async Task HandleCreateClearanceAsync(IntegrationInvocation invocation, string response, IResultMappingStrategy strategy)
        {
            var requestId = strategy.ExtractRequestId(response);
            if (string.IsNullOrWhiteSpace(requestId))
            {
                _logger.Error("[{System}] CREATE: Failed to extract request ID from response", strategy.SystemCode);
                return;
            }

            var (doaCandidateId, candidateId) = await ResolveIdsFromInvocationAsync(invocation);

            _logger.Information("[{System}] CYCLE 1 - CREATE: RequestId={RequestId}", strategy.SystemCode, requestId);

            // Create OneHR tracking record
            var oneHr = new DoaCandidateClearancesOneHR
            {
                DoaCandidateId = doaCandidateId,
                CandidateId = candidateId,
                DoaCandidateClearanceId = requestId,
                RequestedDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                IsCompleted = false,
                Retry = 0
            };
            await _clearancesOneHRRepo.AddAsync(oneHr);
            await _clearancesOneHRRepo.SaveChangesAsync();

            //Create clearance summary record
            var clearance = (await _clearancesRepo.FindAsync(
                    c => c.DoaCandidateId == doaCandidateId &&
                    c.RecruitmentClearanceCode == "MED_CLEAR"))
                    .FirstOrDefault();

            // Update existing record
            clearance.RequestedDate = DateTime.UtcNow;
            clearance.StatusCode = "CLEARANCE_REQUESTED";
            clearance.LinkDetailRemarks = $"clearanceRequestId={requestId}";
            clearance.UpdatedDate = DateTime.UtcNow;
            clearance.CompletionDate = null;
            clearance.Outcome = null;
            await _clearancesRepo.UpdateAsync(clearance);
            await _clearancesRepo.SaveChangesAsync();

            //var clearance = new DoaCandidateClearances
            //{
            //    DoaCandidateId = doaCandidateId,
            //    RecruitmentClearanceCode = strategy.SystemCode,
            //    RecruitmentClearanceTableCode = "RecruitmentClearance",
            //    RequestedDate = DateTime.UtcNow,
            //    StatusCode = "CLEARANCE_REQUESTED",
            //    LinkDetailRemarks = $"clearanceRequestId={requestId}",
            //    CreatedDate = DateTime.UtcNow,
            //    UpdatedDate = DateTime.UtcNow
            //};

            //var clearance = new DoaCandidateClearanceModel
            //{
            //    DoaCandidateId = doaCandidateId,
            //    RecruitmentClearance = new MasterTableValueResultModel
            //    {
            //        Value = new MasterTableValue
            //        {
            //            Code = strategy.SystemCode,
            //            TableCode = "RecruitmentClearance"
            //        }
            //    },
            //    RequestedDate = DateTime.UtcNow,
            //    Status = new MasterTableValueResultModel
            //    {
            //        Value = new MasterTableValue
            //        {
            //            Code = "CLEARANCE_REQUESTED",
            //            TableCode = "ClearanceStatus" // You might need to adjust this TableCode
            //        }
            //    },
            //    LinkDetailRemarks = $"clearanceRequestId={requestId}",
            //    CreatedDate = DateTime.UtcNow,
            //    CreationUser = "System", // You might want to set this
            //    UpdatedDate = DateTime.UtcNow,
            //    UpdateUser = "System" // You might want to set this
            //};
            await _clearancesRepo.AddAsync(clearance);
            await _clearancesRepo.SaveChangesAsync();

            _logger.Information("[{System}] CYCLE 1 Complete: StatusCode=CLEARANCE_REQUESTED", strategy.SystemCode);
        }

        private async Task HandleStatusCheckAsync(IntegrationInvocation invocation, string response, IResultMappingStrategy strategy)
        {
            if (strategy.IsMultiResultStatusResponse(response))
            {
                await HandleMultiResultStatusAsync(invocation, response, strategy);
            }
            else
            {
                await HandleSingleResultStatusAsync(invocation, response, strategy);
            }
        }

        private async Task HandleSingleResultStatusAsync(IntegrationInvocation invocation, string response, IResultMappingStrategy strategy)
        {
            var requestPayload = await GetLatestRequestPayloadAsync(invocation.IntegrationInvocationId);
            if (requestPayload == null)
            {
                _logger.Warning("[{System}] No request payload found for invocation {Id}", strategy.SystemCode, invocation.IntegrationInvocationId);
                return;
            }

            var clearanceId = strategy.ExtractRequestId(requestPayload);
            if (string.IsNullOrWhiteSpace(clearanceId))
            {
                _logger.Warning("[{System}] Could not extract clearance ID from request", strategy.SystemCode);
                return;
            }

            var oneHr = await FindOneHRRecordAsync(clearanceId);
            if (oneHr == null)
            {
                _logger.Warning("[{System}] OneHR record not found for clearanceId={Id}", strategy.SystemCode, clearanceId);
                return;
            }

            var responseId = strategy.ExtractResponseId(response);

            _logger.Information("[{System}] CYCLE 2 - STATUS: ClearanceId={ClearanceId}, ResponseId={ResponseId}",
                strategy.SystemCode, clearanceId, responseId ?? "n/a");

            await UpdateStatusCompletionAsync(oneHr, responseId, strategy);
        }

        private async Task HandleMultiResultStatusAsync(IntegrationInvocation invocation, string response, IResultMappingStrategy strategy)
        {
            var results = await strategy.ExtractStatusResultsAsync(response);

            _logger.Information("[{System}] Processing {Count} status results", strategy.SystemCode, results.Count);

            int successCount = 0;
            foreach (var result in results)
            {
                try
                {
                    var oneHr = await FindOneHRRecordByCandidateAsync(result.CandidateId, result.DoaCandidateId);
                    if (oneHr == null)
                    {
                        _logger.Debug("[{System}] No incomplete OneHR for CandidateId={CandId}, DoaCandidateId={DoaId}",
                            strategy.SystemCode, result.CandidateId, result.DoaCandidateId);
                        continue;
                    }

                    await UpdateStatusCompletionAsync(oneHr, result.Identifier, strategy, result.StatusDate);
                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "[{System}] Error processing result for CandidateId={CandId}",
                        strategy.SystemCode, result.CandidateId);
                }
            }

            _logger.Information("[{System}] CYCLE 2 Complete: Processed {Success} records", strategy.SystemCode, successCount);
        }

        private async Task HandleAcknowledgeAsync(IntegrationInvocation invocation, IResultMappingStrategy strategy)
        {
            if (!strategy.RequiresAcknowledgeCycle)
            {
                _logger.Warning("[{System}] ACKNOWLEDGE called but system has only {Cycles} cycles",
                    strategy.SystemCode, strategy.ClearanceCycleCount);
                return;
            }

            var (doaCandidateId, _) = await ResolveIdsFromInvocationAsync(invocation);

            _logger.Information("[{System}] CYCLE 3 - ACKNOWLEDGE for DoaCandidateId={Id}", strategy.SystemCode, doaCandidateId);

            var clearance = (await _clearancesRepo.FindAsync(
                c => c.DoaCandidateId == doaCandidateId && c.RecruitmentClearanceCode == strategy.SystemCode))
                .FirstOrDefault();

            //        var clearance = (await _clearancesRepo.FindAsync(
            //c => c.DoaCandidateId == doaCandidateId
            //&& c.RecruitmentClearance != null
            //&& c.RecruitmentClearance.Value != null
            //&& c.RecruitmentClearance.Value.Code == strategy.SystemCode))
            //.FirstOrDefault();

            if (clearance != null)
            {
                clearance.StatusCode = "DELIVERED";
                clearance.UpdatedDate = DateTime.UtcNow;
                clearance.AdditionalRemarks = AppendToRemarks(clearance.AdditionalRemarks, "Acknowledgement posted");

                await _clearancesRepo.UpdateAsync(clearance);
                await _clearancesRepo.SaveChangesAsync();

                _logger.Information("[{System}] CYCLE 3 Complete: StatusCode=DELIVERED", strategy.SystemCode);
            }
            //if (clearance != null)
            //{
            //    // Update the Status
            //    clearance.Status = new MasterTableValueResultModel
            //    {
            //        Value = new MasterTableValue
            //        {
            //            Code = "DELIVERED",
            //            TableCode = clearance.Status?.Value?.TableCode ?? "ClearanceStatus" // Preserve existing TableCode or use default
            //        }
            //    };

            //    clearance.UpdatedDate = DateTime.UtcNow;
            //    clearance.UpdateUser = "System"; // You might want to set this appropriately
            //    clearance.AdditionalRemarks = AppendToRemarks(clearance.AdditionalRemarks, "Acknowledgement posted");

            //    await _clearancesRepo.UpdateAsync(clearance);
            //    await _clearancesRepo.SaveChangesAsync();

            //    _logger.Information("[{System}] CYCLE 3 Complete: StatusCode=DELIVERED", strategy.SystemCode);
            //}
        }

        private async Task UpdateStatusCompletionAsync(DoaCandidateClearancesOneHR oneHr, string? responseId,
            IResultMappingStrategy strategy, DateTime? statusDate = null)
        {
            // Update OneHR record
            if (!string.IsNullOrWhiteSpace(responseId))
            {
                oneHr.RVCaseId = responseId;
            }
            oneHr.IsCompleted = true;
            oneHr.CompletionDate = statusDate ?? DateTime.UtcNow;

            await _clearancesOneHRRepo.UpdateAsync(oneHr);
            await _clearancesOneHRRepo.SaveChangesAsync();

            _logger.Information("[{System}] OneHR updated: RVCaseId={RVCaseId}, IsCompleted=true",
                strategy.SystemCode, oneHr.RVCaseId ?? "null");

            // Update clearance summary
            var clearance = (await _clearancesRepo.FindAsync(
                c => c.DoaCandidateId == oneHr.DoaCandidateId && c.RecruitmentClearanceCode == strategy.SystemCode))
                .OrderByDescending(c => c.RequestedDate)
                .FirstOrDefault();

            //var clearance = (await _clearancesRepo.FindAsync(
            //    c => c.DoaCandidateId == oneHr.DoaCandidateId && c.RecruitmentClearance.Value.Code == strategy.SystemCode))
            //    .OrderByDescending(c => c.RequestedDate)
            //    .FirstOrDefault();

            if (clearance != null)
            {
                var completionCode = strategy.GetStatusCompletionCode();
                clearance.StatusCode = completionCode;
                clearance.Outcome = strategy.RequiresAcknowledgeCycle ? null : "Complete";
                clearance.CompletionDate = strategy.RequiresAcknowledgeCycle ? null : (statusDate ?? DateTime.UtcNow);

                if (!string.IsNullOrWhiteSpace(responseId))
                {
                    clearance.LinkDetailRemarks = AppendToRemarks(clearance.LinkDetailRemarks, $"clearanceResponseId={responseId}");
                }

                clearance.UpdatedDate = DateTime.UtcNow;

                await _clearancesRepo.UpdateAsync(clearance);
                await _clearancesRepo.SaveChangesAsync();

                _logger.Information("[{System}] Clearance updated: StatusCode={Status}", strategy.SystemCode, completionCode);
            }
            //if (clearance != null)
            //{
            //    var completionCode = strategy.GetStatusCompletionCode();

            //    // Update the Status
            //    clearance.Status = new MasterTableValueResultModel
            //    {
            //        Value = new MasterTableValue
            //        {
            //            Code = completionCode,
            //            TableCode = clearance.Status?.Value?.TableCode ?? "ClearanceStatus"
            //        }
            //    };

            //    clearance.Outcome = strategy.RequiresAcknowledgeCycle ? null : "Complete";
            //    clearance.CompletionDate = strategy.RequiresAcknowledgeCycle ? null : (statusDate ?? DateTime.UtcNow);

            //    if (!string.IsNullOrWhiteSpace(responseId))
            //    {
            //        clearance.LinkDetailRemarks = AppendToRemarks(clearance.LinkDetailRemarks, $"clearanceResponseId={responseId}");
            //    }

            //    clearance.UpdatedDate = DateTime.UtcNow;
            //    clearance.UpdateUser = "System"; // Set appropriately based on your context

            //    await _clearancesRepo.UpdateAsync(clearance);
            //    await _clearancesRepo.SaveChangesAsync();

            //    _logger.Information("[{System}] Clearance updated: StatusCode={Status}", strategy.SystemCode, completionCode);
            //}
        }

        //private async Task<(long doaCandidateId, long candidateId)> ResolveIdsFromInvocationAsync(IntegrationInvocation invocation)
        //{
        //    var firstRequestLog = (await _invocationLogRepo.FindAsync(l =>
        //            l.IntegrationInvocationId == invocation.IntegrationInvocationId &&
        //            !string.IsNullOrWhiteSpace(l.RequestPayload)))
        //        .OrderBy(l => l.LogSequence)
        //        .FirstOrDefault();

        //    if (firstRequestLog == null)
        //    {
        //        throw new InvalidOperationException($"No request payload found for invocation {invocation.IntegrationInvocationId}");
        //    }

        //    // Parse payload to extract IDs (simplified - extend based on your payload structure)
        //    // This should use the KeyMappingProvider or similar logic from ModelLoaderService
        //    var payload = JToken.Parse(firstRequestLog.RequestPayload);

        //    // Extract based on your payload structure
        //    long doaCandidateId = 0;
        //    long candidateId = 0;

        //    if (doaCandidateId == 0 && candidateId == 0)
        //    {
        //        // For EARTHMED: extract from ReferenceNumber (format: DoaId_DoaCandidateId) for CMTS the logic would be different
        //        var referenceNumber = _fieldExtractor.TryGetStringFromJsonAnyDepth(payload, "ReferenceNumber");
        //        var indexNumber = _fieldExtractor.TryGetStringFromJsonAnyDepth(payload, "IndexNumber");
        //        if (!string.IsNullOrWhiteSpace(referenceNumber) && referenceNumber.Contains("_"))
        //        {
        //            var parts = referenceNumber.Split('_');
        //            if (parts.Length == 2 &&
        //                long.TryParse(parts[0], out var doaId) &&
        //                long.TryParse(parts[1], out var doaCanId))
        //            {

        //                // For EARTHMED, we need to get CandidateId from DoaCandidateClearancesOneHR

        //                var candidate = (await _doaCandidateRepo.FindAsync(x => x.Id == doaId))
        //                .FirstOrDefault();
        //                if (candidateId == null)
        //                {
        //                    if (!string.IsNullOrWhiteSpace(indexNumber) && long.TryParse(indexNumber, out var parsedIndexNumber))
        //                    {
        //                        candidateId = parsedIndexNumber;
        //                    }
        //                }

        //                if (candidateId != null || doaCanId != null)
        //                {
        //                    _logger.Information("[{System}] Resolved from ReferenceNumber: DoaId={DoaId}, DoaCandidateId={DoaCandidateId}, CandidateId={CandidateId}",
        //                        doaId, doaCandidateId, candidate.Id);
        //                    return (doaCanId, candidate.Id);
        //                }
        //            }
        //        }
        //    }

        //    // Add extraction logic here based on your payload format
        //    // This is a placeholder - implement based on actual payload structure

        //    return (doaCandidateId, candidateId);
        //}
        private async Task<(long doaCandidateId, long candidateId)> ResolveIdsFromInvocationAsync(IntegrationInvocation invocation)
        {
            var firstRequestLog = (await _invocationLogRepo.FindAsync(l =>
                    l.IntegrationInvocationId == invocation.IntegrationInvocationId &&
                    !string.IsNullOrWhiteSpace(l.RequestPayload)))
                .OrderBy(l => l.LogSequence)
                .FirstOrDefault();

            if (firstRequestLog == null)
            {
                throw new InvalidOperationException($"No request payload found for invocation {invocation.IntegrationInvocationId}");
            }

            // Parse payload to extract IDs (simplified - extend based on your payload structure)
            // This should use the KeyMappingProvider or similar logic from ModelLoaderService
            var payload = JToken.Parse(firstRequestLog.RequestPayload);

            // Extract based on your payload structure
            long doaCandidateId = 0;
            long candidateId = 0;

            // ✏️ CORRECTED: Extract from ReferenceNumber and IndexNumber
            var referenceNumber = _fieldExtractor.TryGetStringFromJsonAnyDepth(payload, "ReferenceNumber");
            var indexNumber = _fieldExtractor.TryGetStringFromJsonAnyDepth(payload, "IndexNumber");

            if (!string.IsNullOrWhiteSpace(referenceNumber) && referenceNumber.Contains("_"))
            {
                var parts = referenceNumber.Split('_');
                if (parts.Length == 2 &&
                    long.TryParse(parts[0], out var doaId) &&  // ✏️ First part is DoaCandidateId
                    long.TryParse(parts[1], out var doaCanId))       // ✏️ Second part is DoaId
                {
                    doaCandidateId = doaCanId;

                    // ✏️ For EARTHMED, get CandidateId from DoaCandidate table using DoaId
                    var candidate = (await _doaCandidateRepo.FindAsync(x => x.Id == doaId))
                        .FirstOrDefault();

                    if (candidate != null)
                    {
                        candidateId = candidate.Id;

                        _logger.Information("[EARTHMED] Resolved from ReferenceNumber: DoaId={DoaId}, DoaCandidateId={DoaCandidateId}, CandidateId={CandidateId}",
                            doaId, doaCandidateId, candidateId);

                        return (doaCandidateId, candidateId);
                    }
                    else
                    {
                        _logger.Warning("[EARTHMED] DoaCandidate not found for DoaId={DoaId}", doaId);
                    }
                }
            }

            // ✏️ Fallback: Try to get candidateId from IndexNumber if ReferenceNumber parsing failed
            if (candidateId == 0 && !string.IsNullOrWhiteSpace(indexNumber) && long.TryParse(indexNumber, out var parsedIndexNumber))
            {
                candidateId = parsedIndexNumber;
                _logger.Information("[EARTHMED] Resolved CandidateId from IndexNumber: {CandidateId}", candidateId);
            }

            // ✏️ Return whatever we could resolve
            if (doaCandidateId == 0 && candidateId == 0)
            {
                _logger.Warning("[EARTHMED] Could not resolve IDs from payload for invocation {InvocationId}", invocation.IntegrationInvocationId);
            }

            return (doaCandidateId, candidateId);
        }
        private async Task<string?> GetLatestRequestPayloadAsync(long invocationId)
        {
            var log = (await _invocationLogRepo.FindAsync(l =>
                    l.IntegrationInvocationId == invocationId &&
                    !string.IsNullOrWhiteSpace(l.RequestPayload)))
                .OrderBy(l => l.LogSequence)
                .FirstOrDefault();

            return log?.RequestPayload;
        }

        private async Task<DoaCandidateClearancesOneHR?> FindOneHRRecordAsync(string clearanceId)
        {
            return (await _clearancesOneHRRepo.FindAsync(o =>
                    o.DoaCandidateClearanceId == clearanceId && !o.IsCompleted))
                .OrderByDescending(o => o.RequestedDate)
                .FirstOrDefault();
        }

        private async Task<DoaCandidateClearancesOneHR?> FindOneHRRecordByCandidateAsync(long candidateId, long doaCandidateId)
        {
            return (await _clearancesOneHRRepo.FindAsync(o =>
                    o.CandidateId == candidateId &&
                    o.DoaCandidateId == doaCandidateId &&
                    !o.IsCompleted))
                .OrderByDescending(o => o.RequestedDate)
                .FirstOrDefault();
        }

        private static string AppendToRemarks(string? existing, string newRemark)
        {
            if (string.IsNullOrWhiteSpace(existing))
                return newRemark;
            return $"{existing.TrimEnd(';')};{newRemark}";
        }
    }
}
