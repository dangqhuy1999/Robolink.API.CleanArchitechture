using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Components.Features.Projects.Shared
{
    public partial class ProjectPriorityBadge : ComponentBase
    {
        [Parameter] public int Priority { get; set; }
    }
}
