using Microsoft.AspNetCore.Components;
using Robolink.Application.DTOs;

namespace Robolink.WebApp.Components.Features.PhaseTasks.Tables
{
    public partial class RenderTreeNode : ComponentBase
    {
        [Parameter]
        public PhaseTaskDto PhaseTask { get; set; } = null!;

        [Parameter]
        public bool IsExpanded { get; set; } = false;

        [Parameter]
        public HashSet<Guid> ExpandedNodes { get; set; } = new();

        [Parameter]
        public EventCallback<Guid> OnToggle { get; set; }

        [Parameter]
        public EventCallback<Guid> OnSelectPhaseTask { get; set; }

        [Parameter]
        public EventCallback<Guid> OnAddSubPhaseTask { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDelete { get; set; }

        [Parameter]
        public int Level { get; set; } = 0;

        // ✅ NEW: Track dropdown visibility per node
        private bool ShowSubPhaseTasks = false;
    }
}
