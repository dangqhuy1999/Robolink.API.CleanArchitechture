
using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Components.Base.AppFieldBase
{
    
    public partial class AppFieldBase<T> : ComponentBase
    {
        [Parameter] public string Label { get; set; }

        [Parameter] public bool Required { get; set; }

        [Parameter] public Expression<Func<T>> For { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
