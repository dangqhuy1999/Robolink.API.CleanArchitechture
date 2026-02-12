using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.Application.Commands.Projects;
using Robolink.Application.DTOs;
using Robolink.Application.Queries.Projects;
using Robolink.WebApp.Components.Features.Projects.Shared;
namespace Robolink.WebApp.Components.Pages.Projects;

public partial class Projects : ComponentBase
{
    [Inject] private IMediator Mediator { get; set; } = null!;
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
    private int pageSize = PhaseTaskonstants.DefaultPageSize;
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
            var result = await Mediator.Send(new GetPhaseTasksPagedQuery(startIndex, pageSize));
            
            // ✅ FIX: Filter to show ONLY main projects (ParentProjectId == null)
            projects = result.Items
                .Where(p => !p.ParentProjectId.HasValue)  // ✅ Only main projects
                .ToList();

            totalProjects = result.TotalCount;
            totalPages = (int)Math.Ceiling((double)totalProjects / pageSize);

            System.Diagnostics.Debug.WriteLine($"📊 Loaded {projects.Count} main projects");
            foreach (var proj in projects)
            {
                System.Diagnostics.Debug.WriteLine($"  ✅ {proj.ProjectCode} - Subs: {proj.SubProjects?.Count ?? 0}");
            }
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
            var result = await Mediator.Send(new CreateProjectCommand()
            {
                CreatedBy = "Huy Dang",
                Request = new CreateProjectRequest()
                {
                    ProjectCode = $"{PhaseTaskonstants.ProjectCodePrefix}-{Guid.NewGuid().ToString().Substring(0, 5).ToUpper()}",
                    Name = $"Project {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    Description = "Auto created project",
                    ClientId = PhaseTaskonstants.DefaultClientId,
                    ManagerId = PhaseTaskonstants.DefaultManagerId,
                    StartDate = DateTime.UtcNow,
                    Deadline = DateTime.Today.AddDays(PhaseTaskonstants.DefaultProjectDurationDays),
                    Priority = PhaseTaskonstants.DefaultProjectPriority,
                    InternalBudget = PhaseTaskonstants.DefaultInternalBudget,
                    CustomerBudget = PhaseTaskonstants.DefaultCustomerBudget
                }
            });

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
                await Mediator.Send(new DeleteProjectCommand(projectId));
                await LoadProjects();
                await JSRuntime.InvokeVoidAsync("alert", "Project deleted successfully!");
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