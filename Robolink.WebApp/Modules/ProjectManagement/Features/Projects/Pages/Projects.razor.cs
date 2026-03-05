
using Microsoft.AspNetCore.Components;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Services;
using Robolink.WebApp.Shared.Services.NotificationService;
using Microsoft.JSInterop;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Pages;

/// <summary>
/// Projects page component.
/// Orchestrates project listing with CRUD operations.
/// 
/// Logic breakdown:
/// - Projects.razor.cs (this file): Initialization and core lifecycle
/// - Projects.State.cs: State management
/// - Projects.Handlers.cs: Event handling and data operations
/// - Projects.Modals.cs: Modal show/hide operations
/// - Projects.Navigation.cs: Page navigation
/// - Projects.razor: Pure markup (UI only)
/// </summary>
public partial class Projects : ComponentBase, IAsyncDisposable
{
    // ===== INJECTED SERVICES =====
    [Inject] private IProjectService ProjectService { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IToastNotificationService ToastNotificationService { get; set; } = null!;
    [Inject] private ILogger<Projects> Logger { get; set; } = null!;

    // ===== STATE =====
    private ProjectsPageState State { get; set; } = new();

    // ===== LIFECYCLE =====
    private CancellationTokenSource? _cancellationTokenSource;

    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation("Projects page initializing");
        await HandleLoadProjectsAsync();
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        _cancellationTokenSource?.Dispose();
        Logger.LogInformation("Projects page disposed");
    }
}