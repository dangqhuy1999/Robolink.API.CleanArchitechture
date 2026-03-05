using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Components.Features.SystemPhases.Shared
{
    public partial class SystemPhaseStatusBadge : ComponentBase
    {
        [Parameter]
        public bool IsActive { get; set; }
    }
}
