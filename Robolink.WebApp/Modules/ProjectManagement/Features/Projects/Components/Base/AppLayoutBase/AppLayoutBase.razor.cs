using Microsoft.AspNetCore.Components;
namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Components.Base.AppLayoutBase
{
    public partial class AppLayoutBase : ComponentBase
    {
        [Parameter] public string CssClass { get; set; } = "";

        [Parameter] public RenderFragment? ChildContent { get; set; }
    }
}
