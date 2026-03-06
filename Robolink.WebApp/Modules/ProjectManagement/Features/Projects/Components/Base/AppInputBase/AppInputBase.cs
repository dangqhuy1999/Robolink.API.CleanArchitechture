
using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;
namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Components.Base.AppInputBase
{
    

    public class AppInputBase<T> : ComponentBase
    {
        [Parameter] public T Value { get; set; }

        [Parameter] public EventCallback<T> ValueChanged { get; set; }

        [Parameter] public Expression<Func<T>> For { get; set; }

        [Parameter] public string Placeholder { get; set; }

        [Parameter] public bool Disabled { get; set; }
    }
}
