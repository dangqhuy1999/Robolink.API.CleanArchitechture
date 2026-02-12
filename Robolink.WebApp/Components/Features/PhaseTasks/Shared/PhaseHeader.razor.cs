using Microsoft.AspNetCore.Components;
using Robolink.Application.DTOs;

namespace Robolink.WebApp.Components.Features.PhaseTasks.Shared
{
    public partial class PhaseHeader : ComponentBase
    {
        [Parameter]
        public ProjectPhaseConfigDto? PhaseConfig { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public EventCallback OnBack { get; set; }

        [Parameter]
        public EventCallback OnAddTask { get; set; }

        [Parameter]
        public EventCallback OnRefresh { get; set; }

        private string GetPhaseName()
        {
            return PhaseConfig?.CustomPhaseName ?? PhaseConfig?.SystemPhase?.Name ?? "Unknown Phase";
        }
    }
}
