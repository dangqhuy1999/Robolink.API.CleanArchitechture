using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.Application.Commands.SystemPhases;
using Robolink.Application.DTOs;
using Robolink.Application.Queries.SystemPhases;

namespace Robolink.WebApp.Components.Pages.SystemPhases
{
    public partial class SystemPhaseDetail : ComponentBase
    {
        [Inject] private IMediator Mediator { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!; // ✅ NEW
        [Parameter]
        public Guid PhaseId { get; set; }

        private SystemPhaseDto? phase;
        private bool isLoading = true;
        private bool EditMode = false;
        private int UsageCount = 0;

        private string formName = "";
        private string formDescription = "";
        private int formSequence = 1;
        private bool formIsActive = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadPhase();
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
                await JSRuntime.InvokeVoidAsync("alert", $"Error: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task SaveChanges()
        {
            try
            {
                var command = new UpdateSystemPhaseCommand(
                    PhaseId,
                    formName,
                    formDescription,
                    formSequence,
                    formIsActive
                );
                await Mediator.Send(command);
                await LoadPhase();
                EditMode = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Error: {ex.Message}");
            }
        }

        private void GoBack()
        {
            NavigationManager.NavigateTo("/system-phases");
        }
    }
}
