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
            CreateMap<Project, ProjectDto>()
                // 1. Chỉ dẫn cho SQL cách lấy ClientName (AutoMapper tự Join bảng Client)
                .ForMember(dest => dest.ClientName,
                    opt => opt.MapFrom(src => src.Client.Name ?? "Unknown"))

                // 2. Chỉ dẫn cách lấy ManagerName (AutoMapper tự Join bảng Manager)
                .ForMember(dest => dest.ManagerName,
                    opt => opt.MapFrom(src => src.Manager.FullName ?? "Unknown"))

                .ForMember(dest => dest.ParentProjectName,
                    opt => opt.MapFrom(src => src.ParentProject.Name))

                // 3. Đệ quy: Tự động áp dụng chính quy tắc này cho danh sách con
                .ForMember(dest => dest.SubProjects,
                    opt => opt.MapFrom(src => src.SubProjectsItems));

            // KHÔNG DÙNG AfterMap ở đây nữa em nhé! 
            // Vì ProjectTo sẽ tự Join để lấy ClientName cho từng SubProject luôn.

            // Request -> Entity giữ nguyên
            CreateMap<CreateProjectRequest, Project>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => 0));

            CreateMap<UpdateProjectRequest, Project>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}