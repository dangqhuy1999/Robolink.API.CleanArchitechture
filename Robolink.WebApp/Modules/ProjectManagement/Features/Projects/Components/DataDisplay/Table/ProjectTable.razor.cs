using Microsoft.AspNetCore.Components;
using Robolink.Shared.DTOs;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Components.Tables
{
    public partial class ProjectTable : ComponentBase
    {
        // ===== DATA STATE =====
        [Parameter]
        public List<ProjectViewModel> Projects { get; set; } = [] ;

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
