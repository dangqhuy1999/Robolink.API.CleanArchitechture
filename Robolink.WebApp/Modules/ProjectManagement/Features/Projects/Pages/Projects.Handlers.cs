using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;
using Robolink.WebApp.Shared.Services.NotificationService; 

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Pages;

/// <summary>
/// Handles all user interactions and events for the Projects page.
/// Each method represents a single user action.
/// </summary>
public partial class Projects : ComponentBase
{
    /// <summary>
    /// Loads projects data with pagination.
    /// Called on page init and after data changes.
    /// </summary>
    private async Task HandleLoadProjectsAsync()
    {
        try
        {
            State.IsLoading = true;

            var (projects, totalCount) = await ProjectService.GetProjectsPaginatedAsync(
                State.CurrentPage,
                State.PageSize,
                cancellationToken: _cancellationTokenSource?.Token ?? default);

            State.Projects = projects;
            State.TotalProjects = totalCount;
            State.TotalPages = (int)Math.Ceiling((double)State.TotalProjects / State.PageSize);

            Logger.LogInformation(
                "Projects loaded successfully. Count: {Count}, Page: {Page}/{Total}",
                State.Projects.Count, State.CurrentPage, State.TotalPages);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading projects");
            State.Projects = [];
        }
        finally
        {
            State.IsLoading = false;
        }
    }

    /// <summary>
    /// Navigates to a specific page.
    /// Validates page number before navigation.
    /// </summary>
    /// 
    
    private async Task HandlePageChangedAsync(int pageNumber)
    {
        if (pageNumber < 1 || pageNumber > State.TotalPages)
        {
            Logger.LogWarning("Invalid page number: {PageNumber}", pageNumber);
            return;
        }

        State.CurrentPage = pageNumber;
        await HandleLoadProjectsAsync();
    }
    /// <summary>
    /// Handles project deletion with confirmation.
    /// Shows warning if project has sub-projects.
    /// </summary>
    private async Task HandleDeleteProjectAsync(Guid projectId)
    {
        // Find project in current list
        var projectToDelete = State.Projects.FirstOrDefault(p => p.Id == projectId);
        if (projectToDelete == null)
        {
            Logger.LogWarning("Attempted to delete non-existent project. ID: {ProjectId}", projectId);
            return;
        }

        // Build confirmation message
        string message = BuildDeletionMessage(projectToDelete);

        // Show confirmation dialog
        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", message);
        if (!confirmed)
        {
            return;
        }

        // Execute deletion
        await ExecuteProjectDeletionAsync(projectId);
    }

    /// <summary>
    /// Executes project deletion and refreshes list.
    /// </summary>
    private async Task ExecuteProjectDeletionAsync(Guid projectId)
    {
        try
        {
            await ProjectService.DeleteProjectAsync(projectId);

            Logger.LogInformation("Project deleted successfully. ID: {ProjectId}", projectId);

            await ToastNotificationService.ShowSuccessAsync("Project deleted successfully.");

            // Reset to first page if needed
            if (State.Projects.Count <= 1)
            {
                State.CurrentPage = 1;
            }

            await HandleLoadProjectsAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting project. ID: {ProjectId}", projectId);
        }
    }

    /// <summary>
    /// Builds appropriate deletion confirmation message.
    /// Warns about sub-project deletion if applicable.
    /// </summary>
    private static string BuildDeletionMessage(ProjectViewModel project)
    {
        if (project.SubProjectsCount > 0)
        {
            return $"WARNING: This project has {project.SubProjectsCount} sub-project(s). " +
                   $"Deleting '{project.Name}' will also delete all sub-projects. Continue?";
        }

        return $"Are you sure you want to delete '{project.Name}'?";
    }

    /// <summary>
    /// Refreshes the projects list (manual refresh).
    /// </summary>
    private async Task HandleRefreshAsync()
    {
        State.ResetPagination();
        await HandleLoadProjectsAsync();
    }
}