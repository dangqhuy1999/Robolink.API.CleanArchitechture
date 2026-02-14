using Microsoft.AspNetCore.Components;
using Robolink.Shared.DTOs;

namespace Robolink.WebApp.Components.Features.Projects.Tables
{
    public partial class ProjectTable : ComponentBase
    {
        [Parameter]
        public List<ProjectDto> Projects { get; set; } = new();

        [Parameter]
        public EventCallback<Guid> OnEdit { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDelete { get; set; }

        [Parameter]
        public EventCallback<Guid> OnAddSubProject { get; set; }

        [Parameter]
        public EventCallback<Guid> OnViewProject { get; set; } // ✅ NEW: Event for viewing project

        [Parameter]
        public EventCallback OnRefresh { get; set; }

        private HashSet<Guid> expandedProjects = new();

        // ✅ NEW: Handle View/Navigate to Project Management
        private async Task HandleViewProject(Guid projectId)
        {
            await OnViewProject.InvokeAsync(projectId);
        }

        private async Task HandleEdit(Guid projectId)
        {
            await OnEdit.InvokeAsync(projectId);
        }

        private async Task HandleDelete(Guid projectId)
        {
            await OnDelete.InvokeAsync(projectId);
        }

        private async Task HandleAddSub(Guid projectId)
        {
            await OnAddSubProject.InvokeAsync(projectId);
        }

        private void HandleToggleExpand(Guid projectId)
        {
            if (expandedProjects.Contains(projectId))
            {
                expandedProjects.Remove(projectId);
            }
            else
            {
                expandedProjects.Add(projectId);
            }
        }

        private bool IsExpanded(Guid projectId) => expandedProjects.Contains(projectId);
    }
}
