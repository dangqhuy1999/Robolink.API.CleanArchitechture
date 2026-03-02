using AutoMapper;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;

namespace Robolink.Application.Mappers
{
    public class ProjectPhaseMappingProfile : Profile
    {
        public ProjectPhaseMappingProfile()
        {
            // 🚀 ProjectSystemPhaseConfig → ProjectPhaseConfigDto (Bản Tesla)
            CreateMap<ProjectSystemPhaseConfig, ProjectPhaseConfigDto>()
                // Chỉ định rõ cột đổi tên (PhaseTasks -> Tasks)
                .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.PhaseTasks))

                // Đếm số lượng trực tiếp (EF Core sẽ tự dịch thành câu SQL đếm cực nhanh)
                .ForMember(dest => dest.TaskCount, opt => opt.MapFrom(src => src.PhaseTasks.Count));

            // Lưu ý: SystemPhase nó cùng tên nên AutoMapper tự tự nhận diện, không cần viết!
        }
    }
}
