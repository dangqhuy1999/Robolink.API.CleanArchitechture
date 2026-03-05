using Microsoft.AspNetCore.Components;
using Robolink.Shared.DTOs;

namespace Robolink.WebApp.Components.Features.ProjectPhases.Shared
{
    public partial class ProjectStatCards   : ComponentBase
    {
        [Parameter] public ProjectDto? Project { get; set; }
    }
}
