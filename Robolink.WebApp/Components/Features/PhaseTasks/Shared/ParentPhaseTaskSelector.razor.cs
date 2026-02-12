using MediatR;
using Microsoft.AspNetCore.Components;
using Robolink.Application.DTOs;
using Robolink.Application.Queries.PhaseTasks;

namespace Robolink.WebApp.Components.Features.PhaseTasks.Shared
{
    public partial class ParentPhaseTaskSelector : ComponentBase
    {
        [Parameter] public Guid ProjectId { get; set; } // Cha phải truyền cái này vào
        [Parameter] public Guid ProjectSystemPhaseConfigId { get; set; }   // Và cái này nữa

        [Parameter]
        public Guid? SelectedParentId { get; set; }

        [Parameter]
        public EventCallback<Guid?> SelectedParentIdChanged { get; set; }

        [Parameter]
        public Guid? ExcludePhaseTaskId { get; set; }

        [Parameter]
        public string? CurrentParentName { get; set; }

        [Inject]
        private IMediator Mediator { get; set; } = null!;

        private List<PhaseTaskDto> AvailablePhaseTasks = new();

        private Guid _lastProjectId;
        private Guid _lastPhaseId;

        protected override async Task OnParametersSetAsync()
        {
            // Chỉ load lại khi ID thực sự thay đổi
            if (ProjectId != _lastProjectId || ProjectSystemPhaseConfigId != _lastPhaseId)
            {
                await RefreshPhaseTasks();
            }
        }
        private async Task OnValueChanged(Guid? value)
        {
            // Không gán trực tiếp SelectedParentId ở đây để tuân thủ Data Flow của Blazor
            await SelectedParentIdChanged.InvokeAsync(value);
        }

        private async Task ForceRefresh()
        {
            _lastProjectId = Guid.Empty; // Ép buộc load lại
            await RefreshPhaseTasks();
        }

        private async Task RefreshPhaseTasks()
        {
            if (ProjectId == Guid.Empty || ProjectSystemPhaseConfigId == Guid.Empty) return;

            try
            {
                _lastProjectId = ProjectId;
                _lastPhaseId = ProjectSystemPhaseConfigId;

                var result = await Mediator.Send(new GetPhaseTasksQuery(ProjectId, ProjectSystemPhaseConfigId));
                AvailablePhaseTasks = result?.ToList() ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
