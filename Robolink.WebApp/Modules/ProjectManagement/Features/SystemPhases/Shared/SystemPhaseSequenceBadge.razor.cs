using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.SystemPhases.Shared
{
    public partial class SystemPhaseSequenceBadge : ComponentBase
    {
        [Parameter]
        public int Sequence { get; set; }
    }
}
