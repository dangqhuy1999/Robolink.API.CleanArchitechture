using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Components.Features.SystemPhases.Pagination
{
    public partial class SystemPhasePagination : ComponentBase
    {
        [Parameter]
        public int CurrentPage { get; set; } = 1;

        [Parameter]
        public int TotalPages { get; set; }

        [Parameter]
        public int TotalPhases { get; set; }

        [Parameter]
        public EventCallback<int> OnPageChanged { get; set; }
    }
}
