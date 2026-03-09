using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Modules.ProjectManagement.Shared.Components.NavigationLayout.Pagination
{
    public partial class ProjectPagination : ComponentBase
    {
        [Parameter] public int CurrentPage { get; set; } = 1;
        [Parameter] public int TotalPages { get; set; } = 1;
        [Parameter] public int TotalProjects { get; set; } = 0;
        [Parameter] public EventCallback<int> OnPageChanged { get; set; }
    }
}
