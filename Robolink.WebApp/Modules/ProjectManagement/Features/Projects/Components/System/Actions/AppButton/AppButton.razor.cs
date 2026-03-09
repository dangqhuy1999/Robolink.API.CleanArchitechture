using Microsoft.AspNetCore.Components;
namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Components.System.Actions.AppButton
{
    public partial class AppButton : ComponentBase
    {
        [Parameter] public string Theme { get; set; } = "primary"; // success, info, danger...
        [Parameter] public string? Icon { get; set; }
        [Parameter] public bool IsLoading { get; set; }
        [Parameter] public bool IsDisabled { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public EventCallback OnClick { get; set; }

        private string GetThemeClass() => $"btn-{Theme}"; // Tự động map "success" -> "btn-success"
    }
}
