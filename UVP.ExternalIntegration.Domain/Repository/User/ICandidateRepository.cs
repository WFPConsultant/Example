namespace UVP.ExternalIntegration.Domain.Repository.Users
{
    using System.Threading.Tasks;
    using UVP.ExternalIntegration.Domain.Entity.Users;

    public interface ICandidateRepository
    {
        Task<Candidate> GetCandidateByIdAsync(long candidateId);

        Task<DependentDetails> GetDependentsByDoaCandidateIdAsync(long doaCandidateId);
    }
}
