using Microsoft.AspNetCore.Components;
using Robolink.Shared.Enums;

namespace Robolink.WebApp.Modules.ProjectManagement.Shared.Components.DataDisplay.Badge_Tag
{
    public partial class ProjectPriorityBadge : ComponentBase
    {
        [Parameter] public ProjectPriority Priority { get; set; }
    }
}
