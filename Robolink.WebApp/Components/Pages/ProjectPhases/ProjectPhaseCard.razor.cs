using Microsoft.AspNetCore.Components;
using Robolink.Application.DTOs;

namespace Robolink.WebApp.Components.Pages.ProjectPhases
{
    public partial class ProjectPhaseCard : ComponentBase
    {
        [Parameter] public ProjectPhaseConfigDto Phase { get; set; } = new();
        [Parameter] public EventCallback<Guid> OnClick { get; set; }

        private string PhaseName => Phase.CustomPhaseName ?? Phase.SystemPhase?.Name ?? "Unknown Phase";

        private async Task HandleClick()
        {
            await OnClick.InvokeAsync(Phase.Id);
        }
    }
}
