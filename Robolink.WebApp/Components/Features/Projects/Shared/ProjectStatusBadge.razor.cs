using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Components.Features.Projects.Shared
{
    public partial class ProjectStatusBadge : ComponentBase
    {
        [Parameter] public int Status { get; set; }
    }
}
