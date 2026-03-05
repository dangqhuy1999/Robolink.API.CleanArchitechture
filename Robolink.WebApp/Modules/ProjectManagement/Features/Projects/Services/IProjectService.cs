using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Services;

/// <summary>
/// Business service for project operations.
/// Orchestrates API calls and data transformations.
/// Decouples UI from direct API dependency.
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// Gets paginated list of parent projects with sub-projects.
    /// </summary>
    Task<(List<ProjectViewModel> Projects, int TotalCount)> GetProjectsPaginatedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a single project by ID.
    /// </summary>
    Task<ProjectViewModel?> GetProjectByIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new project.
    /// </summary>
    Task<ProjectViewModel> CreateProjectAsync(
        CreateProjectViewModel viewModel,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    Task<ProjectViewModel> UpdateProjectAsync(
        EditProjectViewModel viewModel,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a project.
    /// </summary>
    Task DeleteProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available clients for project creation.
    /// </summary>
    Task<List<ClientViewModel>> GetAvailableClientsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available managers for project assignment.
    /// </summary>
    Task<List<ManagerViewModel>> GetAvailableManagersAsync(
        CancellationToken cancellationToken = default);
}
