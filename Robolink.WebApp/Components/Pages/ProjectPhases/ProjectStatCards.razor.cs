using Microsoft.AspNetCore.Components;
using Robolink.Shared.DTOs;

namespace Robolink.WebApp.Components.Pages.ProjectPhases
{
    public partial class ProjectStatCards   : ComponentBase
    {
        [Parameter] public ProjectDto? Project { get; set; }
    }
}
