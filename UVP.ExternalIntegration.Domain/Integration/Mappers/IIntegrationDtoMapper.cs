namespace UVP.ExternalIntegration.Domain.Integration.Mappers
{
    using UVP.Doa.Domain.Sql.Entities;
    using UVP.ExternalIntegration.Domain.Entity.Users;
    using UVP.ExternalIntegration.Domain.Integration.DTOs;
    using UVP.User.Domain.Entities;

    /// <summary>
    /// Maps between main project entities and integration DTOs.
    /// Follows SOLID principles - Open for extension, closed for modification.
    /// </summary>
    public interface IIntegrationDtoMapper
    {
        IntegrationCandidateDto MapToIntegrationDto(Candidate candidate);
        IntegrationDoaDto MapToIntegrationDto(Doa doa);
        IntegrationUserDto MapToIntegrationDto(User user);
    }
}
