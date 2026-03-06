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
    /// 
    private void HandleDataReceived(PaginatedResult<ProjectViewModel> result)
    {
        // Đây chính là đoạn code cũ của em, chỉ khác là nó nằm gọn trong 1 hàm
        State.Projects = result.Items.ToList();
        State.TotalProjects = result.TotalCount;
        State.TotalPages = (int)Math.Ceiling((double)result.TotalCount / State.PageSize);
        Logger.LogInformation("Projects loaded. Count: {Count}", State.Projects.Count);
    }

    /*
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
    */
    /// <summary>
    /// Navigates to a specific page.
    /// Validates page number before navigation.
    /// </summary>
    /// 
    
    private async Task HandlePageChangedAsync(int pageNumber)
    {
        // Vi da check tai component con, nhưng check lại ở đây để đảm bảo tính toàn vẹn (nếu có lỗi logic ở component con)
        // 1. Chặn click trùng trang
        if (pageNumber == State.CurrentPage) return;

        Logger.LogInformation("Switching to page {PageNumber}", pageNumber);

        // 2. Cập nhật State và Load lần 1
        State.CurrentPage = pageNumber;
        await _loader.RefreshAsync(); // Dùng DataLoader để tự động gọi HandleDataReceived khi có dữ liệu mới
        //await HandleLoadProjectsAsync();

        // 3. Xử lý trường hợp "lệch pha" (Edge Case)
        // Ví dụ: Đang ở trang 10, nhưng ai đó vừa xóa bớt dự án khiến chỉ còn 8 trang.
        // Sau khi Load lần 1, State.TotalPages sẽ được cập nhật con số mới (ví dụ = 8).
        if (State.CurrentPage > State.TotalPages && State.TotalPages > 0)
        {
            Logger.LogWarning("Page {Current} is out of range. Redirecting to last page {Total}",
                               State.CurrentPage, State.TotalPages);

            State.CurrentPage = State.TotalPages;

            // 4. Load lần 2: Lấy dữ liệu thực tế của trang cuối cùng
            await _loader.RefreshAsync(); // Dùng DataLoader để tự động gọi HandleDataReceived khi có dữ liệu mới
        }
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

            await _loader.RefreshAsync(); // Dùng DataLoader để tự động gọi HandleDataReceived khi có dữ liệu mới
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
        await _loader.RefreshAsync(); // Dùng DataLoader để tự động gọi HandleDataReceived khi có dữ liệu mới
    }
}