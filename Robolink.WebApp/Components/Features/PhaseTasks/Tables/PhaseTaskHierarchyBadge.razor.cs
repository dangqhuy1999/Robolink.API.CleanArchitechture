using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Components.Features.PhaseTasks.Tables
{
    public partial class PhaseTaskHierarchyBadge : ComponentBase
    {
        [Parameter]
        public bool HasParent { get; set; }

        [Parameter]
        public bool HasSubTasks { get; set; }

        [Parameter]
        public int SubTaskCount { get; set; } = 0;
    }
}
