using Microsoft.AspNetCore.Components;
using Robolink.Application.DTOs;

namespace Robolink.WebApp.Components.Features.PhaseTasks.Shared
{
    public partial class SubPhaseTasksList : ComponentBase
    {
        [Parameter]
        public List<PhaseTaskDto>? SubPhaseTasks { get; set; }

        [Parameter]
        public EventCallback<Guid> OnSelectSubPhaseTask { get; set; }
    }
}
