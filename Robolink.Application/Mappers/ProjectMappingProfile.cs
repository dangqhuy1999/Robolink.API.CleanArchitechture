using AutoMapper;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Shared.Enums;

namespace Robolink.Application.Mappers
{
    /// <summary>AutoMapper profile for Project entity and related DTOs</summary>
    public class ProjectMappingProfile : Profile
    {
        public ProjectMappingProfile()
        {
            // Entity -> DTO
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.ClientName,
                    opt => opt.MapFrom(src => src.Client != null ? src.Client.Name : "N/A"))

                .ForMember(dest => dest.ManagerName,
                    opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FullName : "Unknown"))

                .ForMember(dest => dest.ParentProjectName,
                    opt => opt.MapFrom(src => src.ParentProject != null ? src.ParentProject.Name : null))

                /*Giải thích thêm: Khi dự án mới tạo qua QuickCreateAsync, 
                 * nó chưa có Task nào (Tasks.Count = 0). 
                 * Câu lệnh SQL do AutoMapper tạo ra nếu không check kỹ sẽ là: 
                 * SELECT ... (completed_count / 0) ... -> 
                 * PostgreSQL sẽ dừng ngay lập tức và báo lỗi float_zero_divide_error.*/

                .ForMember(dest => dest.ProgressPercentage, opt => opt.MapFrom(src =>
                    src.Tasks.Count == 0 ? 0 :
                    Math.Round((double)src.Tasks.Count(x => x.Status == Task_Status.Completed) / (src.Tasks.Count == 0 ? 1 : src.Tasks.Count) * 100, 2)
                ))

                // Giới hạn độ sâu để tránh lỗi vòng lặp vô tận (Circular Reference)
                .ForMember(dest => dest.SubProjects, opt => opt.MapFrom(src => src.SubProjectsItems))
                .MaxDepth(3);

            // Request -> Entity
            CreateMap<CreateProjectRequest, Project>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                // Sử dụng Enum thay vì số 0 để code "Clean" hơn
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => ProjectStatus.Draft));

            CreateMap<UpdateProjectRequest, Project>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Giữ nguyên dòng này
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
            {
                // 1. Nếu trường đó bị Null trong Request -> Bỏ qua, không ghi đè vào DB
                if (srcMember == null) return false;

                // 2. Nếu là kiểu Guid (như ClientId, ManagerId) và là Guid.Empty -> Bỏ qua
                if (srcMember is Guid guidMember && guidMember == Guid.Empty) return false;

                return true;
            }));

        }
    }
}