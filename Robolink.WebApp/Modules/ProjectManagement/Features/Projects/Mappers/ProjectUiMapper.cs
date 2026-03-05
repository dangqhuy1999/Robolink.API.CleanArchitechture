using Robolink.Shared.DTOs;
using Robolink.Shared.Enums;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Mappers;

/// <summary>
/// Mapper for converting between API DTOs and UI ViewModels.
/// Encapsulates all DTO-to-ViewModel transformations.
/// </summary>
public static class ProjectUiMapper
{
    /// <summary>
    /// Converts a ProjectDto to ProjectViewModel.
    /// </summary>
    public static ProjectViewModel ToViewModel(ProjectDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new ProjectViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            ClientId = dto.ClientId,
            ClientName = dto.ClientName ?? "Unknown Client",
            ManagerId = dto.ManagerId,
            ManagerName = dto.ManagerName ?? "Unknown Manager",
            StartDate = dto.StartDate,
            Deadline = dto.Deadline,
            Priority = dto.Priority,
            Status = (ProjectStatus)dto.Status,
            ParentProjectId = dto.ParentProjectId,
            SubProjectsCount = dto.SubProjectsCount,
            SubProjects = dto.SubProjects?.Select(ToViewModel).ToList() ?? []
        };
    }

    /// <summary>
    /// Converts multiple ProjectDtos to ProjectViewModels.
    /// </summary>
    public static List<ProjectViewModel> ToViewModels(IEnumerable<ProjectDto> dtos)
    {
        ArgumentNullException.ThrowIfNull(dtos);

        return dtos.Select(ToViewModel).ToList();
    }

    /// <summary>
    /// Converts a ClientDto to ClientViewModel.
    /// </summary>
    public static ClientViewModel ToClientViewModel(ClientDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new ClientViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            ContactEmail = dto.ContactEmail ,
            Industry = dto.Industry,
            ProjectCount = dto.ProjectCount
        };
    }

    /// <summary>
    /// Converts multiple ClientDtos to ClientViewModels.
    /// </summary>
    public static List<ClientViewModel> ToClientViewModels(IEnumerable<ClientDto> dtos)
    {
        ArgumentNullException.ThrowIfNull(dtos);

        return dtos.Select(ToClientViewModel).ToList();
    }

    /// <summary>
    /// Converts a StaffDto to ManagerViewModel.
    /// </summary>
    public static ManagerViewModel ToManagerViewModel(StaffDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new ManagerViewModel
        {
            Id = dto.Id,
            FullName = dto.FullName ?? "Unknown",
            Email = dto.Email,
            Department = dto.Department,
            Role = dto.Role
        };
    }

    /// <summary>
    /// Converts multiple StaffDtos to ManagerViewModels.
    /// </summary>
    public static List<ManagerViewModel> ToManagerViewModels(IEnumerable<StaffDto> dtos)
    {
        ArgumentNullException.ThrowIfNull(dtos);

        return dtos.Select(ToManagerViewModel).ToList();
    }

    /// <summary>
    /// Converts CreateProjectViewModel to CreateProjectRequest.
    /// </summary>
    public static CreateProjectRequest ToCreateRequest(CreateProjectViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        return new CreateProjectRequest
        {
            ProjectCode = viewModel.ProjectCode,
            Name = viewModel.Name,
            Description = viewModel.Description ?? "Unknown",
            ClientId = viewModel.ClientId,
            ManagerId = viewModel.ManagerId,
            StartDate = viewModel.StartDate,
            Deadline = viewModel.Deadline,
            Priority = viewModel.Priority,
            ParentProjectId = viewModel.ParentProjectId,
            InternalBudget = viewModel.InternalBudget,
            CustomerBudget = viewModel.CustomerBudget

        };
    }

    /// <summary>
    /// Converts EditProjectViewModel to UpdateProjectRequest.
    /// </summary>
    public static UpdateProjectRequest ToUpdateRequest(EditProjectViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        return new UpdateProjectRequest
        {
            Id = viewModel.Id,
            Name = viewModel.Name,
            ProjectCode = viewModel.ProjectCode,
            Description = viewModel.Description,
            ClientId = viewModel.ClientId,
            ManagerId = viewModel.ManagerId,
            StartDate = viewModel.StartDate,
            Deadline = viewModel.Deadline,
            Priority = viewModel.Priority,
            InternalBudget = viewModel.InternalBudget,
            CustomerBudget = viewModel.CustomerBudget

        };
    }
}
