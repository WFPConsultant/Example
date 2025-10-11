namespace UVP.ExternalIntegration.Domain.Integration.Mappers
{
    using System;
    using UVP.Doa.Domain.Sql.Entities;
    using UVP.ExternalIntegration.Domain.Integration.DTOs;
    using UVP.User.Domain.Entities;

    public class IntegrationDtoMapper : IIntegrationDtoMapper
    {
        IntegrationCandidateDto IIntegrationDtoMapper.MapToIntegrationDto(Entity.Users.Candidate candidate)
        {
            if (candidate == null)
                throw new ArgumentNullException(nameof(candidate));

            return new IntegrationCandidateDto
            {
                Id = candidate.CandidateId,
                FirstName = candidate.FirstName ?? string.Empty,
                MiddleName = null,
                LastName = candidate.LastName ?? string.Empty,
                Gender = candidate.Gender,
                CountryOfBirth = candidate.CountryOfBirth,
                CountryOfBirthISOCode = ExtractISOCode(candidate.CountryOfBirth),
                Nationality = candidate.CurrentNationality,
                NationalityISOCode = ExtractISOCode(candidate.CurrentNationality),
                DateOfBirth = candidate.DateOfBirth ?? DateTime.MinValue,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = null,
                UserId = 0,
                EmailAddress = candidate.EmailAddress
            };
        }
        IntegrationDoaDto IIntegrationDtoMapper.MapToIntegrationDto(Doa doa)
        {
            if (doa == null)
                throw new ArgumentNullException(nameof(doa));

            return new IntegrationDoaDto
            {
                Id = doa.Id,
                Name = doa.Name ?? string.Empty,
                OrganizationMission = doa.OrganizationMission ?? string.Empty,
                DutyStationCode = "1620",
                DutyStationDescription = "Accra",
                StartDate = doa.StartDate,
                ExpectedEndDate = doa.ExpectedEndDate,
                CreatedDate = doa.CreatedDate,
                UpdatedDate = doa.UpdatedDate
            };
        }
        IntegrationUserDto IIntegrationDtoMapper.MapToIntegrationDto(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return new IntegrationUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName ?? string.Empty,
                MiddleName = "",
                LastName = user.LastName ?? string.Empty,
                Gender = user.GenderCode == "GenderFemale" ? "F" :user.GenderCode == "GenderMale" ? "M" : "O",
                DateOfBirth = Convert.ToDateTime(user.BirthDate),
                PersonalEmail = user.PersonalEmail,
                NationalityISOCode = ExtractISOCode(user.CountryCode),
                CreatedDate = user.CreatedDate,
                UpdatedDate = user.UpdatedDate
            };
        }
        private string? ExtractISOCode(string? countryOrNationality)
        {
            // TODO: Implement ISO code lookup
            return null;
        }
    }
}
