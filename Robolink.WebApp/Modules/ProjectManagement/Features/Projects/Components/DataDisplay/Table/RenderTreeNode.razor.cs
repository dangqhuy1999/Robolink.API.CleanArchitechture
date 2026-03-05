using Microsoft.AspNetCore.Components;
using Robolink.Shared.DTOs;

namespace Robolink.WebApp.Components.Features.Projects.Tables
{
    public partial class RenderTreeNode : ComponentBase
    {
        [Parameter]
        public ProjectDto Project { get; set; } = null!;

        [Parameter]
        public bool IsExpanded { get; set; } = false;

        [Parameter]
        public HashSet<Guid> ExpandedNodes { get; set; } = new();

        [Parameter]
        public EventCallback<Guid> OnToggle { get; set; }

        [Parameter]
        public EventCallback<Guid> OnSelectProject { get; set; }

        [Parameter]
        public EventCallback<Guid> OnAddSubProject { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDelete { get; set; }

        [Parameter]
        public int Level { get; set; } = 0;

        // ✅ NEW: Track dropdown visibility per node
        private bool ShowSubProjects = false;
    }
}
