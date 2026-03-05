
using Robolink.WebApp.Shared.Services.Clients;
using Robolink.WebApp.Shared.Services.Staffs;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Mappers;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;
using Robolink.WebApp.Shared.Services.ApiError;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Services;

/// <inheritdoc />
public class ProjectService : IProjectService
{
    private readonly IProjectApi _projectApi;
    private readonly IClientApi _clientApi;
    private readonly IStaffApi _staffApi;
    private readonly IApiErrorHandler _errorHandler;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(
        IProjectApi projectApi,
        IClientApi clientApi,
        IStaffApi staffApi,
        IApiErrorHandler errorHandler,
        ILogger<ProjectService> logger)
    {
        _projectApi = projectApi ?? throw new ArgumentNullException(nameof(projectApi));
        _clientApi = clientApi ?? throw new ArgumentNullException(nameof(clientApi));
        _staffApi = staffApi ?? throw new ArgumentNullException(nameof(staffApi));
        _errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<(List<ProjectViewModel> Projects, int TotalCount)> GetProjectsPaginatedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            int skipCount = (pageNumber - 1) * pageSize;

            var result = await _projectApi.GetProjectsPagedAsync(skipCount, pageSize, searchTerm);

            if (result?.Items == null)
            {
                return ([], 0);
            }

            // Filter parent projects and map to ViewModels
            var projects = result.Items
                .Where(p => p.ParentProjectId == null)
                .Select(ProjectUiMapper.ToViewModel)
                .ToList();

            _logger.LogInformation(
                "Projects loaded successfully. Count: {ProjectCount}, Page: {PageNumber}",
                projects.Count, pageNumber);

            return (projects, result.TotalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading projects. Page: {PageNumber}", pageNumber);
            await _errorHandler.HandleAsync(ex, "Loading projects");
            return ([], 0);
        }
    }

    public async Task<ProjectViewModel?> GetProjectByIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var dto = await _projectApi.GetByIdAsync(projectId);

            if (dto == null)
            {
                _logger.LogWarning("Project not found. ID: {ProjectId}", projectId);
                return null;
            }

            return ProjectUiMapper.ToViewModel(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading project. ID: {ProjectId}", projectId);
            await _errorHandler.HandleAsync(ex, "Loading project");
            return null;
        }
    }

    public async Task<ProjectViewModel> CreateProjectAsync(
        CreateProjectViewModel viewModel,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        try
        {
            var request = ProjectUiMapper.ToCreateRequest(viewModel);
            var dto = await _projectApi.CreateAsync(request);

            _logger.LogInformation("Project created successfully. ID: {ProjectId}, Name: {ProjectName}",
                dto.Id, dto.Name);

            return ProjectUiMapper.ToViewModel(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project");
            await _errorHandler.HandleAsync(ex, "Creating project");
            throw;
        }
    }

    public async Task<ProjectViewModel> UpdateProjectAsync(
        EditProjectViewModel viewModel,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        try
        {
            var request = ProjectUiMapper.ToUpdateRequest(viewModel);
            var dto = await _projectApi.UpdateAsync(viewModel.Id, request);

            _logger.LogInformation("Project updated successfully. ID: {ProjectId}, Name: {ProjectName}",
                dto.Id, dto.Name);

            return ProjectUiMapper.ToViewModel(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project. ID: {ProjectId}", viewModel.Id);
            await _errorHandler.HandleAsync(ex, "Updating project");
            throw;
        }
    }

    public async Task DeleteProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _projectApi.DeleteAsync(projectId);

            _logger.LogInformation("Project deleted successfully. ID: {ProjectId}", projectId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project. ID: {ProjectId}", projectId);
            await _errorHandler.HandleAsync(ex, "Deleting project");
            throw;
        }
    }

    public async Task<List<ClientViewModel>> GetAvailableClientsAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _clientApi.GetAllClientsAsync(0, 100);
            return ProjectUiMapper.ToClientViewModels(result?.Items ?? []);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error loading clients");
            await _errorHandler.HandleAsync(ex, "Loading clients");
            return [];
        }
    }

    public async Task<List<ManagerViewModel>> GetAvailableManagersAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _staffApi.GetAllStaffsAsync(0, 100);
            return ProjectUiMapper.ToManagerViewModels(result?.Items ?? []);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error loading managers");
            await _errorHandler.HandleAsync(ex, "Loading managers");
            return [];
        }
    }
}
