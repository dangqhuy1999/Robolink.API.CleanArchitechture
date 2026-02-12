using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.Application.Commands.SystemPhases;

namespace Robolink.WebApp.Components.Features.SystemPhases.Modals
{
    public partial class CreateSystemPhaseModal : ComponentBase
    {
        [Inject] protected IMediator Mediator { get; set; } = null!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
        [Parameter]
        public bool ShowModal { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        [Parameter]
        public EventCallback OnSaved { get; set; }

        private string formName = "";
        private string formDescription = "";
        private int formSequence = 1;
        private bool formIsActive = true;
        private string errorMessage = "";

        private async Task SavePhase()
        {
            try
            {
                errorMessage = "";

                if (string.IsNullOrWhiteSpace(formName))
                {
                    errorMessage = "Phase name is required";
                    return;
                }

                var command = new CreateSystemPhaseCommand(
                    formName,
                    formDescription,
                    formSequence,
                    formIsActive
                );

                await Mediator.Send(command);
                await OnSaved.InvokeAsync();
                await OnClose.InvokeAsync();

                // Reset form
                formName = "";
                formDescription = "";
                formSequence = 1;
                formIsActive = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
    }
}
