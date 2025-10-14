namespace UVP.ExternalIntegration.Business.Interfaces
{
    using System.Threading.Tasks;

    public interface IIntegrationOrchestrationService
    {
        Task<bool> ExecuteFullClearanceCycleAsync(long doaCandidateId, long candidateId, string integrationType);
        Task<bool> CheckAndProgressClearanceAsync(long doaCandidateId, long candidateId, string integrationType);
    }
}
