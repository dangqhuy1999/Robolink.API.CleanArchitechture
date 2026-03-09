using Microsoft.AspNetCore.Components;
using Robolink.Shared.DTOs;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.PhaseTasks.Shared
{
    public partial class SubPhaseTasksList : ComponentBase
    {
        [Parameter]
        public List<PhaseTaskDto>? SubPhaseTasks { get; set; }

        [Parameter]
        public EventCallback<Guid> OnSelectSubPhaseTask { get; set; }
    }
}
