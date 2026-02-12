using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Components.Features.PhaseTasks.Shared
{
    public partial class TaskPriorityBadge : ComponentBase
    {
        [Parameter]
        public int Priority { get; set; }
    }
}
