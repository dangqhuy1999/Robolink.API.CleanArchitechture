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

                .ForMember(dest => dest.ProgressPercentage, opt => opt.MapFrom(src =>
                    (src.Tasks != null && src.Tasks.Any())
                    ? Math.Round((double)src.Tasks.Count(t => t.IsCompleted) / src.Tasks.Count * 100, 2)
                    : 0))

                // Giới hạn độ sâu để tránh lỗi vòng lặp vô tận (Circular Reference)
                .ForMember(dest => dest.SubProjects, opt => opt.MapFrom(src => src.SubProjectsItems))
                .MaxDepth(3);

            // Request -> Entity
            CreateMap<CreateProjectRequest, Project>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                // Sử dụng Enum thay vì số 0 để code "Clean" hơn
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => ProjectStatus.Draft));

            CreateMap<UpdateProjectRequest, Project>()
                // Bỏ qua Id để tránh overwrite key
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            
        }
    }
}