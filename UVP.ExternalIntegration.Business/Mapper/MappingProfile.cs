namespace UVP.ExternalIntegration.Business.Mapper
{
    using AutoMapper;
    using UVP.Doa.Business.Model;
    using UVP.Doa.Domain.Sql.Entities;
    using UVP.ExternalIntegration.Business.Model.Common;
    using UVP.ExternalIntegration.Domain.Entity.Doa;
    using UVP.ExternalIntegration.Domain.Entity.Users;

    /// <summary>
    /// Mapping profile class.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class.
        /// Creates a map.
        /// </summary>
        public MappingProfile()
        {
            this.CreateMap<Candidate, CandidateModel>().ReverseMap();
            this.CreateMap<UserAssignment, AssignmentModel>().ReverseMap();
            this.CreateMap<SalaryBasis, SalaryBasisModel>().ReverseMap();
            this.CreateMap<FundingDetails, FundingDetailsModel>().ReverseMap();
            this.CreateMap<APIStatusDisplay, APIStatusDisplayModel>().ReverseMap();

            this.CreateMap<DoaCandidate, DoaCandidateModel>().ReverseMap();
        }
    }
}
