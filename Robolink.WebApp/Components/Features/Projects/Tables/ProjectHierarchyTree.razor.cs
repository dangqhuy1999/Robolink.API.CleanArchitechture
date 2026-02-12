using Microsoft.AspNetCore.Components;
using Robolink.Application.DTOs;

namespace Robolink.WebApp.Components.Features.Projects.Tables
{
    public partial class ProjectHierarchyTree : ComponentBase
    {
        [Parameter]
        public List<ProjectDto> AllProjects { get; set; } = new();

        [Parameter]
        public EventCallback<Guid> OnSelectProject { get; set; }

        [Parameter]
        public EventCallback<Guid> OnAddSubProject { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDelete { get; set; }

        private List<ProjectDto> MainProjects = new();
        private HashSet<Guid> ExpandedNodes = new();

        protected override void OnParametersSet()
        {
            MainProjects = AllProjects?.Where(p => !p.ParentProjectId.HasValue).ToList() ?? new();
        }

        /// <summary>Toggle expand/collapse state for nested children</summary>
        private async Task ToggleNode(Guid projectId)
        {
            if (ExpandedNodes.Contains(projectId))
                ExpandedNodes.Remove(projectId);
            else
                ExpandedNodes.Add(projectId);

            StateHasChanged();
            await Task.CompletedTask;
        }

        /// <summary>Check if node is expanded for nested display</summary>
        private bool IsExpanded(Guid projectId) => ExpandedNodes.Contains(projectId);
    }
}
