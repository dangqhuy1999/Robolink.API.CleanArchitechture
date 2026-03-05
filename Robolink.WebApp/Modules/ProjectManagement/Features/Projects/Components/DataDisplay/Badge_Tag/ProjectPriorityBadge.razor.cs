using Microsoft.AspNetCore.Components;
using Robolink.Shared.Enums;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Shared
{
    public partial class ProjectPriorityBadge : ComponentBase
    {
        [Parameter] public ProjectPriority Priority { get; set; }
    }
}
