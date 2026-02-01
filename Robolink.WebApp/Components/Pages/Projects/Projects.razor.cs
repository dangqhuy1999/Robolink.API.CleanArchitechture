using MediatR;
using Robolink.Application.Commands.Projects;
using Robolink.Application.DTOs;
using Robolink.Application.Queries.Projects;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Components.Pages.Projects;

public partial class Projects
{
    [Inject] private IMediator Mediator { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

    // ✅ VARIABLES
    private List<ProjectDto>? projects;
    private bool isLoading = true;
    private bool showCreateModal = false;
    private bool showEditModal = false;
    private bool showQuickAddSubModal = false;
    private Guid selectedProjectId = Guid.Empty;

    // Pagination
    private int currentPage = 1;
    private int pageSize = 10;
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
            var result = await Mediator.Send(new GetProjectsPagedQuery(startIndex, pageSize));
            
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

    // ✅ QUICK ADD PROJECT
    private async Task IncrementCount()
    {
        var result = await Mediator.Send(new CreateProjectCommand()
        {
            CreatedBy = "Huy Dang",
            Request = new CreateProjectRequest()
            {
                ProjectCode = "APTX-" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper(),
                Name = $"Project {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                Description = "Auto created project",
                ClientId = Guid.Parse("188cd869-567e-4cd2-870a-48bdb04af5cd"),
                ManagerId = Guid.Parse("1b8c3dbf-63bb-4207-b108-9b28706185a7"),
                StartDate = DateTime.UtcNow,
                Deadline = DateTime.Today.AddDays(30),
                Priority = 1,
                InternalBudget = 1000,
                CustomerBudget = 2000
            }
        });

        if (result != null)
        {
            currentPage = 1;
            await LoadProjects();
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
        var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure?");
        if (confirmed)
        {
            try
            {
                await Mediator.Send(new DeleteProjectCommand(projectId));
                await LoadProjects();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Error: {ex.Message}");
            }
        }
    }

    // ✅ SETTINGS
    private void ShowSettings()
    {
        JSRuntime.InvokeVoidAsync("alert", "Settings not yet implemented");
    }
}