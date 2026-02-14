using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.Application.Commands.PhaseTasks;
using Robolink.Shared.DTOs;
using Robolink.Application.Queries.PhaseTasks;
using Robolink.Application.Queries.Staff;

namespace Robolink.WebApp.Components.Features.PhaseTasks.Modals
{
    public partial class QuickAddSubPhaseTaskModal : ComponentBase
    {
        [Inject] private IMediator Mediator { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Parameter] public bool ShowModal { get; set; }
        [Parameter] public Guid ParentPhaseTaskId { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }
        [Parameter] public EventCallback OnSaved { get; set; }

        private CreatePhaseTaskRequest request = new();
        private PhaseTaskDto? parentPhaseTask;
        private List<StaffDto> staffs = new();
        private bool isLoading = false;

        protected override async Task OnParametersSetAsync()
        {
            if (ShowModal && ParentPhaseTaskId != Guid.Empty)
            {
                isLoading = true;
                request = new()
                {
                    StartDate = DateTime.Today,
                    DueDate = DateTime.Today.AddDays(30),
                    Priority = 1
                };

                await LoadParentPhaseTask();
                await LoadManagers();
                isLoading = false;
            }
        }


        private async Task LoadParentPhaseTask()
        {
            try
            {
                parentPhaseTask = await Mediator.Send(new GetPhaseTaskByIdQuery(ParentPhaseTaskId));
                if (parentPhaseTask != null)
                {
                    // Inherit client and some settings from parent
                    request.AssignedStaffId = parentPhaseTask.AssignedStaffId;
                    request.ParentPhaseTaskId = ParentPhaseTaskId;

                    // PHẢI CÓ 2 DÒNG NÀY:
                    request.ProjectId = parentPhaseTask.ProjectId;
                    request.ProjectSystemPhaseConfigId = parentPhaseTask.ProjectSystemPhaseConfigId;
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
                staffs = result?.ToList() ?? new();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Error loading managers: {ex.Message}");
            }
        }

        private async Task HandleCreateSubPhaseTask()
        {
            try
            {
                var result = await Mediator.Send(new CreatePhaseTaskCommand
                {
                    Request = request,
                    CreatedBy = "Huy Dang"
                });

                if (result != null)
                {
                    await JSRuntime.InvokeVoidAsync("alert", $"Sub-Task created successfully!");
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
            parentPhaseTask = null;
            await OnClose.InvokeAsync();
        }
    }
}
