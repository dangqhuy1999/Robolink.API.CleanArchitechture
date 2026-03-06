
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.Shared.DTOs;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Components.Layout.DataLoader;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Services;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;
using Robolink.WebApp.Shared.Services.NotificationService;

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
public partial class Projects : ComponentBase
{
    // ===== INJECTED SERVICES =====
    [Inject] private IProjectService ProjectService { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IToastNotificationService ToastNotificationService { get; set; } = null!;
    [Inject] private ILogger<Projects> Logger { get; set; } = null!;

    // ===== STATE =====
    protected ProjectsPageState State { get; set; } = new();

    protected DataLoader<ProjectViewModel> _loader = null!;

    // Page bây giờ KHÔNG CẦN:
    // - OnInitializedAsync (DataLoader tự làm)
    // - IAsyncDisposable (DataLoader tự làm)
    // - CancellationTokenSource (DataLoader tự làm)


}