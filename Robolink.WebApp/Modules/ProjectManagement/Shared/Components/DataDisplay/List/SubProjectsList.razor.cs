using Microsoft.AspNetCore.Components;
using Robolink.Shared.DTOs;

namespace Robolink.WebApp.Modules.ProjectManagement.Shared.Components.DataDisplay.List
{
    public partial class SubProjectsList : ComponentBase
    {
        [Parameter]
        public List<ProjectDto>? SubProjects { get; set; }

        [Parameter]
        public EventCallback<Guid> OnSelectSubProject { get; set; }
    }
}
