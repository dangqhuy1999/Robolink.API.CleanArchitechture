using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Components.Features.Projects.Shared
{
    public partial class ProjectHierarchyBadge : ComponentBase
    {
        [Parameter]
        public bool HasParent { get; set; }

        [Parameter]
        public bool HasSubProjects { get; set; }

        [Parameter]
        public int SubProjectCount { get; set; } = 0;
    }
}
