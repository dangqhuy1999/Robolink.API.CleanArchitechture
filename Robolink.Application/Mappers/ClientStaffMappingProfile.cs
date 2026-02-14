using AutoMapper;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;

namespace Robolink.Application.Mappers
{
    /// <summary>AutoMapper profile for Client and Staff entities</summary>
    public class ClientStaffMappingProfile : Profile
    {
        public ClientStaffMappingProfile()
        {
            // Client → ClientDto
            CreateMap<Client, ClientDto>()
                .ForMember(dest => dest.ProjectCount,
                    opt => opt.MapFrom(src => src.Projects != null ? src.Projects.Count : 0));

            // Staff → StaffDto
            CreateMap<Staff, StaffDto>();
        }
    }
}