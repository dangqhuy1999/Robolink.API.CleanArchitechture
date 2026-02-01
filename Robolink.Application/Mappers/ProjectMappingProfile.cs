using AutoMapper;
using Robolink.Application.DTOs;
using Robolink.Core.Entities;

namespace Robolink.Application.Mappers
{
    /// <summary>AutoMapper profile for Project entity</summary>
    public class ProjectMappingProfile : Profile
    {
        public ProjectMappingProfile()
        {
            // ✅ RECURSIVE MAPPING: Project → ProjectDto
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.ClientName,
                    opt => opt.MapFrom(src => src.Client != null ? src.Client.Name : "Unknown"))
                .ForMember(dest => dest.ManagerName,
                    opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FullName : "Unknown"))
                .ForMember(dest => dest.ParentProjectName,
                    opt => opt.MapFrom(src => src.ParentProject != null ? src.ParentProject.Name : null))
                // ✅ FIXED: Map SubProjectsItems (List<Project>) to SubProjects (List<ProjectDto>)
                // This is RECURSIVE - each sub-project will also be mapped to ProjectDto
                .ForMember(dest => dest.SubProjects,
                    opt => opt.MapFrom(src => src.SubProjectsItems != null 
                        ? src.SubProjectsItems.Select(sp => new ProjectDto
                        {
                            Id = sp.Id,
                            ProjectCode = sp.ProjectCode,
                            Name = sp.Name,
                            Description = sp.Description,
                            ClientId = sp.ClientId,
                            ClientName = sp.Client != null ? sp.Client.Name : "Unknown",
                            ManagerId = sp.ManagerId,
                            ManagerName = sp.Manager != null ? sp.Manager.FullName : "Unknown",
                            StartDate = sp.StartDate,
                            Deadline = sp.Deadline,
                            Status = (int)sp.Status,
                            Priority = (int)sp.Priority,
                            InternalBudget = sp.InternalBudget,
                            CustomerBudget = sp.CustomerBudget,
                            CalculationConfigJson = sp.CalculationConfigJson,
                            CreatedAt = sp.CreatedAt,
                            CreatedBy = sp.CreatedBy,
                            ParentProjectId = sp.ParentProjectId,
                            ParentProjectName = sp.ParentProject != null ? sp.ParentProject.Name : null,
                            SubProjects = new List<ProjectDto>() // Empty for subs (no deep nesting)
                        }).ToList()
                        : new List<ProjectDto>()));

            // CreateRequest → Entity (Create)
            CreateMap<CreateProjectRequest, Project>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => 0)) // Draft status
                .ForMember(dest => dest.ParentProjectId, opt => opt.MapFrom(src => src.ParentProjectId));

            // UpdateRequest → Entity (Update - partial)
            CreateMap<UpdateProjectRequest, Project>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}