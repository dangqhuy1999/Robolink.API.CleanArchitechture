using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.Application.Commands.SystemPhases;
using Robolink.Application.DTOs;
using Robolink.Application.Queries.SystemPhases;

namespace Robolink.WebApp.Components.Features.SystemPhases.Modals
{
    public partial class EditSystemPhaseModal : ComponentBase
    {
        [Inject] protected IMediator Mediator { get; set; } = null!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
        [Parameter]
        public bool ShowModal { get; set; }

        [Parameter]
        public Guid PhaseId { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        [Parameter]
        public EventCallback OnSaved { get; set; }

        private SystemPhaseDto? phase;
        private string formName = "";
        private string formDescription = "";
        private int formSequence = 1;
        private bool formIsActive = true;
        private string errorMessage = "";

        protected override async Task OnParametersSetAsync()
        {
            if (ShowModal && PhaseId != Guid.Empty)
            {
                await LoadPhase();
            }
        }

        private async Task LoadPhase()
        {
            try
            {
                var query = new GetAllSystemPhasesQuery();
                var phases = await Mediator.Send(query);
                phase = phases?.FirstOrDefault(p => p.Id == PhaseId);

                if (phase != null)
                {
                    formName = phase.Name;
                    formDescription = phase.Description ?? "";
                    formSequence = phase.DefaultSequence;
                    formIsActive = phase.IsActive;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        private async Task SavePhase()
        {
            try
            {
                errorMessage = "";

                var command = new UpdateSystemPhaseCommand(
                    PhaseId,
                    formName,
                    formDescription,
                    formSequence,
                    formIsActive
                );

                await Mediator.Send(command);
                await OnSaved.InvokeAsync();
                await OnClose.InvokeAsync();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
    }
}
