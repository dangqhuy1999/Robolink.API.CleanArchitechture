using Microsoft.AspNetCore.Components;
using Robolink.Shared.DTOs;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;

namespace Robolink.WebApp.Components.Features.Projects.Tables
{
    /// <summary>
    /// Hierarchical tree view for projects with expand/collapse functionality.
    /// 
    /// Architecture:
    /// - razor.cs: Parameters + Lifecycle
    /// - State.cs: Data state + filtering
    /// - Handlers.cs: User interactions
    /// - razor: Pure markup
    /// </summary>
    public partial class ProjectHierarchyTree : ComponentBase
    {
        // ===== PARAMETERS (From Parent) =====
        [Parameter]
        public List<ProjectViewModel> AllProjects { get; set; } = new();

        [Parameter]
        public EventCallback<Guid> OnSelectProject { get; set; }

        [Parameter]
        public EventCallback<Guid> OnAddSubProject { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDelete { get; set; }

        // State & Handlers defined in partial classes
    }
}
