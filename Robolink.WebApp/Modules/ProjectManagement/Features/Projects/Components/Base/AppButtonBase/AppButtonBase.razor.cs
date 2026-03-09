using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Components.Base.AppButtonBase
{
    public  partial class AppButtonBase : ComponentBase
    {
        [Parameter] public string Class { get; set; } = "";
        [Parameter] public bool IsLoading { get; set; }
        [Parameter] public bool IsDisabled { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public EventCallback OnClick { get; set; }

        private string MergedClasses => $"btn {Class}";

        private async Task HandleClickAsync()
        {
            if (!IsLoading && !IsDisabled) await OnClick.InvokeAsync();
        }
    }
}
