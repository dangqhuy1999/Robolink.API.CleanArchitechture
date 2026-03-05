using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Components.Modals;

/// <summary>
/// Modal component for creating new projects.
/// 
/// Logic breakdown:
/// - CreateProjectModal.razor.cs (this file): Initialization and lifecycle
/// - CreateProjectModal.State.cs: State container
/// - CreateProjectModal.DataLoading.cs: API calls and data fetching
/// - CreateProjectModal.Validation.cs: Form validation
/// - CreateProjectModal.Handlers.cs: Event handling
/// - CreateProjectModal.razor: Pure markup (UI only)
/// </summary>
public partial class CreateProjectModal : ComponentBase, IAsyncDisposable
{
    // ===== COMPONENT PARAMETERS =====
    /// <summary>
    /// Controls visibility of the modal.
    /// </summary>
    [Parameter]
    public bool ShowModal { get; set; }

    /// <summary>
    /// Optional parent project ID (for creating sub-projects).
    /// </summary>
    [Parameter]
    public Guid? ParentProjectId { get; set; }

    /// <summary>
    /// Callback when modal is closed.
    /// </summary>
    [Parameter]
    public EventCallback OnClose { get; set; }

    /// <summary>
    /// Callback when project is successfully created.
    /// </summary>
    [Parameter]
    public EventCallback OnSaved { get; set; }

    // ===== STATE =====
    private CreateProjectModalState State { get; set; } = new();

    // ===== LIFECYCLE =====
    protected override async Task OnParametersSetAsync()
    {
        if (ShowModal)
        {
            Logger.LogInformation("CreateProjectModal opened");
            State.ResetAll();
            InitializeFormData();
            await LoadModalDataAsync();
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        Logger.LogInformation("CreateProjectModal disposed");
    }
}
