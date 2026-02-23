using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.Application.Commands.Projects;
using Robolink.Application.Queries.Projects;
using Robolink.Core.Entities;
using Robolink.Shared.DTOs;
using Robolink.Shared.Interfaces.API.Projects;
using Robolink.WebApp.Components.Features.Projects.Shared;
using System.Threading.Tasks;
namespace Robolink.WebApp.Components.Pages.Projects;

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

            projects = result.Items.ToList(); // 10 ông Cha
            totalProjects = result.TotalCount; // Ví dụ: 10
            totalPages = (int)Math.Ceiling((double)totalProjects / pageSize); // 1 trang duy nhất
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
    private async Task DeleteProject(Guid projectId)
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this project?"))
        {
            try
            {
                // ✅ Gọi qua API thay vì gọi trực tiếp Mediator
                var result = await ProjectApi.DeleteAsync(projectId);
                if (result) // Nếu Handler trả về true
                {
                    await LoadProjects();
                    await JSRuntime.InvokeVoidAsync("alert", "Project deleted successfully!");
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("alert", "Delete failed: Task might not exist.");
                }
                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting project: {ex}");
                await JSRuntime.InvokeVoidAsync("alert", $"Error deleting project: {ex.Message}");
            }
        }
    }

    // ✅ SETTINGS (TODO: Implement in future)
    private async Task ShowSettings()
    {
        await JSRuntime.InvokeVoidAsync("alert", "Settings feature coming soon!");
    }
}