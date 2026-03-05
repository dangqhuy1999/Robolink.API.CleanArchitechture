using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Pages;

/// <summary>
/// Handles all navigation operations for the Projects page.
/// Single responsibility: manage page routing.
/// </summary>
public partial class Projects : ComponentBase
{
    /// <summary>
    /// Navigates to project management page (Level 2: ProjectManagement).
    /// This is where user manages phases and tasks for a specific project.
    /// </summary>
    private void HandleNavigateToProjectManagement(Guid projectId)
    {
        if (projectId == Guid.Empty)
        {
            Logger.LogWarning("Attempted navigation with invalid project ID");
            return;
        }

        Logger.LogInformation("Navigating to project management. ProjectId: {ProjectId}", projectId);
        NavigationManager.NavigateTo($"/projects/{projectId}");
    }

}