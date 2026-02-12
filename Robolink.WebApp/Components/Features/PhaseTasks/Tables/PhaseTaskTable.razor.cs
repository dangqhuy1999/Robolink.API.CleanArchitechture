using Microsoft.AspNetCore.Components;
using Robolink.Application.DTOs;

namespace Robolink.WebApp.Components.Features.PhaseTasks.Tables
{
    public partial class PhaseTaskTable : ComponentBase
    {
        [Parameter]
        public List<PhaseTaskDto> PhaseTasks { get; set; } = new();

        [Parameter]
        public EventCallback<Guid> OnEdit { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDelete { get; set; }

        [Parameter]
        public EventCallback<Guid> OnAddSubPhaseTask { get; set; }

        [Parameter]
        public EventCallback<Guid> OnViewPhaseTask { get; set; } // ✅ NEW: Event for viewing project

        [Parameter]
        public EventCallback OnRefresh { get; set; }

        private HashSet<Guid> expandedPhaseTasks = new();




        // ✅ NEW: Handle View/Navigate to Project Management
        private async Task HandleViewPhaseTask(Guid phaseTaskId)
        {
            await OnViewPhaseTask.InvokeAsync(phaseTaskId);
        }

        private async Task HandleEdit(Guid projectId)
        {
            await OnEdit.InvokeAsync(projectId);
        }

        private async Task HandleDelete(Guid projectId)
        {
            await OnDelete.InvokeAsync(projectId);
        }

        private async Task HandleAddSub(Guid projectId)
        {
            await OnAddSubPhaseTask.InvokeAsync(projectId);
        }

        private void HandleToggleExpand(Guid projectId)
        {
            if (expandedPhaseTasks.Contains(projectId))
            {
                expandedPhaseTasks.Remove(projectId);
            }
            else
            {
                expandedPhaseTasks.Add(projectId);
            }
        }

        private bool IsExpanded(Guid projectId) => expandedPhaseTasks.Contains(projectId);
    }
}
