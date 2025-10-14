namespace UVP.ExternalIntegration.Business.Interfaces
{
    using System.Threading.Tasks;
    using UVP.ExternalIntegration.Domain.Integration.DTOs;

    public interface IInvocationManagerService
    {
        Task<long> CreateInvocationAsync(long doaCandidateId, long candidateId, string integrationType);
        Task<long> CreateInvocationAsync(IntegrationRequestDto request);
        Task<bool> ProcessPendingInvocationsAsync();
        Task<bool> ProcessRetryableInvocationsAsync();
    }
}
