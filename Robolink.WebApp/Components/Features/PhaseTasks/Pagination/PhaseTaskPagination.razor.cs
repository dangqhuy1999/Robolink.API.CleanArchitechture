using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Components.Features.PhaseTasks.Pagination
{
    public partial class PhaseTaskPagination : ComponentBase
    {
        [Parameter] public int CurrentPage { get; set; } = 1;
        [Parameter] public int TotalPages { get; set; } = 1;
        [Parameter] public int TotalTasks { get; set; } = 0;
        [Parameter] public EventCallback<int> OnPageChanged { get; set; }
    }
}
