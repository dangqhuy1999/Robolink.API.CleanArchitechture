using Microsoft.AspNetCore.Components;
using Robolink.Shared.DTOs;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.ProjectPhases.Shared
{
    public partial class ProjectStatCards   : ComponentBase
    {
        [Parameter] public ProjectDto? Project { get; set; }
    }
}
