using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.Application.Commands.Projects;
using Robolink.Application.DTOs;
using Robolink.Application.Queries.Projects;
using Robolink.Application.Queries.Staff;

namespace Robolink.WebApp.Components.Features.Projects.Modals
{
    public partial class EditProjectModal : ComponentBase
    {
        [Inject] protected IMediator Mediator { get; set; } = null!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
        [Parameter] public bool ShowModal { get; set; }
        [Parameter] public Guid ProjectId { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }
        [Parameter] public EventCallback OnSaved { get; set; }

        private ProjectDto? project;
        private UpdateProjectRequest updateRequest = new();
        private List<StaffDto> managers = new();
        private bool isLoading = false;

        protected override async Task OnParametersSetAsync()
        {
            if (ShowModal && ProjectId != Guid.Empty)
            {
                isLoading = true;
                await LoadProject();
                await LoadManagers();
                isLoading = false;
            }
        }

        private async Task LoadProject()
        {
            try
            {
                project = await Mediator.Send(new GetProjectByIdQuery(ProjectId));
                if (project != null)
                {
                    updateRequest = new UpdateProjectRequest
                    {
                        Id = project.Id,
                        Name = project.Name,
                        Description = project.Description,
                        ManagerId = project.ManagerId,
                        Deadline = project.Deadline,
                        Priority = project.Priority,
                        Status = project.Status,
                        InternalBudget = project.InternalBudget,
                        CustomerBudget = project.CustomerBudget,
                        ParentProjectId = project.ParentProjectId
                    };
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Error loading project: {ex.Message}");
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

        private async Task HandleUpdateProject()
        {
            try
            {
                var result = await Mediator.Send(new UpdateProjectCommand
                {
                    Request = updateRequest,
                    UpdatedBy = "Huy Dang"
                });

                if (result != null)
                {
                    await JSRuntime.InvokeVoidAsync("alert", "Project updated successfully!");
                    await OnSaved.InvokeAsync();
                    await CloseModal();
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Error: {ex.Message}");
            }
        }

        private async Task SelectSubProject(Guid subProjectId)
        {
            // Close current modal and open the sub-project
            await CloseModal();
            // This would be handled by parent component
        }

        private async Task CloseModal()
        {
            project = null;
            updateRequest = new();
            await OnClose.InvokeAsync();
        }
    }
}
