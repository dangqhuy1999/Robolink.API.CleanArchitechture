using Microsoft.AspNetCore.Components;
using Robolink.Shared.DTOs;

namespace Robolink.WebApp.Components.Features.Projects.Shared
{
    public partial class SubProjectsList : ComponentBase
    {
        [Parameter]
        public List<ProjectDto>? SubProjects { get; set; }

        [Parameter]
        public EventCallback<Guid> OnSelectSubProject { get; set; }
    }
}
