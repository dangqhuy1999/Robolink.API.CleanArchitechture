using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.SystemPhases.Shared
{
    public partial class SystemPhaseUsageBadge : ComponentBase
    {
        [Parameter]
        public int UsageCount { get; set; }
    }
}
