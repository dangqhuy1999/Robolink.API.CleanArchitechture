using Microsoft.AspNetCore.Components;
using Robolink.Application.DTOs;

namespace Robolink.WebApp.Components.Pages.ProjectPhases
{
    public partial class AddPhaseModal : ComponentBase
    {
        [Parameter] public bool ShowModal { get; set; }
        [Parameter] public bool IsLoading { get; set; }
        [Parameter] public string? ErrorMessage { get; set; }
        [Parameter] public List<SystemPhaseDto>? AvailablePhases { get; set; }

        [Parameter] public EventCallback OnClose { get; set; }
        [Parameter] public EventCallback<Guid> OnAssign { get; set; }

        private Task CloseModal() => OnClose.InvokeAsync();
    }
}
