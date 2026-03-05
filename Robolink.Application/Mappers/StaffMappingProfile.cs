using Robolink.Core.Entities;
using Robolink.Shared.DTOs;
using AutoMapper;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;

namespace Robolink.Application.Mappers
{
    public class StaffMappingProfile : Profile
    {
        public StaffMappingProfile()
        {
            CreateMap<Staff, StaffDto>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Chặn đứng rò rỉ mật khẩu
        }
        
    }
}
