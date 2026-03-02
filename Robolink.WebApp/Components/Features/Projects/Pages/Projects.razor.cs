using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Refit;
using Robolink.Shared.DTOs;
using Robolink.Shared.Interfaces.API.Projects;
using Robolink.WebApp.Components.Features.Projects.Shared;
using System.Threading.Tasks;
namespace Robolink.WebApp.Components.Features.Projects.Pages;

public partial class Projects : ComponentBase
{
    [Inject] private IProjectApi ProjectApi { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!; // ✅ NEW

    // ✅ VARIABLES
    private List<ProjectDto>? projects;
    private bool isLoading = true;
    private bool showCreateModal = false;
    private bool showEditModal = false;
    private bool showQuickAddSubModal = false;
    private Guid selectedProjectId = Guid.Empty;

    // Pagination
    private int currentPage = 1;
    private int pageSize = ProjectConstants.DefaultPageSize;
    private int totalProjects = 0;
    private int totalPages = 0;

    protected override async Task OnInitializedAsync()
    {
        await LoadProjects();
    }

    // ✅ LOAD PROJECTS WITH PAGINATION
    private async Task LoadProjects()
    {
        isLoading = true;
        try
        {
            int startIndex = (currentPage - 1) * pageSize;

            // Gọi API: Server trả về đúng 10 ông Cha, trong mỗi ông Cha ĐÃ CÓ SẴN đám Con
            var result = await ProjectApi.GetProjectsPagedAsync(startIndex, pageSize);

            // ✅ Vừa lọc ông Cha, vừa đảm bảo danh sách con không bị null
            projects = result.Items
                .Where(p => p.ParentProjectId == null)
                .Select(p => {
                    p.SubProjects ??= new List<ProjectDto>();
                    return p;
                })
                .ToList();
            totalProjects = result.TotalCount; // Ví dụ: 10
            totalPages = (int)Math.Ceiling((double)totalProjects / pageSize); // 1 trang duy nhất
        }
        catch (ApiException ex) // Lỗi từ phía Server (400, 404, 500...)
        {
            // Đọc nội dung lỗi từ Server gửi về
            var errorContent = await ex.GetContentAsAsync<Dictionary<string, string>>();
            await JSRuntime.InvokeVoidAsync("alert", "Error API server: " + ex.Message);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading projects: {ex}");
            await JSRuntime.InvokeVoidAsync("alert", $"Error loading projects: {ex.Message}");
            projects = new List<ProjectDto>();
        }
        finally
        {
            isLoading = false;
        }
    }

    // ✅ GO TO PAGE
    private async Task GoToPage(int page)
    {
        if (page >= 1 && page <= totalPages)
        {
            currentPage = page;
            await LoadProjects();
        }
    }

    // ✅ NEW: NAVIGATE TO PROJECT MANAGEMENT (LEVEL 2)
    private void NavigateToProjectManagement(Guid projectId)
    {
        NavigationManager.NavigateTo($"/projects/{projectId}");
    }

    // ✅ QUICK ADD PROJECT
    private async Task QuickAddProject()
    {
        try
        {
            var result = await ProjectApi.QuickCreateAsync();

            if (result != null)
            {
                currentPage = 1;
                await LoadProjects();
                await JSRuntime.InvokeVoidAsync("alert", "Project created successfully!");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error creating quick project: {ex}");
            await JSRuntime.InvokeVoidAsync("alert", $"Error creating project: {ex.Message}"); 
        }
    }

    // ✅ QUICK ADD SUB-PROJECT
    private void ShowQuickAddSubModal(Guid parentProjectId)
    {
        selectedProjectId = parentProjectId;
        showQuickAddSubModal = true;
    }

    private void HideQuickAddSubModal()
    {
        showQuickAddSubModal = false;
        selectedProjectId = Guid.Empty;
    }

    // ✅ MODAL METHODS
    private void ShowCreateModal() => showCreateModal = true;
    
    private void HideCreateModal() 
    { 
        showCreateModal = false;
        selectedProjectId = Guid.Empty;
    }

    private void ShowEditModal(Guid projectId)
    {
        selectedProjectId = projectId;
        showEditModal = true;
    }

    private void HideEditModal()
    {
        showEditModal = false;
        selectedProjectId = Guid.Empty;
    }

    // ✅ DELETE PROJECT
    private async Task DeleteProject(Guid id)
    {
        // Tìm project trong list hiện tại để lấy thông tin
        var projectToDelete = projects?.FirstOrDefault(p => p.Id == id);
        if (projectToDelete == null) return;

        string message = $"Bạn có chắc chắn muốn xóa dự án '{projectToDelete.Name}' không?";

        // Nếu có con, thay đổi lời nhắn cảnh báo
        if (projectToDelete.SubProjectsCount > 0)
        {
            message = $"CẢNH BÁO: Dự án này có {projectToDelete.SubProjectsCount} dự án con. " +
                      "Nếu xóa, TẤT CẢ dự án con cũng sẽ bị xóa theo. Bạn vẫn muốn tiếp tục?";
        }

        // Hiện Confirm (Có thể dùng SweetAlert2 thay cho confirm này)
        bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", message);

        if (confirmed)
        {
            try
            {
                await ProjectApi.DeleteAsync(id);
                // Refresh lại danh sách
                await LoadProjects();
            }
            catch (ApiException ex) // Lỗi từ phía Server (400, 404, 500...)
            {
                // Đọc nội dung lỗi từ Server gửi về
                var errorContent = await ex.GetContentAsAsync<Dictionary<string, string>>();
                await JSRuntime.InvokeVoidAsync("alert", "Error API server: " + ex.Message);
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", "Lỗi khi xóa: " + ex.Message);
            }
        }
    }

    // ✅ SETTINGS (TODO: Implement in future)
    private async Task ShowSettings()
    {
        await JSRuntime.InvokeVoidAsync("alert", "Settings feature coming soon!");
    }
}