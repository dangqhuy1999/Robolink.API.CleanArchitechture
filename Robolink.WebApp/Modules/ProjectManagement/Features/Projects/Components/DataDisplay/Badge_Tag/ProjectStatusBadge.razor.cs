using Microsoft.AspNetCore.Components;
using Robolink.Shared.Enums;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Shared
{
    public partial class ProjectStatusBadge : ComponentBase
    {
        [Parameter] public ProjectStatus Status  { get; set; }
    }
}
