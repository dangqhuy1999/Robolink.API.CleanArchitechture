using Microsoft.AspNetCore.Components;
using Robolink.Shared.DTOs;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Components.Tables
{
    /// <summary>
    /// Table component for displaying projects with expand/collapse rows.
    /// 
    /// Architecture:
    /// - razor.cs: Parameters + Lifecycle (this file)
    /// - State.cs: UI state (expanded rows, sorting)
    /// - Handlers.cs: Event handlers
    /// - razor: Pure markup
    /// </summary>
    public partial class ProjectTable : ComponentBase
    {
        // ===== PARAMETERS (From Parent) =====
        [Parameter]
        public List<ProjectViewModel> Projects { get; set; } = [];

        [Parameter]
        public EventCallback<Guid> OnEdit { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDelete { get; set; }

        [Parameter]
        public EventCallback<Guid> OnAddSubProject { get; set; }

        [Parameter]
        public EventCallback<Guid> OnViewProject { get; set; } // ✅ NEW: Event for viewing project

        private HashSet<Guid> expandedProjects = new();

        private bool IsExpanded(Guid projectId) => expandedProjects.Contains(projectId);
    }
}
