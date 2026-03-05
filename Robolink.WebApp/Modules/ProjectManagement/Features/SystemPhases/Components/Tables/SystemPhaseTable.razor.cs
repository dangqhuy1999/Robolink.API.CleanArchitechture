using Microsoft.AspNetCore.Components;
using Robolink.Shared.DTOs;

namespace Robolink.WebApp.Components.Features.SystemPhases.Tables
{
    public partial class SystemPhaseTable : ComponentBase
    {
        [Parameter]
        public List<SystemPhaseDto>? Phases { get; set; }

        [Parameter]
        public EventCallback<Guid> OnEdit { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDelete { get; set; }

        [Parameter]
        public EventCallback OnRefresh { get; set; }

        private Dictionary<Guid, int> UsageCount = new();

        private async Task HandleEdit(Guid phaseId)
        {
            await OnEdit.InvokeAsync(phaseId);
        }

        private async Task HandleDelete(Guid phaseId)
        {
            await OnDelete.InvokeAsync(phaseId);
        }

        private int GetUsageCount(Guid phaseId)
        {
            return UsageCount.ContainsKey(phaseId) ? UsageCount[phaseId] : 0;
        }
    }
}
