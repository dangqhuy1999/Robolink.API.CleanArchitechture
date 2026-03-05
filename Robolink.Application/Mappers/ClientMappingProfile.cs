using AutoMapper;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;

namespace Robolink.Application.Mappers
{
    /// <summary>AutoMapper profile for Client and Staff entities</summary>
    public class ClientMappingProfile : Profile
    {
        public ClientMappingProfile()
        {
            // Client → ClientDto
            CreateMap<Client, ClientDto>()
                .ForMember(dest => dest.ProjectCount,
                    opt => opt.MapFrom(src => src.Projects != null ? src.Projects.Count : 0));

        }
    }
}