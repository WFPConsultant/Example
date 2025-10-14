namespace UVP.ExternalIntegration.Business.Mapper.ResponseMapper.Strategies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Serilog;
    using UVP.ExternalIntegration.Business.Mapper.ResponseMapper.Interfaces;
    using UVP.ExternalIntegration.Domain.Integration.DTOs;

    public class CmtsResultMappingStrategy : IResultMappingStrategy
    {
        private readonly ILogger _logger = Log.ForContext<CmtsResultMappingStrategy>();

        public string SystemCode => "CMTS";
        public bool RequiresAcknowledgeCycle => true;
        public int ClearanceCycleCount => 3;

        public string? ExtractRequestId(string responseBody)
        {
            try
            {
                var response = JsonConvert.DeserializeObject<ClearanceResponseDto>(responseBody);
                return response?.ClearanceRequestId;
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "[{System}] Failed to extract request ID", SystemCode);
                return null;
            }
        }

        public string? ExtractResponseId(string responseBody)
        {
            try
            {
                var response = JsonConvert.DeserializeObject<ClearanceResponseDto>(responseBody);
                return response?.ClearanceResponseId;
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "[{System}] Failed to extract response ID", SystemCode);
                return null;
            }
        }

        public bool IsMultiResultStatusResponse(string responseBody) => false;

        public Task<List<StatusResultItem>> ExtractStatusResultsAsync(string responseBody)
        {
            return Task.FromResult(new List<StatusResultItem>());
        }

        public string GetStatusCompletionCode() => "CLEARED";
    }
}
