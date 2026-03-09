using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.PhaseTasks.Shared
{
    public partial class TaskStatusBadge : ComponentBase
    {
        [Parameter] public int Status { get; set; }
    }
}
