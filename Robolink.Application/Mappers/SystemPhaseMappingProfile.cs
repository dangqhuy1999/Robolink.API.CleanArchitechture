using AutoMapper;
using Robolink.Shared.DTOs;
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

            
        }
    }
}