using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.Application.DTOs;
using Robolink.Application.Queries.Projects;

namespace Robolink.WebApp.Components.Features.Projects.Shared
{
    public partial class ParentProjectSelector : ComponentBase
    {
        [Inject] protected IMediator Mediator { get; set; } = null!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
        [Parameter]
        public Guid? SelectedParentId { get; set; }

        [Parameter]
        public EventCallback<Guid?> SelectedParentIdChanged { get; set; }

        [Parameter]
        public Guid? ExcludeProjectId { get; set; }

        [Parameter]
        public string? CurrentParentName { get; set; }


        private List<ProjectDto> AvailableProjects = new();

        protected override async Task OnInitializedAsync()
        {
            await RefreshProjects();
        }

        private async Task OnValueChanged(Guid? value)
        {
            SelectedParentId = value; // Cập nhật nội bộ
            await SelectedParentIdChanged.InvokeAsync(value); // Báo cho cha (Modal) biết để cập nhật updateRequest
        }

        private async Task RefreshProjects()
        {
            try
            {
                var result = await Mediator.Send(new GetAllProjectsQuery());
                AvailableProjects = result?.ToList() ?? new();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading projects: {ex.Message}");
            }
        }
    }
}
