using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.Application.Commands.Projects;
using Robolink.Application.DTOs;
using Robolink.Application.Queries.Projects;
using Robolink.Application.Queries.Staff;

namespace Robolink.WebApp.Components.Features.Projects.Modals
{
    public partial class QuickAddSubProjectModal : ComponentBase
    {
        [Inject] protected IMediator Mediator { get; set; } = null!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
        [Parameter] public bool ShowModal { get; set; }
        [Parameter] public Guid ParentProjectId { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }
        [Parameter] public EventCallback OnSaved { get; set; }

        private CreateProjectRequest request = new();
        private ProjectDto? parentProject;
        private List<StaffDto> managers = new();
        private bool isLoading = false;

        protected override async Task OnParametersSetAsync()
        {
            if (ShowModal && ParentProjectId != Guid.Empty)
            {
                isLoading = true;
                request = new()
                {
                    StartDate = DateTime.Today,
                    Deadline = DateTime.Today.AddDays(30),
                    Priority = 1
                };

                await LoadParentProject();
                await LoadManagers();
                isLoading = false;
            }
        }

        private async Task LoadParentProject()
        {
            try
            {
                parentProject = await Mediator.Send(new GetProjectByIdQuery(ParentProjectId));
                if (parentProject != null)
                {
                    // Inherit client and some settings from parent
                    request.ClientId = parentProject.ClientId;
                    request.ParentProjectId = ParentProjectId;
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Error loading parent project: {ex.Message}");
            }
        }

        private async Task LoadManagers()
        {
            try
            {
                var result = await Mediator.Send(new GetAllStaffQuery());
                managers = result?.ToList() ?? new();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Error loading managers: {ex.Message}");
            }
        }

        private async Task HandleCreateSubProject()
        {
            try
            {
                var result = await Mediator.Send(new CreateProjectCommand
                {
                    Request = request,
                    CreatedBy = "Huy Dang"
                });

                if (result != null)
                {
                    await JSRuntime.InvokeVoidAsync("alert", $"Sub-project '{result.Name}' created successfully!");
                    await OnSaved.InvokeAsync();
                    await CloseModal();
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Error: {ex.Message}");
            }
        }

        private async Task CloseModal()
        {
            request = new();
            parentProject = null;
            await OnClose.InvokeAsync();
        }
    }
}
