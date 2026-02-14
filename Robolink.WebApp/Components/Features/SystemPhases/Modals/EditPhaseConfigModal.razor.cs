using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.Application.Commands.ProjectPhases;
using Robolink.Shared.DTOs;

namespace Robolink.WebApp.Components.Features.SystemPhases.Modals
{
    public partial class EditPhaseConfigModal : ComponentBase
    {
        [Inject] protected IMediator Mediator { get; set; } = null!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
        [Parameter]
        public bool ShowModal { get; set; }

        [Parameter]
        public ProjectPhaseConfigDto? Phase { get; set; }

        [Parameter]
        public EventCallback OnModalClose { get; set; }

        [Parameter]
        public EventCallback OnConfigChanged { get; set; }

        private string? CustomPhaseName;
        private int Sequence;
        private bool IsEnabled;
        private string? ErrorMessage;

        protected override void OnParametersSet()
        {
            if (Phase != null)
            {
                CustomPhaseName = Phase.CustomPhaseName;
                Sequence = Phase.Sequence;
                IsEnabled = Phase.IsEnabled;
                ErrorMessage = null;
            }
        }

        private async Task SaveChanges()
        {
            try
            {
                ErrorMessage = null;
                var command = new UpdateProjectPhaseConfigCommand(
                    Phase!.Id,
                    CustomPhaseName,
                    Sequence,
                    IsEnabled
                );
                await Mediator.Send(command);
                await OnConfigChanged.InvokeAsync();
                await OnClose();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private async Task RemovePhase()
        {
            if (!await JSRuntime.InvokeAsync<bool>("confirm", "Remove this phase from project?"))
                return;

            try
            {
                ErrorMessage = null;
                var command = new RemovePhaseFromProjectCommand(Phase!.Id);
                await Mediator.Send(command);
                await OnConfigChanged.InvokeAsync();
                await OnClose();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private async Task OnClose()
        {
            await OnModalClose.InvokeAsync();
        }
    }
}
