using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Components.Features.PhaseTasks.Shared
{
    public partial class TaskStatusBadge : ComponentBase
    {
        [Parameter] public int Status { get; set; }
    }
}
