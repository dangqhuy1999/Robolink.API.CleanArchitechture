using AutoMapper;
using Robolink.Application.DTOs;
using Robolink.Core.Entities;

namespace Robolink.Application.Mappers
{
    /// <summary>AutoMapper profile for SystemPhase and ProjectSystemPhaseConfig entities</summary>
    public class SystemPhaseMappingProfile : Profile
    {
        public SystemPhaseMappingProfile()
        {
            // ✅ SystemPhase → SystemPhaseDto
            CreateMap<SystemPhase, SystemPhaseDto>();

            // ✅ ProjectSystemPhaseConfig → ProjectPhaseConfigDto
            CreateMap<ProjectSystemPhaseConfig, ProjectPhaseConfigDto>()
                .ForMember(dest => dest.SystemPhase, 
                    opt => opt.MapFrom(src => src.SystemPhase))
                .ForMember(dest => dest.TaskCount,
                    opt => opt.MapFrom(src => src.PhaseTasks != null ? src.PhaseTasks.Count : 0))
                .ForMember(dest => dest.Tasks,
                    opt => opt.MapFrom(src => src.PhaseTasks != null ? src.PhaseTasks : new List<PhaseTask>()));
        }
    }
}