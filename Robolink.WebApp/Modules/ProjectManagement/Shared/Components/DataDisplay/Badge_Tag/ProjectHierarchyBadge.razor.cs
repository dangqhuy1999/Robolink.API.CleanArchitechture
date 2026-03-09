using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Modules.ProjectManagement.Shared.Components.DataDisplay.Badge_Tag
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
