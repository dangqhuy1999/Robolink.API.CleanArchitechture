using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Components.Features.SystemPhases.Shared
{
    public partial class SystemPhaseUsageBadge : ComponentBase
    {
        [Parameter]
        public int UsageCount { get; set; }
    }
}
