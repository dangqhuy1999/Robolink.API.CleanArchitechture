using Microsoft.AspNetCore.Components;
using Robolink.Shared.DTOs;

namespace Robolink.WebApp.Components.Features.PhaseTasks.Shared
{
    public partial class PhaseInfoCard : ComponentBase
    {
        [Parameter]
        public ProjectPhaseConfigDto? PhaseConfig { get; set; }

        private int GetCompletedCount()
        {
            return PhaseConfig?.Tasks?.Count(t => (int)t.Status == 2) ?? 0;
        }
    }
}
