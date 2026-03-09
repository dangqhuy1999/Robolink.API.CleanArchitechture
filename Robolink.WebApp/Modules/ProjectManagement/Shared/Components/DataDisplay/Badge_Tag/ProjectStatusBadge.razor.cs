using Microsoft.AspNetCore.Components;
using Robolink.Shared.Enums;

namespace Robolink.WebApp.Modules.ProjectManagement.Shared.Components.DataDisplay.Badge_Tag
{
    public partial class ProjectStatusBadge : ComponentBase
    {
        [Parameter] public ProjectStatus Status  { get; set; }
    }
}
