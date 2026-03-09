using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Robolink.WebApp.Modules.ProjectManagement.Shared.UI;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Components.Base.AppInputBase
{ 

    public abstract class AppInputBase<T> : InputBase<T>
    {
        [Parameter] public string? Id { get; set; }

        [Parameter] public string? Placeholder { get; set; }

        [Parameter] public bool Disabled { get; set; }

        [Parameter] public string CssClass { get; set; } = Css.Input;

        protected string InputCssClass
            => $"{CssClass} {(EditContext?.GetValidationMessages(FieldIdentifier).Any() == true ? "is-invalid" : "")}";
    }
}
